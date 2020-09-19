using System;
using QlQuote = QuantLib.SimpleQuote;
using QlQuoteHandle = QuantLib.QuoteHandle;
using FlatHazardRate = QuantLib.FlatHazardRate;

namespace CfAnalytics.QuantLib.TermStructures.DefaultProbabilityTermStructures
{
    public class FlatHazardRateBuilder : DefaultProbabilityTermStructure.BuilderBase
    {
        public FlatHazardRateBuilder(DateTime tradeDate)
            : base(tradeDate)
        {
        }

        public double FlatHazardRate { get; set; }

        internal override global::QuantLib.DefaultProbabilityTermStructure Build()
        {
            var termStructureDayCounter = DayCountBasis.ToQlDayCounter();
            var quoteHandle = new QlQuoteHandle(new QlQuote(FlatHazardRate));
            return new FlatHazardRate(TradeDate, quoteHandle, termStructureDayCounter);
        }
    }
}