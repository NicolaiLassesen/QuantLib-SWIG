using QlInstrument= QuantLib.Instrument;
// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib
{
    public abstract class Instrument
    {
        public PricingEngine PricingEngine
        {
            set => SetPricingEngine(value);
        }

        protected abstract void SetPricingEngine(PricingEngine engine);
    }

    public class Instrument<TWrapped> : Instrument where TWrapped : QlInstrument
    {
        internal TWrapped QlObj { get; }

        internal Instrument(TWrapped qlObj)
        {
            QlObj = qlObj;
        }

        #region Overrides of Instrument

        protected override void SetPricingEngine(PricingEngine engine)
        {
            QlObj.setPricingEngine(engine.GetQlEngine());
        }

        #endregion
    }
}