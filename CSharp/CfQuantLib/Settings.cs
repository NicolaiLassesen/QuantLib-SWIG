using System;
using QuantLib;
using QlSettings = QuantLib.Settings;

namespace CfAnalytics.QuantLib
{
    public static class Settings
    {
        public static DateTime EvaluationDate
        {
            get => QlSettings.instance().getEvaluationDate().AsDateTime();
            set => QlSettings.instance().setEvaluationDate(value);
        }
    }
}
