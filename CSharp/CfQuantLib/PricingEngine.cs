using QlEngine = QuantLib.PricingEngine;
// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib
{
    public abstract class PricingEngine
    {
        internal abstract QlEngine GetQlEngine();
    }

    public class PricingEngine<TWrapped> : PricingEngine where TWrapped : QlEngine
    {
        internal TWrapped QlObj { get; }

        internal PricingEngine(TWrapped qlObj)
        {
            QlObj = qlObj;
        }

        #region Overrides of PricingEngine

        internal override QlEngine GetQlEngine() => QlObj;

        #endregion
    }
}