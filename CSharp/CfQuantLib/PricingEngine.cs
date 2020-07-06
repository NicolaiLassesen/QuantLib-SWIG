namespace CfAnalytics.QuantLib
{
    public class PricingEngine
    {
    }

    public class PricingEngine<TWrapped> : PricingEngine
    {
        internal TWrapped QlObj { get; }

        internal PricingEngine(TWrapped qlObj)
        {
            QlObj = qlObj;
        }
    }
}