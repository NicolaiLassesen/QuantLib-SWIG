using System;
using CfAnalytics.QuantLib.InternalUtils;
using QlRate = QuantLib.InterestRate;

namespace CfAnalytics.QuantLib
{
    public readonly struct InterestRate : IDisposable
    {
        internal QlRate QlObj { get; }

        internal InterestRate(QlRate qlObj)
        {
            QlObj = qlObj;
        }

        public InterestRate(double rate, DayCounter basis, Compounding compounding, Frequency frequency)
            : this(new QlRate(rate, basis.ToQlDayCounter(), compounding.ToQlCompounding(), frequency.ToQlFrequency()))
        {
        }

        public double Rate => QlObj.rate();

        public Compounding Compounding => QlObj.compounding().ToCfCompounding();

        public Frequency Frequency => QlObj.frequency().ToCfFrequency();

        //public DayCounter Basis =>QlObj.dayCounter().

        public double CompoundFactor(double time) => QlObj.compoundFactor(time);

        public double DiscountFactor(double time) => QlObj.discountFactor(time);

        public InterestRate EquivalentRate(Compounding compounding, Frequency frequency, double time) =>
            new InterestRate(QlObj.equivalentRate(compounding.ToQlCompounding(), frequency.ToQlFrequency(), time));

        #region IDisposable

        public void Dispose()
        {
            QlObj?.Dispose();
        }

        #endregion

        #region Overrides of ValueType

        public override string ToString() => QlObj.__str__();

        #endregion
    }
}