using QlTs = QuantLib.TermStructure;

namespace CfAnalytics.QuantLib
{
    public abstract class TermStructure
    {
        public abstract override string ToString();
    }

    public abstract class TermStructure<TWrapped> : TermStructure where TWrapped : QlTs
    {
        internal TWrapped QlObj { get; }

        internal TermStructure(TWrapped qlObj)
        {
            QlObj = qlObj;
        }
    }
}