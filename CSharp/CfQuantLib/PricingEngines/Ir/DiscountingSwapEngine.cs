using CfAnalytics.QuantLib.TermStructures;
using QlDiscountingSwapEngine = QuantLib.DiscountingSwapEngine;

namespace CfAnalytics.QuantLib.PricingEngines.Ir
{
    public class DiscountingSwapEngine:PricingEngine<QlDiscountingSwapEngine>
    {
        public DiscountingSwapEngine(YieldTermStructure discountCurve)
            : base(new QlDiscountingSwapEngine(discountCurve.GetHandle()))
        {
        }
    }
}