using System;
using QlYts = QuantLib.YieldTermStructure;
using QlYtsHandle = QuantLib.YieldTermStructureHandle;
using QlYtsRelinkHandle = QuantLib.RelinkableYieldTermStructureHandle;
// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib.TermStructures
{
    public class YieldTermStructure : TermStructure<QlYts>
    {
        public abstract class BuilderBase
        {
            protected BuilderBase(DateTime tradeDate)
            {
                TradeDate = tradeDate;
            }

            public DateTime TradeDate { get; }
            public int SpotDays { get; set; }

            public Currency Currency { get; set; }
            public Calendar Calendar { get; set; }
            public DayCounter DayCountBasis { get; set; }

            internal abstract QlYts Build();
        }

        public YieldTermStructure(BuilderBase builder)
            : base(builder.Build())
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