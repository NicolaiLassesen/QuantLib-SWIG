using System;
using System.Collections.Generic;
using System.Linq;
using CfAnalytics.QuantLib.InternalUtils;
using QlYts = QuantLib.YieldTermStructure;
using QlQuote = QuantLib.SimpleQuote;
using QlQuoteHandle = QuantLib.QuoteHandle;
using QlDepoHelper = QuantLib.DepositRateHelper;
using QlSwapHelper = QuantLib.SwapRateHelper;

namespace CfAnalytics.QuantLib.TermStructures.YieldTermStructures
{
    public class SwapCurveBootstrapBuilder : YieldTermStructure.BuilderBase
    {
        public SwapCurveBootstrapBuilder(DateTime tradeDate)
            : base(tradeDate, true)
        {
            DepositRates = new List<(Period tenor, double depositRate)>();
            SwapRates = new List<(Period tenor, double swapRate)>();
            RateHelpers = new List<RateHelper>();
        }

        public CalendarName DepositCalendar { get; set; }
        public DayCounter DepositBasis { get; set; }
        public bool DepositUseEndOfMonth { get; set; }
        public BusinessDayConvention DepositRollConvention { get; set; }
        public ICollection<(Period tenor, double depositRate)> DepositRates { get; set; }

        public CalendarName SwapCalendar { get; set; }
        public Frequency SwapFixedFrequency { get; set; }
        public BusinessDayConvention SwapFixedRollConvention { get; set; }
        public DayCounter SwapFixedBasis { get; set; }
        public string SwapFloatIndex { get; set; }
        public ICollection<(Period tenor, double swapRate)> SwapRates { get; set; }

        public ICollection<RateHelper> RateHelpers { get; }

        internal override QlYts Build()
        {
            var calendar = Calendar.ToQlCalendar();
            var termStructureDayCounter = DayCountBasis.ToQlDayCounter();
            var rateHelpers = new global::QuantLib.RateHelperVector();

            // Deposits
            var depoCalendar = DepositCalendar == CalendarName.Unknown ? calendar : DepositCalendar.ToQlCalendar();
            var depoConv = DepositRollConvention.ToQlConvention();
            var depoDayCount = DepositBasis.ToQlDayCounter();
            foreach (var rate in DepositRates)
            {
                var quote = new QlQuoteHandle(new QlQuote(rate.depositRate));
                var depoHelper = new QlDepoHelper(quote, rate.tenor.QlObj, Convert.ToUInt32(SpotDays), depoCalendar, depoConv, DepositUseEndOfMonth, depoDayCount);
                rateHelpers.Add(depoHelper);
                RateHelpers.Add(new RateHelper(depoHelper));
            }

            // Swaps
            if (SwapRates.Any())
            {
                var swapFixedFreq = SwapFixedFrequency.ToQlFrequency();
                var swapCalendar = SwapCalendar == CalendarName.Unknown ? calendar : SwapCalendar.ToQlCalendar();
                var swapFixedRoll = SwapFixedRollConvention.ToQlConvention();
                var swapFixedBasis = SwapFixedBasis.ToQlDayCounter();
                var floatIdx = IborIndexHelper.GetIborIndexInternal(SwapFloatIndex, null);
                foreach (var rate in SwapRates)
                {
                    var quote = new QlQuoteHandle(new QlQuote(rate.swapRate));
                    var swapHelper = new QlSwapHelper(quote, rate.tenor.QlObj, swapCalendar, swapFixedFreq, swapFixedRoll, swapFixedBasis, floatIdx);
                    rateHelpers.Add(swapHelper);
                    RateHelpers.Add(new RateHelper(swapHelper));
                }
            }

            return new global::QuantLib.PiecewiseLogLinearDiscount(TradeDate, rateHelpers, termStructureDayCounter);
        }
    }
}