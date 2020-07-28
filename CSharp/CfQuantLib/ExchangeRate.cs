using System;
using CfAnalytics.QuantLib.InternalUtils;
using QlEr = QuantLib.ExchangeRate;
// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib
{
    public readonly struct ExchangeRate : IEquatable<ExchangeRate>
    {
        public enum Type
        {
            Direct,
            Derived
        }

        internal QlEr QlObj { get; }

        internal ExchangeRate(QlEr qlObj)
        {
            QlObj = qlObj;
        }

        public ExchangeRate(Currency baseCurrency, Currency quoteCurrency, decimal rate)
            : this(baseCurrency, quoteCurrency, Convert.ToDouble(rate))
        {
        }

        public ExchangeRate(Currency baseCurrency, Currency quoteCurrency, double rate)
            : this(new QlEr(baseCurrency.ToQlCurrency(), quoteCurrency.ToQlCurrency(), rate))
        {
        }

        public Currency BaseCurrency => QlObj.source().ToCfCurrency();
        public Currency QuoteCurrency => QlObj.target().ToCfCurrency();
        public double Rate => QlObj.rate();

        public Type ExchangeRateType
        {
            get
            {
                switch (QlObj.type())
                {
                    case QlEr.Type.Direct:
                        return Type.Direct;
                    case QlEr.Type.Derived:
                        return Type.Derived;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Money Exchange(Money money)
        {
            return new Money(QlObj.exchange(money.QlObj));
        }

        public ExchangeRate Inverse()
        {
            return new ExchangeRate(QlEr.inverse(QlObj));
        }

        public override string ToString()
        {
            return QlObj.ToString();
        }

        #region Equality members

        public bool Equals(ExchangeRate other)
        {
            return BaseCurrency == other.BaseCurrency && QuoteCurrency == other.QuoteCurrency && Rate.Equals(other.Rate);
        }

        public override bool Equals(object obj)
        {
            return obj is ExchangeRate other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)BaseCurrency;
                hashCode = (hashCode * 397) ^ (int)QuoteCurrency;
                hashCode = (hashCode * 397) ^ Rate.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(ExchangeRate left, ExchangeRate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ExchangeRate left, ExchangeRate right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}