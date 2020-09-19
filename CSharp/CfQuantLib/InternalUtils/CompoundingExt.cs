using System;
using QlCompounding = QuantLib.Compounding;
using CfCompounding = CfAnalytics.Compounding;

namespace CfAnalytics.QuantLib.InternalUtils
{
    internal static class CompoundingExt
    {
        internal static QlCompounding ToQlCompounding(this CfCompounding compounding)
        {
            switch (compounding)
            {
                case CfCompounding.Simple: return QlCompounding.Simple;
                case CfCompounding.Compounded: return QlCompounding.Compounded;
                case CfCompounding.Continuous: return QlCompounding.Continuous;
                case CfCompounding.SimpleThenCompounded: return QlCompounding.SimpleThenCompounded;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compounding), compounding, "Unmapped compounding when mapping to QL");
            }
        }

        internal static CfCompounding ToCfCompounding(this QlCompounding compounding)
        {
            switch (compounding)
            {
                case QlCompounding.Simple: return CfCompounding.Simple;
                case QlCompounding.Compounded: return CfCompounding.Compounded;
                case QlCompounding.Continuous: return CfCompounding.Continuous;
                case QlCompounding.SimpleThenCompounded: return CfCompounding.SimpleThenCompounded;
                case QlCompounding.CompoundedThenSimple:
                default:
                    throw new ArgumentOutOfRangeException(nameof(compounding), compounding, "Unmapped compounding when mapping from QL");
            }
        }
    }
}