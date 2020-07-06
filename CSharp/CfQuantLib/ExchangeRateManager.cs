using System;
using CfAnalytics.QuantLib.InternalUtils;
using QlErm = QuantLib.ExchangeRateManager;
using QlMoney = QuantLib.Money;
// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib
{
    public static class ExchangeRateManager
    {
        public enum ConversionType
        {
            NoConversion,
            BaseCurrencyConversion,
            AutomatedConversion
        }

        public static ConversionType MoneyConversion
        {
            set
            {
                switch (value)
                {
                    case ConversionType.NoConversion:
                        QlMoney.setConversionType(QlMoney.ConversionType.NoConversion);
                        break;
                    case ConversionType.BaseCurrencyConversion:
                        QlMoney.setConversionType(QlMoney.ConversionType.BaseCurrencyConversion);
                        break;
                    case ConversionType.AutomatedConversion:
                        QlMoney.setConversionType(QlMoney.ConversionType.AutomatedConversion);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        public static Currency BaseCurrency
        {
            set
            {
                var ccy = CcyHelper.Convert(value);
                QlMoney.setBaseCurrency(ccy);
            }
        }

        public static void Add(Currency baseCurrency, Currency quoteCurrency, double rate, DateTime date)
        {
            Add(new ExchangeRate(baseCurrency, quoteCurrency, rate), date);
        }

        public static void Add(ExchangeRate exchangeRate, DateTime date)
        {
            QlErm.instance().add(exchangeRate.QlObj, date);
        }

        public static ExchangeRate LookUp(Currency baseCurrency, Currency quoteCurrency, DateTime date)
        {
            return new ExchangeRate(QlErm.instance().lookup(CcyHelper.Convert(baseCurrency), CcyHelper.Convert(quoteCurrency), date));
        }
    }
}