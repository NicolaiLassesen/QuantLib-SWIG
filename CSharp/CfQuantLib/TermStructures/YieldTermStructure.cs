using QuantLib;

namespace CfAnalytics.QuantLib.TermStructures
{
    public class YieldTermStructure : TermStructure<YieldTermStructureHandle>
    {
        public class Builder
        {
            internal YieldTermStructureHandle BuildHandle()
            {
                return new YieldTermStructureHandle();
            }
        }

        public YieldTermStructure(Builder builder)
            : base(builder.BuildHandle())
        {
        }
    }
}