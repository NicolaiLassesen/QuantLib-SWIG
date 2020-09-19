using System;
using CfAnalytics.QuantLib.InternalUtils;

namespace CfAnalytics.QuantLib
{
    public class RateHelper
    {
        internal RateHelper(global::QuantLib.RateHelper qlObj)
        {
            QlObj = qlObj;
        }

        internal global::QuantLib.RateHelper QlObj { get; }

        public DateTime EarliestDate => QlObj.earliestDate().AsDateTime();
        public DateTime LatestDate => QlObj.latestDate().AsDateTime();
        public DateTime LastesRelevantDate => QlObj.latestRelevantDate().AsDateTime();
        public DateTime MaturityDate => QlObj.maturityDate().AsDateTime();
        public DateTime PillarDate => QlObj.pillarDate().AsDateTime();

        public bool HasQuote => !QlObj.quote().empty();
        public bool HasValidQuote => QlObj.quote().isValid();
        public double QuoteValue => QlObj.quote().value();
        public double ImpliedQuote => QlObj.impliedQuote();
        public double QuoteError => QlObj.quoteError();
    }
}