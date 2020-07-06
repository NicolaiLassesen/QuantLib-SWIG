using System;
using QlFwdXrate = QuantLib.ForwardExchangeRate;

namespace CfAnalytics.QuantLib
{
    public readonly struct ForwardExchangeRate : IEquatable<ForwardExchangeRate>
    {
        internal QlFwdXrate QlObj { get; }

        internal ForwardExchangeRate(QlFwdXrate qlObj)
        {
            QlObj = qlObj;
        }

        public ForwardExchangeRate(ExchangeRate spotExchangeRate, double forwardpoints, Period tenor)
            : this(new QlFwdXrate(spotExchangeRate.QlObj, forwardpoints, tenor.QlObj))
        {
        }

        public ExchangeRate SpotExchangeRate => new ExchangeRate(QlObj.spotExchangeRate());
        public double SpotRate => QlObj.spotRate();
        public double ForwardPoints => QlObj.forwardPoints();
        public double Forwardrate => QlObj.forwardRate();
        public Period Tenor => new Period(QlObj.tenor());

        #region Equality members

        public bool Equals(ForwardExchangeRate other)
        {
            return SpotExchangeRate.Equals(other.SpotExchangeRate) && ForwardPoints.Equals(other.ForwardPoints) && Tenor.Equals(other.Tenor);
        }

        public override bool Equals(object obj)
        {
            return obj is ForwardExchangeRate other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = SpotExchangeRate.GetHashCode();
                hashCode = (hashCode * 397) ^ ForwardPoints.GetHashCode();
                hashCode = (hashCode * 397) ^ Tenor.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(ForwardExchangeRate left, ForwardExchangeRate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ForwardExchangeRate left, ForwardExchangeRate right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}