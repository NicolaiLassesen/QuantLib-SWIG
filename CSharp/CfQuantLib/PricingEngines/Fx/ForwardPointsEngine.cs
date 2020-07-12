using CfAnalytics.QuantLib.TermStructures;
using QlFwdPtsEngine = QuantLib.ForwardPointsEngine;

namespace CfAnalytics.QuantLib.PricingEngines.Fx
{
    public class ForwardPointsEngine : PricingEngine<QlFwdPtsEngine>
    {
        public ForwardPointsEngine(ExchangeRate spotExchangeRate, FxForwardPointTermStructure forwardPointsCurve,
                                   YieldTermStructure baseDiscountCurve, YieldTermStructure quoteDiscountCurve)
            : base(new QlFwdPtsEngine(spotExchangeRate.QlObj, forwardPointsCurve.GetHandle(), baseDiscountCurve.GetHandle(), quoteDiscountCurve.GetHandle()))
        {
        }
    }
}