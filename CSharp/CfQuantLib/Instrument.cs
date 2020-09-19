using System;
using CfAnalytics.QuantLib.InternalUtils;
using QlInstrument = QuantLib.Instrument;

namespace CfAnalytics.QuantLib
{
    public abstract class Instrument : IDisposable
    {
        public PricingEngine PricingEngine
        {
            set => SetPricingEngine(value);
        }

        public abstract DateTime ValuationDate { get; }
        public abstract double Npv { get; }
        public abstract double ErrorEstimate { get; }
        public abstract bool IsExpired { get; }

        public abstract void Recalculate();

        public abstract override string ToString();

        protected abstract void SetPricingEngine(PricingEngine engine);

        #region IDisposable

        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    public class Instrument<TImpl> : Instrument where TImpl : class, IInstrumentImpl
    {
        internal Instrument(TImpl impl)
        {
            Impl = impl;
        }

        protected TImpl Impl { get; }

        #region Overrides of Instrument

        public override DateTime ValuationDate => Impl.ValuationDate;
        public override double Npv => Impl.Npv;
        public override double ErrorEstimate => Impl.ErrorEstimate;
        public override bool IsExpired => Impl.IsExpired;

        protected override void SetPricingEngine(PricingEngine engine)
        {
            Impl.SetPricingEngine(engine);
        }

        public override void Recalculate()
        {
            Impl.Recalculate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Impl.Dispose();
            }
        }

        public override string ToString() => Impl.ToString();

        #endregion
    }

    public interface IInstrumentImpl : IDisposable
    {
        DateTime ValuationDate { get; }
        double Npv { get; }
        double ErrorEstimate { get; }
        bool IsExpired { get; }

        void Recalculate();
        void SetPricingEngine(PricingEngine engine);
    }

    public abstract class InstrumentImpl<TWrapped> : IInstrumentImpl where TWrapped : QlInstrument
    {
        internal TWrapped QlObj { get; }

        internal InstrumentImpl(TWrapped qlObj)
        {
            QlObj = qlObj;
        }

        public DateTime ValuationDate => QlObj.valuationDate().AsDateTime();
        public double Npv => QlObj.NPV();
        public double ErrorEstimate => QlObj.errorEstimate();
        public bool IsExpired => QlObj.isExpired();

        public void Recalculate()
        {
            QlObj.recalculate();
        }

        public void SetPricingEngine(PricingEngine engine)
        {
            QlObj.setPricingEngine(engine.GetQlEngine());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                QlObj.Dispose();
            }
        }

        public override string ToString() => QlObj.ToString();
    }
}