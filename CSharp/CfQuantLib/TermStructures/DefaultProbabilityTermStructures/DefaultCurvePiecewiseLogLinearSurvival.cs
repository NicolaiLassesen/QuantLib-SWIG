using System;
using System.Collections.Generic;
using System.Linq;
using CfAnalytics.QuantLib.InternalUtils;
using CfSchedule = CfAnalytics.Utilities.Schedule;

namespace CfAnalytics.QuantLib.TermStructures.DefaultProbabilityTermStructures
{
    public enum CdsQuoteType
    {
        Upfront,
        Spread
    }

    /// <summary>
    /// Defaults are set to ISDA model default values
    /// </summary>
    public class DefaultCurvePiecewiseLogLinearSurvival : DefaultProbabilityTermStructure.BuilderBase
    {
        public DefaultCurvePiecewiseLogLinearSurvival(DateTime tradeDate)
            : base(tradeDate)
        {
            CdsQuotes = new List<(Period tenor, CdsQuoteType type, double quoteValue)>();
        }

        public ICollection<(Period tenor, CdsQuoteType type, double quoteValue)> CdsQuotes { get; set; }
        public int SettlementDays { get; set; } = 1;
        public CalendarName CdsQuoteCalendar { get; set; } = CalendarName.WeekendsOnly;
        public Frequency CdsQuoteFrequency { get; set; } = Frequency.Quarterly;
        public DayCounter CdsQuoteBasis { get; set; } = DayCounter.Actual360;
        public BusinessDayConvention CdsQuoteRollConvention { get; set; } = BusinessDayConvention.Following;
        public CfSchedule.DateGeneration CdsQuoteDateGenerationRule { get; set; } = CfSchedule.DateGeneration.Cds;
        public double? CdsQuoteRecoveryRate { get; set; }
        public YieldTermStructure DiscountCurve { get; set; }

        public int UpfrontSettlementDays { get; set; } = 0;
        public bool SettlesAccrual { get; set; } = true;
        public bool PaysAtDefaultTime { get; set; } = true;
        public DateTime? StartDate { get; set; }
        public DayCounter LastPeriodDayCounter { get; set; } = DayCounter.Actual360LD;
        public bool RebatesAccrual { get; set; } = true;

        /// <summary>
        /// Valid values are Midpoint and ISDA
        /// </summary>
        public string PricingModel { get; set; } = "Midpoint";

        /// <summary>
        /// Contract spread on CDS (typically 100bps or 500bps)
        /// </summary>
        public double? CdsQuoteRunningSpread { get; set; }

        internal override global::QuantLib.DefaultProbabilityTermStructure Build()
        {
            var termStructureDayCounter = DayCountBasis.ToQlDayCounter();
            if (CdsQuotes == null || !CdsQuotes.Any())
                throw new ArgumentNullException(nameof(CdsQuotes));
            var helpers = CdsQuotes.Select(cq => GetDefaultProbabilityHelper(cq.tenor, cq.type, cq.quoteValue));
            var defProbHelpers = new global::QuantLib.DefaultProbabilityHelperVector(helpers);
            return new global::QuantLib.PiecewiseLogLinearSurvival(TradeDate, defProbHelpers, termStructureDayCounter);
        }

        internal global::QuantLib.DefaultProbabilityHelper GetDefaultProbabilityHelper(Period tenor, CdsQuoteType type, double quoteValue)
        {
            if (CdsQuoteCalendar == CalendarName.Unknown)
                throw new ArgumentNullException(nameof(CdsQuoteCalendar));
            if (!CdsQuoteRecoveryRate.HasValue)
                CdsQuoteRecoveryRate = 0.4;
            if (DiscountCurve == null)
                throw new ArgumentNullException(nameof(DiscountCurve));
            global::QuantLib.CreditDefaultSwap.PricingModel pricingModel =
                PricingModel == "ISDA" ? global::QuantLib.CreditDefaultSwap.PricingModel.ISDA : global::QuantLib.CreditDefaultSwap.PricingModel.Midpoint;
            switch (type)
            {
                case CdsQuoteType.Upfront:
                    if (!CdsQuoteRunningSpread.HasValue)
                        throw new ArgumentNullException(nameof(CdsQuoteRunningSpread));
                    return new global::QuantLib.UpfrontCdsHelper(quoteValue, CdsQuoteRunningSpread.Value, tenor.QlObj, SettlementDays, CdsQuoteCalendar.ToQlCalendar(),
                        CdsQuoteFrequency.ToQlFrequency(), CdsQuoteRollConvention.ToQlConvention(), CdsQuoteDateGenerationRule.ToQlDateGeneration(), CdsQuoteBasis.ToQlDayCounter(),
                        CdsQuoteRecoveryRate.Value, DiscountCurve.GetHandle(), Convert.ToUInt32(UpfrontSettlementDays), SettlesAccrual, PaysAtDefaultTime,
                        StartDate ?? new global::QuantLib.Date(), LastPeriodDayCounter.ToQlDayCounter(), RebatesAccrual, pricingModel);
                case CdsQuoteType.Spread:
                    return new global::QuantLib.SpreadCdsHelper(quoteValue, tenor.QlObj, SettlementDays, CdsQuoteCalendar.ToQlCalendar(), CdsQuoteFrequency.ToQlFrequency(),
                        CdsQuoteRollConvention.ToQlConvention(), CdsQuoteDateGenerationRule.ToQlDateGeneration(), CdsQuoteBasis.ToQlDayCounter(), CdsQuoteRecoveryRate.Value,
                        DiscountCurve.GetHandle(), SettlesAccrual, PaysAtDefaultTime, StartDate ?? new global::QuantLib.Date(), LastPeriodDayCounter.ToQlDayCounter(),
                        RebatesAccrual, pricingModel);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}