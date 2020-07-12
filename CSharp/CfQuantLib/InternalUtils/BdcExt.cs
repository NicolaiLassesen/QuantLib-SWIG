using System;
using QlConv = QuantLib.BusinessDayConvention;
using CfConv = CfAnalytics.BusinessDayConvention;
// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib.InternalUtils
{
    public static class BdcExt
    {
        public static QlConv ToQlConvention(this CfConv convention)
        {
            switch (convention)
            {
                case CfConv.Following: return QlConv.Following;
                case CfConv.ModifiedFollowing: return QlConv.ModifiedFollowing;
                case CfConv.Preceding: return QlConv.Preceding;
                case CfConv.ModifiedPreceding: return QlConv.ModifiedPreceding;
                case CfConv.Unadjusted: return QlConv.Unadjusted;
                default: throw new ArgumentOutOfRangeException(nameof(convention), convention, null);
            }
        }

        public static CfConv ToCfConvention(this QlConv convention)
        {
            switch (convention)
            {
                case QlConv.Following: return CfConv.Following;
                case QlConv.ModifiedFollowing: return CfConv.ModifiedFollowing;
                case QlConv.Preceding: return CfConv.Preceding;
                case QlConv.ModifiedPreceding: return CfConv.ModifiedPreceding;
                case QlConv.Unadjusted: return CfConv.Unadjusted;
                default: throw new ArgumentOutOfRangeException(nameof(convention), convention, null);
            }
        }
    }
}