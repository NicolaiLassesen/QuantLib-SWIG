using System;
using System.Collections.Generic;
using System.Linq;
using QlFwdPntTsHandle = QuantLib.FxForwardPointTermStructureHandle;
using QlCalendar = QuantLib.Calendar;
using QlJointCalendar = QuantLib.JointCalendar;
using QlDayCounter = QuantLib.DayCounter;
using QlFwdXRate = QuantLib.ForwardExchangeRate;
using QlFwdXRateVector = QuantLib.ForwardExchangeRateVector;
// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib.TermStructures
{
    public class FxForwardPointTermStructure : TermStructure<QlFwdPntTsHandle>
    {
        public class Builder
        {
            private ExchangeRate _spotExchangeRate;

            public Builder(DateTime referenceDate, Currency baseCurrency, Currency quoteCurrency)
            {
                ReferenceDate = referenceDate;
                BaseCurrency = baseCurrency;
                QuoteCurrency = quoteCurrency;
            }

            public Builder(DateTime referenceDate, ExchangeRate spotExchangeRate)
            {
                ReferenceDate = referenceDate;
                BaseCurrency = spotExchangeRate.BaseCurrency;
                QuoteCurrency = spotExchangeRate.QuoteCurrency;
                _spotExchangeRate = spotExchangeRate;
            }

            public DateTime ReferenceDate { get; }

            public Currency BaseCurrency { get; }
            public Currency QuoteCurrency { get; }

            public Calendar BaseCalendar { get; set; }
            public Calendar QuoteCalendar { get; set; }
            public DayCounter DayCounter { get; set; }

            public double SpotExchangeRate
            {
                get => _spotExchangeRate.Rate;
                set => _spotExchangeRate = new ExchangeRate(QuoteCurrency, BaseCurrency, value);
            }

            public ICollection<(Period tenor, double forwardPoints)> ForwardPoints { get; set; }

            internal QlFwdPntTsHandle Build()
            {
                if (_spotExchangeRate == null)
                    throw new ArgumentNullException(nameof(SpotExchangeRate));
                if (ForwardPoints == null || !ForwardPoints.Any())
                    throw new ArgumentException("No forward points observations were provided");

                QlCalendar calendar = new QlJointCalendar(BaseCalendar.ToQlCalendar(), QuoteCalendar.ToQlCalendar());
                QlDayCounter dayCounter = DayCounter.ToQlDayCounter();

                if (_spotExchangeRate.BaseCurrency != BaseCurrency)
                    _spotExchangeRate = _spotExchangeRate.Inverse();

                var fwdExchRates = new QlFwdXRateVector(ForwardPoints.Select(fp => new QlFwdXRate(_spotExchangeRate.QlObj, fp.forwardPoints, fp.tenor.QlObj)));
                var fwdPtCurve = new global::QuantLib.InterpolatedFxForwardPointTermStructureLinear(ReferenceDate, fwdExchRates, dayCounter, calendar);
                return new QlFwdPntTsHandle(fwdPtCurve);
            }
        }

        public FxForwardPointTermStructure(Builder builder)
            : base(builder.Build())
        {
        }
    }
}