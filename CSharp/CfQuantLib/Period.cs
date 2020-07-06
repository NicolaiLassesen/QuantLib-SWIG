using System;
using CfAnalytics.QuantLib.InternalUtils;
using QlPeriod = QuantLib.Period;

namespace CfAnalytics.QuantLib
{
    public readonly struct Period : IEquatable<Period>
    {
        internal QlPeriod QlObj { get; }

        internal Period(QlPeriod qlObj)
        {
            QlObj = qlObj;
        }

        public Period(int length, TimeUnit units)
            : this(new QlPeriod(length, units.ToQlTimeUnit()))
        {
        }

        public int Length => QlObj.length();
        public TimeUnit Units => QlObj.units().ToTimeUnit();

        #region Equality members

        public bool Equals(Period other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Length == other.Length && Units == other.Units;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Period)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Length * 397) ^ (int)Units;
            }
        }

        public static bool operator ==(Period left, Period right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Period left, Period right)
        {
            return !Equals(left, right);
        }

        #endregion

        public override string ToString() => QlObj.__str__();
    }
}