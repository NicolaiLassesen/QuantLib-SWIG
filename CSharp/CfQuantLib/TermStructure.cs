namespace CfAnalytics.QuantLib
{
    public class TermStructure
    {
    }

    public class TermStructure<TWrapped> : TermStructure
    {
        internal TWrapped QlObj { get; }

        internal TermStructure(TWrapped qlObj)
        {
            QlObj = qlObj;
        }
    }
}