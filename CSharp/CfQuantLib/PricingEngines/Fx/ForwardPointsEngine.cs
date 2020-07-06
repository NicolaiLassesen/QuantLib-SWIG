using CfAnalytics.QuantLib.TermStructures;
using QlFwdPtsEngine = QuantLib.ForwardPointsEngine;

namespace CfAnalytics.QuantLib.PricingEngines.Fx
{
    public class ForwardPointsEngine : PricingEngine<QlFwdPtsEngine>
    {
        public ForwardPointsEngine(ExchangeRate spotExchangeRate, FxForwardPointTermStructure forwardPointsCurve)
            : base(new QlFwdPtsEngine(spotExchangeRate.QlObj, forwardPointsCurve.QlObj, null, null))
        {
        }
    }
}