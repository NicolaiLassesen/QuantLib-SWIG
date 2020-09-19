using CfAnalytics.QuantLib.TermStructures;
using QlIsdaCdsEngine = QuantLib.IsdaCdsEngine;

namespace CfAnalytics.QuantLib.PricingEngines.Credit
{
    public class IsdaCdsEngine : PricingEngine<QlIsdaCdsEngine>
    {
        public IsdaCdsEngine(DefaultProbabilityTermStructure creditCurve, double recoveryRate, YieldTermStructure discountCurve)
            : base(new QlIsdaCdsEngine(creditCurve.GetHandle(), recoveryRate, discountCurve.GetHandle()))
        {
        }
    }
}