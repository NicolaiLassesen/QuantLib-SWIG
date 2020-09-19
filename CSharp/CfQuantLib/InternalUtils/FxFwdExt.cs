namespace CfAnalytics.QuantLib.InternalUtils
{
    internal static class FxFwdExt
    {
        internal static string Print(this global::QuantLib.ForeignExchangeForward fxFwd)
        {
            var baseCcy = fxFwd.baseCurrency();
            var termCcy = fxFwd.termCurrency();
            var deliveryDate = fxFwd.deliveryDate();
            var contractNotionalBase = fxFwd.contractNotionalAmountBase();
            string results = $"FxFwd, {baseCcy}{termCcy} - {deliveryDate} - {contractNotionalBase}";
            return results;
        }
    }
}