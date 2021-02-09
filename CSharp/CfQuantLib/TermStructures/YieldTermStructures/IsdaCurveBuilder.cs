using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Xml;
using CfAnalytics.QuantLib.InternalUtils;
using CfAnalytics.Utilities;
using Path = System.IO.Path;
using QlYts = QuantLib.YieldTermStructure;
using QlQuote = QuantLib.SimpleQuote;
using QlQuoteHandle = QuantLib.QuoteHandle;
using QlRateHelperList = QuantLib.RateHelperVector;
using QlRateHelper = QuantLib.RateHelper;
using QlDepoHelper = QuantLib.DepositRateHelper;
using QlSwapHelper = QuantLib.SwapRateHelper;

namespace CfAnalytics.QuantLib.TermStructures.YieldTermStructures
{
    public class IsdaCurveBuilder : YieldTermStructure.BuilderBase
    {
        private static readonly global::QuantLib.Calendar CALENDAR = new global::QuantLib.WeekendsOnly();

        public IsdaCurveBuilder(DateTime tradeDate)
            : base(tradeDate, true)
        {
        }

        public XmlDocument CurveData { get; set; }

        #region Overrides of BuilderBase

        public override CalendarName Calendar
        {
            get => CalendarName.WeekendsOnly;
            set => throw new NotSupportedException($"Setting the {nameof(Calendar)} is not supported for {nameof(IsdaCurveBuilder)}");
        }

        public override DayCounter DayCountBasis
        {
            get => DayCounter.Actual365Fixed;
            set => throw new NotSupportedException($"Setting the {nameof(DayCountBasis)} is not supported for {nameof(IsdaCurveBuilder)}");
        }

        internal override QlYts Build()
        {
            var termStructureDayCounter = DayCountBasis.ToQlDayCounter();
            if (CurveData == null)
                CurveData = Download(Currency, TradeDate);
            var rateHelpers = GetRateHelperList(TradeDate, Currency, CurveData);

            return new global::QuantLib.PiecewiseLogLinearDiscount(TradeDate, rateHelpers, termStructureDayCounter);
        }

        #endregion

        #region Private helpers

