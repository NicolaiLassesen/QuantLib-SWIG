using System;
using System.Collections.Generic;
using System.Linq;
using QlDate = QuantLib.Date;
using QlCalendar = QuantLib.Calendar;
using QlJointCalendar = QuantLib.JointCalendar;
using QlDayCounter = QuantLib.DayCounter;
using QlFwdXRate = QuantLib.ForwardExchangeRate;
using QlFwdXRateVector = QuantLib.ForwardExchangeRateVector;
using QlFwdPntTs = QuantLib.FxForwardPointTermStructure;
using QlFwdPntTsHandle = QuantLib.FxForwardPointTermStructureHandle;
// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib.TermStructures
{
    public class FxForwardPointTermStructure : TermStructure<QlFwdPntTs>
    {
        public class Builder
        {
            private ExchangeRate _spotExchangeRate;

            public Builder(DateTime tradeDate, Currency baseCurrency, Currency quoteCurrency)
            {
                TradeDate = tradeDate;
                BaseCurrency = baseCurrency;
                QuoteCurrency = quoteCurrency;
                ForwardPoints = new List<(Period tenor, double forwardPoints)>();
            }

            public Builder(DateTime tradeDate, ExchangeRate spotExchangeRate)
            : this(tradeDate, spotExchangeRate.BaseCurrency, spotExchangeRate.QuoteCurrency)
            {
                _spotExchangeRate = spotExchangeRate;
            }

            public DateTime TradeDate { get; }
            public int SpotDays { get; set; }

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

            public ICollection<(Period Tenor, double ForwardPoints)> ForwardPoints { get; set; }

            internal QlFwdPntTs Build()
            {
                if (_spotExchangeRate == null)
                    throw new ArgumentNullException(nameof(SpotExchangeRate));
                if (ForwardPoints == null || !ForwardPoints.Any())
                    throw new ArgumentException("No forward points observations were provided");

                QlCalendar calendar = new QlJointCalendar(BaseCalendar.ToQlCalendar(), QuoteCalendar.ToQlCalendar());
                QlDayCounter dayCounter = DayCounter.ToQlDayCounter();
                QlDate spotDate = calendar.advance(TradeDate, SpotDays, global::QuantLib.TimeUnit.Days);

                if (_spotExchangeRate.BaseCurrency != BaseCurrency)
                    _spotExchangeRate = _spotExchangeRate.Inverse();

                var qlFwdRates = ForwardPoints.OrderBy(fp => fp.Tenor)
                                              .Select(fp => new QlFwdXRate(_spotExchangeRate.QlObj, fp.ForwardPoints, fp.Tenor.QlObj));
                var fwdExchRates = new QlFwdXRateVector(qlFwdRates);
                return new global::QuantLib.InterpolatedFxForwardPointTermStructureLinear(spotDate, fwdExchRates, dayCounter, calendar);
            }
        }

        public FxForwardPointTermStructure(Builder builder)
            : base(builder.Build())
        {
        }

        internal QlFwdPntTsHandle GetHandle()
        {
            return new QlFwdPntTsHandle(QlObj);
        }

        #region Overrides of TermStructure

        public override string ToString()
        {
            return $"{nameof(FxForwardPointTermStructure)} {QlObj.referenceDate().ISO()}";
        }

        #endregion
    }
}