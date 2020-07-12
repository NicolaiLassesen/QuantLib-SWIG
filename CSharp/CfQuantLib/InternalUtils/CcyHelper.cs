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
                case Currency.EUR:
                    return EUR;
                case Currency.USD:
                    return USD;
                case Currency.GBP:
                    return GBP;
                case Currency.CHF:
                    return CHF;
                case Currency.JPY:
                    return JPY;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ccy), ccy, "Unmapped Currency");
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
    }
}