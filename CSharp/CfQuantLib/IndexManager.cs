using System;
using System.Collections.Generic;
using System.Linq;
using CfAnalytics.QuantLib.InternalUtils;
using QlIm = QuantLib.IndexManager;

namespace CfAnalytics.QuantLib
{
    public static class IndexManager
    {
        public static ICollection<string> IndexList
        {
            get
            {
                var qlList = QlIm.instance().histories();
                return qlList == null ? new List<string>() : qlList.ToList();
            }
        }

        public static void ClearAll()
        {
            QlIm.instance().clearHistories();
        }

        public static void ClearIndex(string indexName)
        {
            QlIm.instance().clearHistory(indexName);
        }

        public static bool HasIndexFixings(string indexName)
        {
            return QlIm.instance().hasHistory(indexName);
        }

        public static void AddFixing(string indexName, DateTime fixingDate, double fixingValue)
        {
            var ts = QlIm.instance().getHistory(indexName);
            ts.set(fixingDate, fixingValue);
        }

        public static double GetFixing(string indexName, DateTime fixingDate)
        {
            var ts = QlIm.instance().getHistory(indexName);
            return ts.get(fixingDate);
        }

        public static Dictionary<DateTime, double> GetFixings(string indexName)
        {
            var ts = QlIm.instance().getHistory(indexName);
            return Enumerable.Range(0, Convert.ToInt32(ts.size())).ToDictionary(i => ts.dates()[i].AsDateTime(), i => ts.values()[i]);
        }
    }
}