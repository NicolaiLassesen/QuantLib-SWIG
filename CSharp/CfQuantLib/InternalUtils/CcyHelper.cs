using System;
using CfAnalytics.Utilities;
using QlCcy = QuantLib.Currency;
// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib.InternalUtils
{
    internal static class CcyHelper
    {
        internal static QlCcy ToQlCurrency(this Currency ccy)
        {
            switch (ccy)
            {
                case Currency.EUR: return EUR;
                case Currency.USD: return USD;
                case Currency.GBP: return GBP;
                case Currency.CHF: return CHF;
                case Currency.JPY: return JPY;
                case Currency.DKK: return DKK;
                case Currency.NOK: return NOK;
                case Currency.SEK: return SEK;
                case Currency.CAD: return CAD;
                case Currency.PLN: return PLN;
                case Currency.NZD: return NZD;
                case Currency.AUD: return AUD;
                default: throw new ArgumentOutOfRangeException(nameof(ccy), ccy, "Unmapped Currency");
            }
        }

        internal static Currency ToCfCurrency(this QlCcy ccy)
        {
            return EnumUtils.GetCurrency(ccy.code());
        }

        internal static readonly QlCcy EUR = new global::QuantLib.EURCurrency();
        internal static readonly QlCcy USD = new global::QuantLib.USDCurrency();
        internal static readonly QlCcy GBP = new global::QuantLib.GBPCurrency();
        internal static readonly QlCcy CHF = new global::QuantLib.CHFCurrency();
        internal static readonly QlCcy JPY = new global::QuantLib.JPYCurrency();
        internal static readonly QlCcy DKK = new global::QuantLib.DKKCurrency();
        internal static readonly QlCcy NOK = new global::QuantLib.NOKCurrency();
        internal static readonly QlCcy SEK = new global::QuantLib.SEKCurrency();
        internal static readonly QlCcy CAD = new global::QuantLib.CADCurrency();
        internal static readonly QlCcy PLN = new global::QuantLib.PLNCurrency();
        internal static readonly QlCcy NZD = new global::QuantLib.NZDCurrency();
        internal static readonly QlCcy AUD = new global::QuantLib.AUDCurrency();
    }
}