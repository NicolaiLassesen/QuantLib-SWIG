using System;

namespace QuantLib
{
    public static class ForeignExchangeForwardExtension
    {
        public static string Print(this ForeignExchangeForward fxFwd)
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