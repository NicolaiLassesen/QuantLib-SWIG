using System;
using CfAnalytics.QuantLib.InternalUtils;
using QlSchedule = QuantLib.Schedule;
using QlScheduleBuilder = QuantLib.MakeSchedule;

namespace CfAnalytics.QuantLib
{
    public class Schedule
    {
        public class Builder
        {
            public Builder(DateTime startDate, DateTime endDate)
            {
                StartDate = startDate;
                EndDate = endDate;
            }

            public DateTime StartDate { get; }
            public DateTime EndDate { get; }

            public Frequency? Frequency { get; set; }
            public Period? Tenor { get; set; }
            public CalendarName Calendar { get; set; }
            public BusinessDayConvention? Convention { get; set; }
            public BusinessDayConvention? TerminationDateConvention { get; set; }
            public Utilities.Schedule.DateGeneration? Rule { get; set; }
            public bool EndOfMonth { get; set; }
            public DateTime? FirstDate { get; set; }
            public DateTime? NextToLastDate { get; set; }
            internal QlSchedule Build()
            {
                var builder = new QlScheduleBuilder().from(StartDate).to(EndDate).endOfMonth(EndOfMonth);
                if (Frequency.HasValue && Tenor.HasValue)
                    throw new ArgumentException("Both Frequency and Tenor were supplied while only one is allowed");
                if (Frequency.HasValue)
                    builder = builder.withFrequency(Frequency.Value.ToQlFrequency());
                else if (Tenor.HasValue)
                    builder = builder.withTenor(Tenor.Value.QlObj);

                if (Calendar != CalendarName.Unknown)
                    builder = builder.withCalendar(Calendar.ToQlCalendar());
                if (Convention.HasValue)
                    builder = builder.withConvention(Convention.Value.ToQlConvention());
                if (TerminationDateConvention.HasValue)
                    builder = builder.withTerminationDateConvention(TerminationDateConvention.Value.ToQlConvention());
                if (Rule.HasValue)
                    builder = builder.withRule(Rule.Value.ToQlDateGeneration());
                if (FirstDate.HasValue)
                    builder = builder.withFirstDate(FirstDate.Value);
                if (NextToLastDate.HasValue)
                    builder = builder.withNextToLastDate(NextToLastDate.Value);

                return builder.schedule();
            }
        }

        internal Schedule(Builder builder)
        {
            QlObj = builder.Build();
        }

        internal QlSchedule QlObj { get; }

        public DateTime StartDate => QlObj.startDate().AsDateTime();

        public DateTime EndDate => QlObj.endDate().AsDateTime();

        public Frequency Frequency => QlObj.tenor().frequency().ToCfFrequency();

        public Period Tenor => new Period(QlObj.tenor());

        public int Size => Convert.ToInt32(QlObj.size());
    }
}