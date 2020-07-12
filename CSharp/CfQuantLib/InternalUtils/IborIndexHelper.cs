using System;
using QlPeriod = QuantLib.Period;
using QlTime = QuantLib.TimeUnit;
using QlIborIdx = QuantLib.IborIndex;
using QlEuribor = QuantLib.Euribor;
using QlUsdLibor = QuantLib.USDLibor;
using QlGbpLibor = QuantLib.GBPLibor;
using QlChfLibor = QuantLib.CHFLibor;
using QlJpyLibor = QuantLib.JPYLibor;

namespace CfAnalytics.QuantLib.InternalUtils
{
    public static class IborIndexHelper
    {
        public static QlIborIdx GetIborIndex(string iborIndex)
        {
            switch (iborIndex)
            {
                case "EUR-EURIBOR-1M": return Euribor1M;
                case "EUR-EURIBOR-2M": return Euribor2M;
                case "EUR-EURIBOR-3M": return Euribor3M;
                case "EUR-EURIBOR-4M": return Euribor4M;
                case "EUR-EURIBOR-5M": return Euribor5M;
                case "EUR-EURIBOR-6M": return Euribor6M;
                case "EUR-EURIBOR-9M": return Euribor9M;
                case "EUR-EURIBOR-12M": return Euribor12M;

                case "USD-LIBOR-1M": return UsdLibor1M;
                case "USD-LIBOR-2M": return UsdLibor2M;
                case "USD-LIBOR-3M": return UsdLibor3M;
                case "USD-LIBOR-4M": return UsdLibor4M;
                case "USD-LIBOR-5M": return UsdLibor5M;
                case "USD-LIBOR-6M": return UsdLibor6M;
                case "USD-LIBOR-9M": return UsdLibor9M;
                case "USD-LIBOR-12M": return UsdLibor12M;

                case "GBP-LIBOR-1M": return GbpLibor1M;
                case "GBP-LIBOR-2M": return GbpLibor2M;
                case "GBP-LIBOR-3M": return GbpLibor3M;
                case "GBP-LIBOR-4M": return GbpLibor4M;
                case "GBP-LIBOR-5M": return GbpLibor5M;
                case "GBP-LIBOR-6M": return GbpLibor6M;
                case "GBP-LIBOR-9M": return GbpLibor9M;
                case "GBP-LIBOR-12M": return GbpLibor12M;

                case "CHF-LIBOR-1M": return ChfLibor1M;
                case "CHF-LIBOR-2M": return ChfLibor2M;
                case "CHF-LIBOR-3M": return ChfLibor3M;
                case "CHF-LIBOR-4M": return ChfLibor4M;
                case "CHF-LIBOR-5M": return ChfLibor5M;
                case "CHF-LIBOR-6M": return ChfLibor6M;
                case "CHF-LIBOR-9M": return ChfLibor9M;
                case "CHF-LIBOR-12M": return ChfLibor12M;

                case "JPY-LIBOR-1M": return JpyLibor1M;
                case "JPY-LIBOR-2M": return JpyLibor2M;
                case "JPY-LIBOR-3M": return JpyLibor3M;
                case "JPY-LIBOR-4M": return JpyLibor4M;
                case "JPY-LIBOR-5M": return JpyLibor5M;
                case "JPY-LIBOR-6M": return JpyLibor6M;
                case "JPY-LIBOR-9M": return JpyLibor9M;
                case "JPY-LIBOR-12M": return JpyLibor12M;
                default:
                    throw new ArgumentOutOfRangeException(nameof(iborIndex), iborIndex, "Unmapped Ibor Index");
            }
        }

        public static QlEuribor Euribor1M = new global::QuantLib.Euribor1M();
        public static QlEuribor Euribor2M = new global::QuantLib.Euribor2M();
        public static QlEuribor Euribor3M = new global::QuantLib.Euribor3M();
        public static QlEuribor Euribor4M = new global::QuantLib.Euribor4M();
        public static QlEuribor Euribor5M = new global::QuantLib.Euribor5M();
        public static QlEuribor Euribor6M = new global::QuantLib.Euribor6M();
        public static QlEuribor Euribor9M = new global::QuantLib.Euribor9M();
        public static QlEuribor Euribor12M = new global::QuantLib.Euribor1Y();

