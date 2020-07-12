using System;
using CfAnalytics.Utilities;
using QuantLib;
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(ccy), ccy, "Unmapped Currency");
            }
        }

        internal static Currency ToCfCurrency(this QlCcy ccy)
        {
            return EnumUtils.GetCurrency(ccy.code());
        }

        internal static readonly QlCcy EUR = new EURCurrency();
        internal static readonly QlCcy USD = new USDCurrency();
        internal static readonly QlCcy GBP = new GBPCurrency();
        internal static readonly QlCcy CHF = new CHFCurrency();
    }
}