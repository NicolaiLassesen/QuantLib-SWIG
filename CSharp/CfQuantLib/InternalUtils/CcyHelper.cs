using System;
using QuantLib;
using Ccy = QuantLib.Currency;
// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib.InternalUtils
{
    internal static class CcyHelper
    {
        internal static Ccy Convert(Currency ccy)
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

        internal static readonly Ccy EUR = new EURCurrency();
        internal static readonly Ccy USD = new USDCurrency();
        internal static readonly Ccy GBP = new GBPCurrency();
        internal static readonly Ccy CHF = new CHFCurrency();
    }
}