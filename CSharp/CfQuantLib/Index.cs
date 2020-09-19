using System;
using System.Collections.Generic;
using System.Linq;
using CfAnalytics.QuantLib.InternalUtils;
using CfAnalytics.QuantLib.TermStructures;
using JetBrains.Annotations;
using QlIndex = QuantLib.Index;
using QlIrIdx = QuantLib.InterestRateIndex;
using QlIborIdx = QuantLib.IborIndex;

namespace CfAnalytics.QuantLib
{
    public abstract class Index : IDisposable
    {
        public abstract class IndexImpl : IDisposable
        {
            public abstract bool IsValidFixingDate(DateTime date);
            public abstract void ClearFixings();
            public abstract void AddFixing(DateTime fixingDate, double fixingValue, bool? forceOverwrite = null);
            public abstract void AddFixings(Dictionary<DateTime, double> fixings, bool? forceOverwrite = null);
            public abstract double GetFixing(DateTime fixingDate, bool? forecastTodaysFixing = null);
            public abstract Dictionary<DateTime, double> GetFixings();

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected abstract void Dispose(bool disposing);
        }

        public class IndexImpl<TWrapped> : IndexImpl where TWrapped : QlIndex
        {
            internal IndexImpl([NotNull] TWrapped qlObj)
            {
                QlObj = qlObj ?? throw new ArgumentNullException(nameof(qlObj));
            }

            internal TWrapped QlObj { get; }

            public override bool IsValidFixingDate(DateTime date)
            {
                return QlObj.isValidFixingDate(date);
            }

            public override void ClearFixings()
            {
                QlObj.clearFixings();
            }

            public override void AddFixing(DateTime fixingDate, double fixingValue, bool? forceOverwrite = null)
            {
                if (forceOverwrite.HasValue)
                    QlObj.addFixing(fixingDate, fixingValue, forceOverwrite.Value);
                else
                    QlObj.addFixing(fixingDate, fixingValue);
            }

            public override void AddFixings(Dictionary<DateTime, double> fixings, bool? forceOverwrite = null)
            {
                var dates = new global::QuantLib.DateVector(fixings.Keys);
                var values = new global::QuantLib.DoubleVector(fixings.Values);
                if (forceOverwrite.HasValue)
                    QlObj.addFixings(dates, values, forceOverwrite.Value);
                else
                    QlObj.addFixings(dates, values);
            }

            public override double GetFixing(DateTime fixingDate, bool? forecastTodaysFixing = null)
            {
                return forecastTodaysFixing.HasValue ? QlObj.fixing(fixingDate, forecastTodaysFixing.Value) : QlObj.fixing(fixingDate);
            }

            public override Dictionary<DateTime, double> GetFixings()
            {
                var ts = QlObj.timeSeries();
                return Enumerable.Range(0, Convert.ToInt32(ts.size())).ToDictionary(i => ts.dates()[i].AsDateTime(), i => ts.values()[i]);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    QlObj.Dispose();
                }
            }
        }

        protected Index(IndexImpl impl)
        {
            Impl = impl;
        }

        internal IndexImpl Impl { get; }

        public bool IsValidFixingDate(DateTime date) => Impl.IsValidFixingDate(date);
        public void ClearFixings() => Impl.ClearFixings();
        public void AddFixing(DateTime fixingDate, double fixingValue, bool? forceOverwrite = null) => Impl.AddFixing(fixingDate, fixingValue, forceOverwrite);
        public void AddFixings(Dictionary<DateTime, double> fixings, bool? forceOverwrite = null) => Impl.AddFixings(fixings, forceOverwrite);
        public double GetFixing(DateTime fixingDate, bool? forecastTodaysFixing = null) => Impl.GetFixing(fixingDate, forecastTodaysFixing);
        public Dictionary<DateTime, double> GetFixings() => Impl.GetFixings();

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Impl.Dispose();
        }

        #endregion
    }

    public class Index<TImpl> : Index where TImpl : Index.IndexImpl
    {
        public Index(TImpl impl)
            : base(impl)
        {
            Impl = impl;
        }

        internal new TImpl Impl { get; }
    }

    public class IrIndex : Index<IrIndex.IrIndexImpl>
    {
        public class IrIndexImpl : IndexImpl<QlIrIdx>
        {
            internal IrIndexImpl([NotNull] QlIrIdx qlObj)
                : base(qlObj)
            {
            }

            public Currency Currency => QlObj.currency().ToCfCurrency();
            public string Family => QlObj.familyName();
            public Period Tenor => new Period(QlObj.tenor());
        }

        internal IrIndex(QlIrIdx qlObj)
            : base(new IrIndexImpl(qlObj))
        {
        }

        public Currency Currency => Impl.Currency;
        public string Family => Impl.Family;
        public Period Tenor => Impl.Tenor;

        public override string ToString() => $"{Currency}-{Family}-{Tenor}".ToUpper();
    }

    public class IborIndex : IrIndex
    {
        internal IborIndex([NotNull] QlIborIdx qlObj)
            : base(qlObj)
        {
        }

        public static IborIndex GetIborIndex(Currency currency, string indexFamily, string tenor, YieldTermStructure forwardingCurve = null)
        {
            return IborIndexHelper.GetIborIndex($"{currency}-{indexFamily}-{tenor}".ToUpper(), forwardingCurve);
        }
    }
}