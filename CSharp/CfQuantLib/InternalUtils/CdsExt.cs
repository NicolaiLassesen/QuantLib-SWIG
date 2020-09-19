using System;
using CfCdsSide = CfAnalytics.QuantLib.Instruments.CreditDefaultSwap.Side;
using QlCdsSide = QuantLib.Protection.Side;

namespace CfAnalytics.QuantLib.InternalUtils
{
    internal static class CdsExt
    {
        internal static CfCdsSide ToCfSide(this QlCdsSide side)
        {
            switch (side)
            {
                case QlCdsSide.Buyer: return CfCdsSide.Buyer;
                case QlCdsSide.Seller: return CfCdsSide.Seller;
                default: throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }

        internal static QlCdsSide ToQlSide(this CfCdsSide side)
        {
            switch (side)
            {
                case CfCdsSide.Buyer: return QlCdsSide.Buyer;
                case CfCdsSide.Seller: return QlCdsSide.Seller;
                default: throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }
    }
}