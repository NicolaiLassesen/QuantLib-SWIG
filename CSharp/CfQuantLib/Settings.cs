using System;
using CfAnalytics.QuantLib.InternalUtils;
using QlSettings = QuantLib.Settings;
using QlDate = QuantLib.Date;

namespace CfAnalytics.QuantLib
{
    public static class Settings
    {
        internal static QlDate QlEvaluationDate
        {
            get => QlSettings.instance().getEvaluationDate();
            set => QlSettings.instance().setEvaluationDate(value);
        }

        public static DateTime EvaluationDate
        {
            get => QlEvaluationDate.AsDateTime();
            set => QlEvaluationDate = value;
        }
    }
}