using System;
using CfAnalytics.Models.Identifiers;
using CfAnalytics.QuantLib.InternalUtils;
using CfAnalytics.Utilities;
using JetBrains.Annotations;
using QuantLib;
using QlCds = QuantLib.CreditDefaultSwap;

namespace CfAnalytics.QuantLib.Instruments
{
    public class CreditDefaultSwap : Instrument<CdsImpl>
    {
        public enum Side
        {
            Buyer,
            Seller
        }

        public class Builder
        {
            public Builder([NotNull] string creditTicker, Currency currency, string seniority, string restructuringType, IQuote couponRate)
            {
                if (string.IsNullOrWhiteSpace(creditTicker))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(creditTicker));
                CreditTicker = creditTicker;
                Currency = currency;
                Seniority = seniority;
                RestructuringType = restructuringType;
                CouponRate = couponRate;
            }

            public string CreditTicker { get; }
            public Currency Currency { get; }
            public string Seniority { get; }
            public string RestructuringType { get; }
            public IQuote CouponRate { get; }

            public Side Protection { get; set; } = Side.Buyer;
            public double Notional { get; set; } = 1000000.0;
            public double UpfrontRate { get; set; } = 0.0;
            public DateTime? EffectiveDate { get; set; }
            public DateTime? Maturity { get; set; }
            public Period? Tenor { get; set; }
            public Schedule Schedule { get; set; }
            public CalendarName Calendar { get; set; } = CalendarName.WeekendsOnly;

            public Frequency CouponFrequency { get; set; } = Frequency.Quarterly;

            //public BusinessDayConvention PaymentConvention { get; set; } = BusinessDayConvention.Following;
            public DayCounter DayCountBasis { get; set; } = DayCounter.Actual360;
            public DayCounter LastPeriodDayCountBasis { get; set; } = DayCounter.Actual360LD;

            internal QlCds Build()
            {
                if (Schedule == null)
                    Schedule = BuildSchedule();
                return new QlCds(Protection.ToQlSide(), Notional, UpfrontRate, Convert.ToDouble(CouponRate.RealValue), Schedule.QlObj,
                    global::QuantLib.BusinessDayConvention.Following, DayCountBasis.ToQlDayCounter());
            }

            private Schedule BuildSchedule()
            {
                DateTime startDate = EffectiveDate ?? Settings.EvaluationDate.AddDays(1);
                DateTime maturity = GetMaturity();
                var builder = new Schedule.Builder(startDate, maturity)
                {
                    Frequency = CouponFrequency,
                    Calendar = Calendar,
                    Convention = BusinessDayConvention.Following,
                    TerminationDateConvention = BusinessDayConvention.Unadjusted,
                    Rule = Utilities.Schedule.DateGeneration.Cds,
                    EndOfMonth = false
                };
                return new Schedule(builder);
            }

            private DateTime GetMaturity()
            {
                if (Maturity.HasValue)
                    return Maturity.Value;
                if (Tenor.HasValue)
                    return EffectiveDate?.AddPeriod(Tenor.Value.AsCfPeriod()) ?? Settings.QlEvaluationDate.Add(Tenor.Value.QlObj).AsDateTime();
                throw new ArgumentException("Missing either Maturity or Tenor to build CDS Schedule");
            }
        }

        public CreditDefaultSwap(Builder builder)
            : base(new CdsImpl(builder.Build()))
        {
            int size = builder.Schedule.Size;
            var frequency = builder.Schedule.Frequency;
            //var tenor = builder.Schedule.Tenor;
            DateTime startDate = builder.Schedule.StartDate;
            DateTime endDate = builder.Schedule.EndDate;
            var timeSpan = endDate - startDate;

            if (frequency == Frequency.Quarterly && size == 22 && timeSpan.Days > 5 * 365 && timeSpan.Days < 5 * 365 + 183)
                CreditId = new CreditCurveId(builder.Currency, builder.CreditTicker, builder.Seniority, builder.RestructuringType, new Utilities.Period(5, TimeUnit.Years));
            else if (frequency == Frequency.Quarterly && size == 18 && timeSpan.Days > 4 * 365 && timeSpan.Days < 4 * 365 + 183)
                CreditId = new CreditCurveId(builder.Currency, builder.CreditTicker, builder.Seniority, builder.RestructuringType, new Utilities.Period(4, TimeUnit.Years));
            else if (frequency == Frequency.Quarterly && size == 14 && timeSpan.Days > 3 * 365 && timeSpan.Days < 3 * 365 + 183)
                CreditId = new CreditCurveId(builder.Currency, builder.CreditTicker, builder.Seniority, builder.RestructuringType, new Utilities.Period(3, TimeUnit.Years));
            else if (frequency == Frequency.Quarterly && size == 10 && timeSpan.Days > 2 * 365 && timeSpan.Days < 2 * 365 + 183)
                CreditId = new CreditCurveId(builder.Currency, builder.CreditTicker, builder.Seniority, builder.RestructuringType, new Utilities.Period(2, TimeUnit.Years));
            else if (frequency == Frequency.Quarterly && size == 6 && timeSpan.Days > 365 && timeSpan.Days < 365 + 183)
                CreditId = new CreditCurveId(builder.Currency, builder.CreditTicker, builder.Seniority, builder.RestructuringType, new Utilities.Period(1, TimeUnit.Years));
            else
                CreditId = new CreditCurveId(builder.Currency, builder.CreditTicker, builder.Seniority, builder.RestructuringType);
        }

        public Side ProtectionSide
        {
            get
            {
                var value = Impl.QlObj.side();
                switch (value)
                {
                    case Protection.Side.Buyer: return Side.Buyer;
                    case Protection.Side.Seller: return Side.Seller;
                    default: throw new ArgumentOutOfRangeException(nameof(ProtectionSide), value.ToString());
                }
            }
        }

        public CreditCurveId CreditId { get; }

        public double Notional => Impl.QlObj.notional();
        public Utilities.Quote RunningSpread => new Utilities.Quote(Impl.QlObj.runningSpread(), QuoteType.Bps);
        public DateTime ProtectionStartDate => Impl.QlObj.protectionStartDate().AsDateTime();
        public DateTime ProtectionEndDate => Impl.QlObj.protectionEndDate().AsDateTime();
        public DateTime Maturity => Impl.QlObj.maturityDate().AsDateTime();

        public Utilities.Quote TradeSpread => new Utilities.Quote(Impl.QlObj.fairSpread(), QuoteType.Bps);
        public double CouponLegValue => Impl.QlObj.couponLegNPV();
        public double CouponLegBps => Impl.QlObj.couponLegBPS();
        public double ProtectionLegValue => Impl.QlObj.defaultLegNPV();
        public double ProtectionLegBps => 0; //QlObj.d
        public double SpreadDv01 => 0;
        public double InterestRateDv01 => 0;
    }

    public class CdsImpl : InstrumentImpl<QlCds>
    {
        public CdsImpl(QlCds qlObj)
            : base(qlObj)
        {
        }
    }
}