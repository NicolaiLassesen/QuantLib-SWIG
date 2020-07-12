using System;
using System.Collections.Generic;
using CfAnalytics.QuantLib.InternalUtils;
using QlDate = QuantLib.Date;
using QlCalendar = QuantLib.Calendar;
using QlDayCounter = QuantLib.DayCounter;
using QlConv = QuantLib.BusinessDayConvention;
using QlQuote = QuantLib.QuoteHandle;
using QlDepoHelper = QuantLib.DepositRateHelper;
using QlRateVector = QuantLib.RateHelperVector;
using QlYts = QuantLib.YieldTermStructure;
using QlYtsHandle = QuantLib.YieldTermStructureHandle;
using QlYtsRelinkHandle = QuantLib.RelinkableYieldTermStructureHandle;
// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib.TermStructures
{
    public class YieldTermStructure : TermStructure<QlYts>
    {
        public class Builder
        {
            public Builder(DateTime tradeDate)
            {
                TradeDate = tradeDate;
            }

            public DateTime TradeDate { get; }
            public int SpotDays { get; set; }

            public Currency Currency { get; set; }
            public Calendar Calendar { get; set; }
            public DayCounter DayCounter { get; set; }

            public DayCounter DepositDayCounter { get; set; }
            public BusinessDayConvention DepositConvention { get; set; }
            public ICollection<(Period tenor, double depositRate)> DepositRates { get; set; }

            internal QlYts BuildBootstrapper()
            {
                QlCalendar calendar = Calendar.ToQlCalendar();
                QlDayCounter termStructureDayCounter = DayCounter.ToQlDayCounter();
                QlDate spotDate = calendar.advance(TradeDate, SpotDays, global::QuantLib.TimeUnit.Days);
                var rateHelpers = new QlRateVector();

                // Deposits
                QlConv depoConv = DepositConvention.ToQlConvention();
                QlDayCounter depoDayCount = DepositDayCounter.ToQlDayCounter();
                foreach (var rate in DepositRates)
                {
                    var quote = new QlQuote(new global::QuantLib.SimpleQuote(rate.depositRate));
                    var depoHelper = new QlDepoHelper(quote, rate.tenor.QlObj, Convert.ToUInt32(SpotDays), calendar, depoConv, true, depoDayCount);
                    rateHelpers.Add(depoHelper);
                }

                return new global::QuantLib.PiecewiseLogLinearDiscount(spotDate, rateHelpers, termStructureDayCounter);
            }
        }

        public YieldTermStructure(Builder builder)
            : base(builder.BuildBootstrapper())
        {
        }

        internal QlYtsHandle GetHandle()
        {
            return new QlYtsHandle(QlObj);
        }

        internal QlYtsRelinkHandle GetRelinkableHandle()
        {
            return new QlYtsRelinkHandle(QlObj);
        }

        #region Overrides of TermStructure

        public override string ToString()
        {
            return $"{nameof(YieldTermStructure)} {QlObj.referenceDate().ISO()}";
        }

        #endregion
    }
}