using System;
using QlDateGen = QuantLib.DateGeneration.Rule;
using CfDateGen = CfAnalytics.Utilities.Schedule.DateGeneration;

namespace CfAnalytics.QuantLib.InternalUtils
{
    internal static class DataGenerationExt
    {
        internal static QlDateGen ToQlDateGeneration(this CfDateGen dateGeneration)
        {
            switch (dateGeneration)
            {
                case CfDateGen.None: return QlDateGen.Zero;
                case CfDateGen.Backward: return QlDateGen.Backward;
                case CfDateGen.Forward: return QlDateGen.Forward;
                case CfDateGen.ThirdWednesday: return QlDateGen.ThirdWednesday;
                case CfDateGen.Twentieth: return QlDateGen.Twentieth;
                case CfDateGen.TwentiethImm: return QlDateGen.TwentiethIMM;
                case CfDateGen.Zero: return QlDateGen.Zero;
                case CfDateGen.OldCds: return QlDateGen.OldCDS;
                case CfDateGen.Cds: return QlDateGen.CDS;
                case CfDateGen.Cds2015: return QlDateGen.CDS2015;
                default: throw new ArgumentOutOfRangeException(nameof(dateGeneration), dateGeneration, null);
            }
        }

        internal static CfDateGen ToCfDateGeneration(this QlDateGen dateGeneration)
        {
            switch (dateGeneration)
            {
                case QlDateGen.Backward: return CfDateGen.Backward;
                case QlDateGen.Forward: return CfDateGen.Forward;
                case QlDateGen.Zero: return CfDateGen.Zero;
                case QlDateGen.ThirdWednesday: return CfDateGen.ThirdWednesday;
                case QlDateGen.Twentieth: return CfDateGen.Twentieth;
                case QlDateGen.TwentiethIMM: return CfDateGen.TwentiethImm;
                case QlDateGen.OldCDS: return CfDateGen.OldCds;
                case QlDateGen.CDS: return CfDateGen.Cds;
                case QlDateGen.CDS2015: return CfDateGen.Cds2015;
                default: throw new ArgumentOutOfRangeException(nameof(dateGeneration), dateGeneration, null);
            }
        }
    }
}