using System;
using QlIrs = QuantLib.VanillaSwap;

namespace CfAnalytics.QuantLib.Instruments
{
    public class VanillaSwap : Instrument<IrsImpl>
    {
        public enum Type
        {
            Payer,
            Receiver
        }

        public class Builder
        {
            public Builder(DateTime startDate, DateTime maturity)
            {
                StartDate = startDate;
                Maturity = maturity;
            }

            public DateTime StartDate { get; }
            public DateTime Maturity { get; }

            public Type Type { get; set; }
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
                var fixedScheduleBuilder = new Schedule.Builder(StartDate, Maturity)
                {
                    Frequency = FixedFrequency,
                    Calendar = Calendar,
                    Convention = FixedConvention,
                    Rule = Utilities.Schedule.DateGeneration.Backward
                };
                var fixedSchedule = new Schedule(fixedScheduleBuilder);
                var floatScheduleBuilder = new Schedule.Builder(StartDate, Maturity)
                {
                    Frequency = FloatFrequency,
                    Calendar = Calendar,
                    Convention = FloatConvention,
                    Rule = Utilities.Schedule.DateGeneration.Backward
                };
                var floatSchedule = new Schedule(floatScheduleBuilder);
                return new QlIrs(Type == Type.Payer ? QlIrs.Type.Payer : QlIrs.Type.Receiver,
                    Notional, fixedSchedule.QlObj, FixedRate, FixedDaycount.ToQlDayCounter(),
                    floatSchedule.QlObj, (global::QuantLib.IborIndex)FloatIndex.Impl.QlObj, FloatSpread, FloatDaycount.ToQlDayCounter());
            }
        }

        public VanillaSwap(Builder builder)
            : base(new IrsImpl(builder.Build()))
        {
        }
    }

    public class IrsImpl : InstrumentImpl<QlIrs>
    {
        public IrsImpl(QlIrs qlObj)
            : base(qlObj)
        {
        }
    }
}