using System;
using QlYts = QuantLib.YieldTermStructure;
using QlYtsHandle = QuantLib.YieldTermStructureHandle;
using QlYtsRelinkHandle = QuantLib.RelinkableYieldTermStructureHandle;
using QlDaycounter = QuantLib.DayCounter;
// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib.TermStructures
{
    public class YieldTermStructure : TermStructure<YieldTermStructureImpl>
    {
        public abstract class BuilderBase
        {
            protected BuilderBase(DateTime tradeDate, bool isBootstrapped)
            {
                TradeDate = tradeDate;
                IsBootstrapped = isBootstrapped;
            }

            public DateTime TradeDate { get; }
            public virtual int SpotDays { get; set; }

            public Currency Currency { get; set; }
            public virtual CalendarName Calendar { get; set; }
            public virtual DayCounter DayCountBasis { get; set; }

            public bool IsBootstrapped { get; }

            internal abstract QlYts Build();
        }

        public YieldTermStructure(BuilderBase builder)
            : base(new YieldTermStructureImpl(builder.Build()))
        {
        }

        internal QlYtsHandle GetHandle()
        {
            return new QlYtsHandle(Impl.QlObj);
        }

        internal QlYtsRelinkHandle GetRelinkableHandle()
        {
            return new QlYtsRelinkHandle(Impl.QlObj);
        }

        public double Discount(DateTime date, bool extrapolate = false)
        {
            return Impl.QlObj.discount(date, extrapolate);
        }

        public double Discount(double time, bool extrapolate = false)
        {
            return Impl.QlObj.discount(time, extrapolate);
        }

        public InterestRate ZeroRate(double time, bool extrapolate = false)
        {
            return extrapolate
                ? new InterestRate(Impl.QlObj.zeroRate(time, global::QuantLib.Compounding.Continuous, global::QuantLib.Frequency.NoFrequency, true))
                : new InterestRate(Impl.QlObj.zeroRate(time, global::QuantLib.Compounding.Continuous));
        }

        public InterestRate ZeroRate(DateTime date, bool extrapolate = false)
        {
            return extrapolate
                ? new InterestRate(Impl.QlObj.zeroRate(date, Impl.DayCounter, global::QuantLib.Compounding.Continuous, global::QuantLib.Frequency.NoFrequency, true))
                : new InterestRate(Impl.QlObj.zeroRate(date, Impl.DayCounter, global::QuantLib.Compounding.Continuous));
        }

        #region Overrides of TermStructure

        public override string ToString()
        {
            return $"{nameof(YieldTermStructure)} {Impl.QlObj.referenceDate().ISO()}";
        }

        #endregion
    }

    public class YieldTermStructureImpl : TermStructureImpl<QlYts>
    {
        public YieldTermStructureImpl(QlYts qlObj)
            : base(qlObj)
        {
        }

        internal QlDaycounter DayCounter => QlObj.dayCounter();
    }
}