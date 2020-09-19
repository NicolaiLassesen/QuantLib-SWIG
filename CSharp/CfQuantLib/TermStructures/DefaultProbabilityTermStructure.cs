using System;
using QlDefProbTs = QuantLib.DefaultProbabilityTermStructure;
using QlDefProbTsHandle = QuantLib.DefaultProbabilityTermStructureHandle;
using QlDefProbTsRelinkHandle = QuantLib.RelinkableDefaultProbabilityTermStructureHandle;

namespace CfAnalytics.QuantLib.TermStructures
{
    public class DefaultProbabilityTermStructure : TermStructure<DefaultProbabilityTermStructureImpl>
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
            public CalendarName Calendar { get; set; }
            public DayCounter DayCountBasis { get; set; }

            internal abstract QlDefProbTs Build();
        }

        public DefaultProbabilityTermStructure(BuilderBase builder)
            : base(new DefaultProbabilityTermStructureImpl(builder.Build()))
        {
        }

        internal QlDefProbTsHandle GetHandle()
        {
            return new QlDefProbTsHandle(Impl.QlObj);
        }

        internal QlDefProbTsRelinkHandle GetRelinkableHandle()
        {
            return new QlDefProbTsRelinkHandle(Impl.QlObj);
        }

        public bool AllowsExtrapolation
        {
            get => Impl.QlObj.allowsExtrapolation();
            set
            {
                if (value)
                    Impl.QlObj.enableExtrapolation();
                else
                    Impl.QlObj.disableExtrapolation();
            }
        }

        public double TimeFromReference(DateTime date)
        {
            return Impl.QlObj.timeFromReference(date);
        }

        public double DefaultProbability(DateTime date, bool extrapolate = false)
        {
            return Impl.QlObj.defaultProbability(date, extrapolate);
        }

        public double DefaultProbability(double time, bool extrapolate = false)
        {
            return Impl.QlObj.defaultProbability(time, extrapolate);
        }

        public double SurvivalProbability(DateTime date, bool extrapolate = false)
        {
            return Impl.QlObj.survivalProbability(date, extrapolate);
        }

        public double SurvivalProbability(double time, bool extrapolate = false)
        {
            return Impl.QlObj.survivalProbability(time, extrapolate);
        }

        public double HazardRate(DateTime date, bool extrapolate = false)
        {
            return Impl.QlObj.hazardRate(date, extrapolate);
        }

        public double HazardRate(double time, bool extrapolate = false)
        {
            return Impl.QlObj.hazardRate(time, extrapolate);
        }

        public override string ToString()
        {
            return Impl.QlObj.ToString();
        }
    }

    public class DefaultProbabilityTermStructureImpl : TermStructureImpl<QlDefProbTs>
    {
        public DefaultProbabilityTermStructureImpl(QlDefProbTs qlObj)
            : base(qlObj)
        {
        }
    }
}