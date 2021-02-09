using System;
using CfAnalytics.QuantLib.InternalUtils;
using QlIrs = QuantLib.VanillaSwap;

namespace CfAnalytics.QuantLib.Instruments
{
    public class VanillaSwap : Instrument<IrsImpl>
    {
        public class Builder
        {
            public Builder(DateTime effectiveDate, DateTime maturity)
            {
                EffectiveDate = effectiveDate;
                Maturity = maturity;
            }

            public DateTime EffectiveDate { get; }
            public DateTime Maturity { get; }

            public SwapType Type { get; set; }
            public double Notional { get; set; }
            public CalendarName Calendar { get; set; }
            public double FixedRate { get; set; }
            public Frequency FixedFrequency { get; set; }
            public DayCounter FixedDaycount { get; set; }
            public BusinessDayConvention FixedConvention { get; set; }
            public Frequency FloatFrequency { get; set; }
            public DayCounter FloatDaycount { get; set; }
            public BusinessDayConvention FloatConvention { get; set; }
            public IborIndex FloatIndex { get; set; }
            public double FloatSpread { get; set; }

            internal QlIrs Build()
            {
                var fixedScheduleBuilder = new Schedule.Builder(EffectiveDate, Maturity)
                {
                    Frequency = FixedFrequency,
                    Calendar = Calendar,
                    Convention = FixedConvention,
                    Rule = Utilities.Schedule.DateGeneration.Backward
                };
                var fixedSchedule = new Schedule(fixedScheduleBuilder);
                var floatScheduleBuilder = new Schedule.Builder(EffectiveDate, Maturity)
                {
                    Frequency = FloatFrequency,
                    Calendar = Calendar,
                    Convention = FloatConvention,
                    Rule = Utilities.Schedule.DateGeneration.Backward
                };
                var floatSchedule = new Schedule(floatScheduleBuilder);
                return new QlIrs(Type == SwapType.Payer ? QlIrs.Type.Payer : QlIrs.Type.Receiver,
                    Notional, fixedSchedule.QlObj, FixedRate, FixedDaycount.ToQlDayCounter(),
                    floatSchedule.QlObj, (global::QuantLib.IborIndex)FloatIndex.Impl.QlObj, FloatSpread, FloatDaycount.ToQlDayCounter());
            }
        }

        public VanillaSwap(Builder builder)
            : base(new IrsImpl(builder.Build()))
        {
        }

        public DateTime EffectiveDate => Impl.QlObj.startDate().AsDateTime();
        public DateTime MaturityDate => Impl.QlObj.maturityDate().AsDateTime();
        public double Notional => Impl.QlObj.nominal();

        public double FairRate => Impl.QlObj.fairRate();
        public double FairSpread => Impl.QlObj.fairSpread();
        public double FixedLegNpv => Impl.QlObj.fixedLegNPV();
        public double FixedLegBps => Impl.QlObj.fixedLegBPS();
        public double FloatLegNpv => Impl.QlObj.floatingLegNPV();
        public double FloatLegBps => Impl.QlObj.floatingLegBPS();
    }

    public class IrsImpl : InstrumentImpl<QlIrs>
    {
        public IrsImpl(QlIrs qlObj)
            : base(qlObj)
        {
        }
    }
}