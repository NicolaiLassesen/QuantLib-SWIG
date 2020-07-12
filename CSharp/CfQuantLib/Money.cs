using System;
using CfAnalytics.QuantLib.InternalUtils;
using QlMoney = QuantLib.Money;

namespace CfAnalytics.QuantLib
{
    public readonly struct Money : IEquatable<Money>
    {
        internal QlMoney QlObj { get; }

        internal Money(QlMoney qlObj)
        {
            QlObj = qlObj;
        }

        public Money(double value, Currency currency)
            : this(new QlMoney(value, currency.ToQlCurrency()))
        {
        }

        public double Value => QlObj.value();
        public Currency Currency => QlObj.currency().ToCfCurrency();

        public override string ToString()
        {
            return QlObj.ToString();
        }

        #region Equality members

        public bool Equals(Money other)
        {
            return Value.Equals(other.Value) && Currency == other.Currency;
        }

        public override bool Equals(object obj)
        {
            return obj is Money other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Value.GetHashCode() * 397) ^ (int)Currency;
            }
        }

        public static bool operator ==(Money left, Money right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Money left, Money right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}