        private static QlRateHelperList GetRateHelperList(DateTime tradeDate, Currency currency, XmlDocument xmlDoc = null)
        {
            try
            {
                XmlNode curveData = xmlDoc["interestRateCurve"];
                if (curveData == null)
                    throw new ApplicationException("Unexpected XML structure for ISDA curve: {currency}, {tradeDate:yyyy-MM-dd}");

                DateTime effectiveDate = DateTime.Parse(curveData["effectiveasof"].InnerText);
                if ((effectiveDate - tradeDate).TotalDays > 3.0)
                    throw new ApplicationException($"Trade date mismatch in ISDA curve construction. Expected '{tradeDate:yyyy-MM-dd}', got '{effectiveDate:yyyy-MM-dd}'");

                Currency crncy = EnumUtils.GetCurrency(curveData["currency"].InnerText.ToUpper());
                if (crncy != currency)
                    throw new ApplicationException($"Currency mismatch in ISDA curve construction. Expected '{currency}', got '{crncy}'");

                BusinessDayConvention baddays = BusinessDayConventionHelper.FromString(curveData["baddayconvention"].InnerText);

                XmlElement deposits = curveData["deposits"];
                XmlElement swaps = curveData["swaps"];

                var depoDaycount = GetDayCounter(deposits["daycountconvention"].InnerText);
                DateTime depoSpot = DateTime.Parse(deposits["spotdate"].InnerText);
                int depoDays = CALENDAR.businessDaysBetween(tradeDate, depoSpot);

                QlRateHelper GetDepoHelper(string tenor, double depositQuote)
                {
                    var tenorObj = new Period(tenor);
                    var quote = new QlQuoteHandle(new QlQuote(depositQuote));
                    return new QlDepoHelper(quote, tenorObj.QlObj, Convert.ToUInt32(depoDays), CALENDAR, baddays.ToQlConvention(), false, depoDaycount);
                }

                var depoHelpers = deposits.SelectNodes("curvepoint").Cast<XmlNode>().Select(s => new
                {
                    Tenor = s["tenor"].InnerText,
                    Maturity = DateTime.Parse(s["maturitydate"].InnerText),
                    ParRate = Convert.ToDouble(s["parrate"].InnerText)
                }).Select(s => GetDepoHelper(s.Tenor, s.ParRate)).ToList();

                var swapFixedDaycount = GetDayCounter(swaps["fixeddaycountconvention"].InnerText);
                var swapFloatDaycount = GetDayCounter(swaps["floatingdaycountconvention"].InnerText);
                var swapFixedFreq = new Period(swaps["fixedpaymentfrequency"].InnerText);
                var swapFloatFreq = new Period(swaps["floatingpaymentfrequency"].InnerText);
                DateTime swapSpot = DateTime.Parse(swaps["spotdate"].InnerText);
                int swapDays = CALENDAR.businessDaysBetween(tradeDate, swapSpot);
                var swapFloatIdx = new global::QuantLib.IborIndex(string.Empty, swapFloatFreq.QlObj, swapDays, crncy.ToQlCurrency(), CALENDAR, baddays.ToQlConvention(), false,
                    swapFloatDaycount);

                QlRateHelper GetSwapHelper(string tenor, double swapQuote)
                {
                    var tenorObj = new Period(tenor);
                    var quote = new QlQuoteHandle(new QlQuote(swapQuote));
                    return new QlSwapHelper(quote, tenorObj.QlObj, CALENDAR, swapFixedFreq.QlObj.frequency(), baddays.ToQlConvention(), swapFixedDaycount, swapFloatIdx);
                }

                var swapHelpers = swaps.SelectNodes("curvepoint").Cast<XmlNode>().Select(s => new
                {
                    Tenor = s["tenor"].InnerText,
                    Maturity = DateTime.Parse(s["maturitydate"].InnerText),
                    ParRate = Convert.ToDouble(s["parrate"].InnerText)
                }).Select(s => GetSwapHelper(s.Tenor, s.ParRate)).ToList();

                return new QlRateHelperList(depoHelpers.Union(swapHelpers));
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Error parsing XML file for ISDA curve: {currency}, {tradeDate:yyyy-MM-dd}", e);
            }
        }

        private static XmlDocument Download(Currency currency, DateTime tradeDate)
        {
            var fileDate = CALENDAR.advance(tradeDate, -1, global::QuantLib.TimeUnit.Days, global::QuantLib.BusinessDayConvention.Preceding).AsDateTime();
            string filename = $"InterestRates_{currency}_{fileDate:yyyyMMdd}";
            string url = $"https://www.markit.com/news/{filename}.zip";
            string destPath = $"{Path.GetTempPath()}{filename}.zip";
            string fileNameXml = filename + ".xml";

            if (!File.Exists(destPath))
            {
                try
                {
                    using (var webClient = new WebClient())
                    {
                        webClient.DownloadFile(url, destPath);
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException($"Error downloading ISDA curve: {currency}, {tradeDate:yyyy-MM-dd}", e);
                }
            }

            var xmlDoc = new XmlDocument();
            try
            {
                using (ZipArchive archive = ZipFile.Open(destPath, ZipArchiveMode.Read))
                {
                    ZipArchiveEntry entry = archive.GetEntry(fileNameXml);
                    Stream xmlStream = entry.Open();
                    xmlDoc.Load(xmlStream);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Error unzipping ISDA curve: {currency}, {tradeDate:yyyy-MM-dd}", e);
            }

            return xmlDoc;
        }

        private static global::QuantLib.DayCounter GetDayCounter(string dayCountCode)
        {
            switch (dayCountCode)
            {
                case "30/360":
                    return new global::QuantLib.Thirty360();
                case "ACT/360":
                    return new global::QuantLib.Actual360(false);
                default:
                    throw new ArgumentOutOfRangeException(nameof(dayCountCode), dayCountCode);
            }
        }

        #endregion
    }
}