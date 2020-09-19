using System;
using QlTs = QuantLib.TermStructure;

namespace CfAnalytics.QuantLib
{
    public abstract class TermStructure : IDisposable
    {
        public abstract bool AllowsExtrapolation { get; }

        public abstract override string ToString();

        #region IDisposable

        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
           Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
    
    public abstract class TermStructure<TImpl> : TermStructure where TImpl : ITermStructureImpl
    {
        protected TermStructure(TImpl impl)
        {
            Impl = impl;
        }

        protected TImpl Impl { get; }

        #region Overrides of TermStructure

        public override bool AllowsExtrapolation => Impl.AllowsExtrapolation;

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Impl.Dispose();
            }
        }
    }

    public interface ITermStructureImpl : IDisposable
    {
        bool AllowsExtrapolation { get; }
    }

    public abstract class TermStructureImpl<TWrapped> : ITermStructureImpl where TWrapped : QlTs
    {
        internal TWrapped QlObj { get; }

        internal TermStructureImpl(TWrapped qlObj)
        {
            QlObj = qlObj;
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

        #region Implementation of ITermStructureImpl

        public bool AllowsExtrapolation => QlObj.allowsExtrapolation();

        #endregion
    }
}