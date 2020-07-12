using System;
using System.Collections.Generic;
using CfAnalytics.QuantLib.InternalUtils;
using QlYts = QuantLib.YieldTermStructure;
using QlQuote=QuantLib.SimpleQuote;
using QlQuoteHandle = QuantLib.QuoteHandle;
using QlDepoHelper = QuantLib.DepositRateHelper;
using QlSwapHelper = QuantLib.SwapRateHelper;

namespace CfAnalytics.QuantLib.TermStructures.YieldTermStructures
{
    public class SwapCurveBootstrapBuilder : YieldTermStructure.BuilderBase
    {
        public SwapCurveBootstrapBuilder(DateTime tradeDate)
            : base(tradeDate)
        {
            DepositRates = new List<(Period tenor, double depositRate)>();
        }

        public DayCounter DepositBasis { get; set; }
        public BusinessDayConvention DepositRollConvention { get; set; }
        public ICollection<(Period tenor, double depositRate)> DepositRates { get; set; }

        public Frequency SwapFixedFrequency { get; set; }
        public BusinessDayConvention SwapFixedRollConvention { get; set; }
        public DayCounter SwapFixedBasis { get; set; }
        public string SwapFloatIndex { get; set; }
        public ICollection<(Period tenor,double swapRate)> SwapRates { get; set; }

        internal override QlYts Build()
        {
            var calendar = Calendar.ToQlCalendar();
            var termStructureDayCounter = DayCountBasis.ToQlDayCounter();
            var spotDate = calendar.advance(TradeDate, SpotDays, global::QuantLib.TimeUnit.Days);
            var rateHelpers = new global::QuantLib.RateHelperVector();

            // Deposits
            var depoConv = DepositRollConvention.ToQlConvention();
            var depoDayCount = DepositBasis.ToQlDayCounter();
            foreach (var rate in DepositRates)
            {
                var quote = new QlQuoteHandle(new QlQuote(rate.depositRate));
                var depoHelper = new QlDepoHelper(quote, rate.tenor.QlObj, Convert.ToUInt32(SpotDays), calendar, depoConv, true, depoDayCount);
                rateHelpers.Add(depoHelper);
            }

            // Swaps
            var swapFixedFreq = SwapFixedFrequency.ToQlFrequency();
            var swapFixedRoll = SwapFixedRollConvention.ToQlConvention();
            var swapFixedBasis = SwapFixedBasis.ToQlDayCounter();
            var floatIdx = IborIndexHelper.GetIborIndex(SwapFloatIndex);
            foreach (var rate in SwapRates)
            {
                var quote = new QlQuoteHandle(new QlQuote(rate.swapRate));
                var swapHelper = new QlSwapHelper(quote, rate.tenor.QlObj, calendar, swapFixedFreq, swapFixedRoll, swapFixedBasis, floatIdx);
                rateHelpers.Add(swapHelper);
            }

            return new global::QuantLib.PiecewiseLogLinearDiscount(spotDate, rateHelpers, termStructureDayCounter);
        }
    }
}