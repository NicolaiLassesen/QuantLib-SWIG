namespace CfAnalytics.QuantLib
{
    public class Instrument
    {
    }

    public class Instrument<TWrapped> : Instrument
    {
        internal TWrapped QlObj { get; }

        internal Instrument(TWrapped qlObj)
        {
            QlObj = qlObj;
        }
    }
}