        public static QlUsdLibor UsdLibor1M = new QlUsdLibor(new QlPeriod(1, QlTime.Months));
        public static QlUsdLibor UsdLibor2M = new QlUsdLibor(new QlPeriod(2, QlTime.Months));
        public static QlUsdLibor UsdLibor3M = new QlUsdLibor(new QlPeriod(3, QlTime.Months));
        public static QlUsdLibor UsdLibor4M = new QlUsdLibor(new QlPeriod(4, QlTime.Months));
        public static QlUsdLibor UsdLibor5M = new QlUsdLibor(new QlPeriod(5, QlTime.Months));
        public static QlUsdLibor UsdLibor6M = new QlUsdLibor(new QlPeriod(6, QlTime.Months));
        public static QlUsdLibor UsdLibor9M = new QlUsdLibor(new QlPeriod(9, QlTime.Months));
        public static QlUsdLibor UsdLibor12M = new QlUsdLibor(new QlPeriod(12, QlTime.Months));

        public static QlGbpLibor GbpLibor1M = new QlGbpLibor(new QlPeriod(1, QlTime.Months));
        public static QlGbpLibor GbpLibor2M = new QlGbpLibor(new QlPeriod(2, QlTime.Months));
        public static QlGbpLibor GbpLibor3M = new QlGbpLibor(new QlPeriod(3, QlTime.Months));
        public static QlGbpLibor GbpLibor4M = new QlGbpLibor(new QlPeriod(4, QlTime.Months));
        public static QlGbpLibor GbpLibor5M = new QlGbpLibor(new QlPeriod(5, QlTime.Months));
        public static QlGbpLibor GbpLibor6M = new QlGbpLibor(new QlPeriod(6, QlTime.Months));
        public static QlGbpLibor GbpLibor9M = new QlGbpLibor(new QlPeriod(9, QlTime.Months));
        public static QlGbpLibor GbpLibor12M = new QlGbpLibor(new QlPeriod(12, QlTime.Months));

        public static QlChfLibor ChfLibor1M = new QlChfLibor(new QlPeriod(1, QlTime.Months));
        public static QlChfLibor ChfLibor2M = new QlChfLibor(new QlPeriod(2, QlTime.Months));
        public static QlChfLibor ChfLibor3M = new QlChfLibor(new QlPeriod(3, QlTime.Months));
        public static QlChfLibor ChfLibor4M = new QlChfLibor(new QlPeriod(4, QlTime.Months));
        public static QlChfLibor ChfLibor5M = new QlChfLibor(new QlPeriod(5, QlTime.Months));
        public static QlChfLibor ChfLibor6M = new QlChfLibor(new QlPeriod(6, QlTime.Months));
        public static QlChfLibor ChfLibor9M = new QlChfLibor(new QlPeriod(9, QlTime.Months));
        public static QlChfLibor ChfLibor12M = new QlChfLibor(new QlPeriod(12, QlTime.Months));

        public static QlJpyLibor JpyLibor1M = new QlJpyLibor(new QlPeriod(1, QlTime.Months));
        public static QlJpyLibor JpyLibor2M = new QlJpyLibor(new QlPeriod(2, QlTime.Months));
        public static QlJpyLibor JpyLibor3M = new QlJpyLibor(new QlPeriod(3, QlTime.Months));
        public static QlJpyLibor JpyLibor4M = new QlJpyLibor(new QlPeriod(4, QlTime.Months));
        public static QlJpyLibor JpyLibor5M = new QlJpyLibor(new QlPeriod(5, QlTime.Months));
        public static QlJpyLibor JpyLibor6M = new QlJpyLibor(new QlPeriod(6, QlTime.Months));
        public static QlJpyLibor JpyLibor9M = new QlJpyLibor(new QlPeriod(9, QlTime.Months));
        public static QlJpyLibor JpyLibor12M = new QlJpyLibor(new QlPeriod(12, QlTime.Months));
    }
}