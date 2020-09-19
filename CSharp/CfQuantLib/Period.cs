using System;
using CfAnalytics.QuantLib.InternalUtils;
using QlPeriod = QuantLib.Period;

namespace CfAnalytics.QuantLib
{
    public readonly struct Period : IEquatable<Period>, IComparable<Period>, IComparable, IDisposable
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

        public Period(string periodString)
            : this(new QlPeriod(periodString))
        {
        }

        public int Length => QlObj.length();
        public TimeUnit Units => QlObj.units().ToTimeUnit();

        public Utilities.Period AsCfPeriod()
        {
            return new Utilities.Period(Length, Units);
        }

        #region Equality members

        public bool Equals(Period other)
        {
            return Length == other.Length && Units == other.Units;
        }

        public override bool Equals(object obj)
        {
            return obj is Period other && Equals(other);
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

        #region Relational members

        public int CompareTo(Period other)
        {
            // TODO: Logic copied from QL - expose actual QL logic via SWIG instead

            // special cases
            if (Length == 0)
                return other.Length.CompareTo(0);
            if (other.Length == 0)
                return Length.CompareTo(0);

            // exact comparisons
            if (Units == other.Units)
                return Length.CompareTo(other.Length);
            if (Units == TimeUnit.Months && other.Units == TimeUnit.Years)
                return Length.CompareTo(other.Length * 12);
            if (Units == TimeUnit.Years && other.Units == TimeUnit.Months)
                return (Length * 12).CompareTo(other.Length);
            if (Units == TimeUnit.Days && other.Units == TimeUnit.Weeks)
                return Length.CompareTo(other.Length * 7);
            if (Units == TimeUnit.Weeks && other.Units == TimeUnit.Days)
                return (Length * 7).CompareTo(other.Length);

            // inexact comparisons (handled by converting to days and using limits)
            var thisLim = DaysMinMax(this);
            var otherLim = DaysMinMax(other);

            if (thisLim.Max < otherLim.Min)
                return -1;
            if (thisLim.Min > otherLim.Max)
                return 1;

            throw new ArgumentException($"Undecidable comparison between Period objects {this} and {other}");
        }

        public int CompareTo(object obj)
        {
            if (obj is null) return 1;
            return obj is Period other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Period)}");
        }

        public static bool operator <(Period left, Period right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(Period left, Period right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(Period left, Period right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(Period left, Period right)
        {
            return left.CompareTo(right) >= 0;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            QlObj?.Dispose();
        }

        #endregion

        public override string ToString() => QlObj.__str__();

        #region Private helpers

        private static (int Min, int Max) DaysMinMax(Period p)
        {
            switch (p.Units)
            {
                case TimeUnit.Days:
                    return (p.Length, p.Length);
                case TimeUnit.Weeks:
                    return (7 * p.Length, 7 * p.Length);
                case TimeUnit.Months:
                    return (28 * p.Length, 31 * p.Length);
                case TimeUnit.Years:
                    return (365 * p.Length, 366 * p.Length);
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(TimeUnit)} cannot be converted to days interval ({p.Units})");
            }
        }

        #endregion
    }
}