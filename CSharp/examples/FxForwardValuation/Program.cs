using System;
using System.Linq;
using CfAnalytics.QuantLib;
using CfAnalytics.QuantLib.Instruments;
using CfAnalytics.QuantLib.PricingEngines.Fx;
using CfAnalytics.QuantLib.TermStructures;
using BusinessDayConvention = CfAnalytics.BusinessDayConvention;
using Currency = CfAnalytics.Currency;
using ExchangeRate = CfAnalytics.QuantLib.ExchangeRate;
using ExchangeRateManager = CfAnalytics.QuantLib.ExchangeRateManager;
using Money = CfAnalytics.QuantLib.Money;
using Settings = CfAnalytics.QuantLib.Settings;
using TimeUnit = CfAnalytics.TimeUnit;

namespace FxForwardValuation
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;

            var todaysDate = new DateTime(2020, 2, 28);
            Settings.EvaluationDate = todaysDate;
            Console.WriteLine($"Today: {todaysDate:D}\n");

            ExchangeRateManager.MoneyConversion = ExchangeRateManager.ConversionType.AutomatedConversion;
            ExchangeRateManager.Add(Currency.USD, Currency.EUR, 0.9103736341, todaysDate);
            ExchangeRateManager.Add(Currency.GBP, Currency.EUR, 1.1628202171, todaysDate);
            ExchangeRateManager.Add(Currency.CHF, Currency.EUR, 0.9405171323, todaysDate);

            ShortUsdEurExample(todaysDate);
            Console.WriteLine(string.Concat(Enumerable.Repeat('-', 20)));
            ShortGbpEurExample(todaysDate);
            Console.WriteLine(string.Concat(Enumerable.Repeat('-', 20)));
            LongUsdEurExample(todaysDate);
            Console.WriteLine(string.Concat(Enumerable.Repeat('-', 20)));
            LongGbpEurExample(todaysDate);
            Console.WriteLine(string.Concat(Enumerable.Repeat('-', 20)));

            DateTime endTime = DateTime.Now;
            TimeSpan delta = endTime - startTime;
            Console.WriteLine();
            Console.WriteLine("Run completed in {0} s", delta.TotalSeconds);
            Console.WriteLine();
        }

        private static void ShortUsdEurExample(DateTime todaysDate)
        {
            var deliveryDate = new DateTime(2020, 3, 4);
            var baseNotionalAmount = new Money(12925000, Currency.USD);
            var contractAllInRate = new ExchangeRate(Currency.USD, Currency.EUR, 0.897487215294618);

            var fxFwd = new ForeignExchangeForward(deliveryDate, baseNotionalAmount, contractAllInRate);
            Console.WriteLine("Valuation of FxFwd " + fxFwd);

            ExchangeRate spotUsdEurRate = ExchangeRateManager.LookUp(Currency.USD, Currency.EUR, todaysDate);

            var eurUsdFwdCurve = EurUsdFwdPointStructure(todaysDate);
            var usdEurFwdCurve = UsdEurFwdPointStructure(todaysDate);
            var eurDiscountCurve = DiscountingEurCurve(todaysDate);
            var usdDiscountCurve = DiscountingUsdCurve(todaysDate);

            var engine = new ForwardPointsEngine(spotUsdEurRate, usdEurFwdCurve, usdDiscountCurve, eurDiscountCurve);
            fxFwd.setPricingEngine(engine);

            PrintResults(fxFwd);

            // Base Leg:  11,600,022.36 EUR
            // Term Leg: -11,762,835.05 EUR
            // ----------------------------
            // NPV:         -162,812.69 EUR
            // ============================
        }

        private static void ShortGbpEurExample(DateTime todaysDate)
        {
            var deliveryDate = new DateTime(2020, 3, 11);
            var baseNotionalAmount = new Money(40300000, GBP_CCY);
            var contractAllInRate = new ExchangeRate(GBP_CCY, EUR_CCY, 1.16992588519517);

            var fxFwd = new ForeignExchangeForward(deliveryDate, baseNotionalAmount, contractAllInRate);
            Console.WriteLine("Valuation of FxFwd " + fxFwd);

            ExchangeRate spotBaseTermRate = ExchangeRateManager.instance().lookup(GBP_CCY, EUR_CCY, todaysDate);
            var termBaseFwdCurve = EurGbpFwdPointStructure(todaysDate);
            var baseTermFwdCurve = GbpEurFwdPointStructure(todaysDate);
            var termDiscountCurve = DiscountingEurCurve(todaysDate);
            var baseDiscountCurve = DiscountingGbpCurve(todaysDate);

            var engine = new ForwardPointsEngine(spotBaseTermRate, baseTermFwdCurve, baseDiscountCurve, termDiscountCurve);
            fxFwd.setPricingEngine(engine);

            PrintResults(fxFwd);

            // Base Leg:  47,148,013.17 EUR
            // Term Leg: -46,843,587.57 EUR
            // ----------------------------
            // NPV:         -304,425.60 EUR
            // ============================
        }

        private static void LongUsdEurExample(DateTime todaysDate)
        {
            var deliveryDate = new DateTime(2020, 5, 28);
            var baseNotionalAmount = new Money(24750000, USD_CCY);
            var contractAllInRate = new ExchangeRate(USD_CCY, EUR_CCY, 0.919214806712107);

            var fxFwd = new ForeignExchangeForward(deliveryDate, baseNotionalAmount, contractAllInRate);
            Console.WriteLine("Valuation of FxFwd " + fxFwd);

            ExchangeRate spotUsdEurRate = ExchangeRateManager.instance().lookup(USD_CCY, EUR_CCY, todaysDate);
            var eurUsdFwdCurve = EurUsdFwdPointStructure(todaysDate);
            var usdEurFwdCurve = UsdEurFwdPointStructure(todaysDate);
            var eurDiscountCurve = DiscountingEurCurve(todaysDate);
            var usdDiscountCurve = DiscountingUsdCurve(todaysDate);

            var engine = new ForwardPointsEngine(spotUsdEurRate, usdEurFwdCurve, usdDiscountCurve, eurDiscountCurve);
            fxFwd.setPricingEngine(engine);

            PrintResults(fxFwd);

            // Base Leg:  22,750,566.47 EUR
            // Term Leg: -22,412,996.84 EUR
            // ----------------------------
            // NPV:         -337,569.62 EUR
            // ============================
        }

        private static void LongGbpEurExample(DateTime todaysDate)
        {
            var deliveryDate = new DateTime(2020, 5, 28);
            var baseNotionalAmount = new Money(16925000, GBP_CCY);
            var contractAllInRate = new ExchangeRate(GBP_CCY, EUR_CCY, 1.19394431443717);

            var fxFwd = new ForeignExchangeForward(deliveryDate, baseNotionalAmount, contractAllInRate);
            Console.WriteLine("Valuation of FxFwd " + fxFwd);

            ExchangeRate spotBaseTermRate = ExchangeRateManager.instance().lookup(GBP_CCY, EUR_CCY, todaysDate);
            var termBaseFwdCurve = EurGbpFwdPointStructure(todaysDate);
            var baseTermFwdCurve = GbpEurFwdPointStructure(todaysDate);
            var termDiscountCurve = DiscountingEurCurve(todaysDate);
            var baseDiscountCurve = DiscountingGbpCurve(todaysDate);

            var engine = new ForwardPointsEngine(spotBaseTermRate, baseTermFwdCurve, baseDiscountCurve, termDiscountCurve);
            fxFwd.setPricingEngine(engine);

            PrintResults(fxFwd);

            // Base Leg:  20,207,507.52 EUR
            // Term Leg: -19,621,824.42 EUR
            // ----------------------------
            // NPV:         -585,683.10 EUR
            // ============================
        }

        private static void PrintResults(ForeignExchangeForward fxFwd)
        {
            Console.WriteLine($"Fair forward points: {fxFwd.FairForwardPoints()}");
            Console.WriteLine($"Forward base leg value: {fxFwd.ContractNotionalAmountQuote()}");
            Console.WriteLine($"Forward term leg value: {fxFwd.ForwardGrossValueQuote()}");
            Console.WriteLine($"Forward net value: {fxFwd.ForwardNetValueQuote()}");
            Console.WriteLine($"Present net value: {fxFwd.PresentNetValueQuote()}");
        }

        private static FxForwardPointTermStructure UsdEurFwdPointStructure(DateTime todaysDate)
        {
            ExchangeRate spotExchRate = ExchangeRateManager.LookUp(Currency.USD, Currency.EUR, todaysDate);
            if (spotExchRate.BaseCurrency != Currency.USD)
                spotExchRate = spotExchRate.Inverse();
            var builder = new FxForwardPointTermStructure.Builder(todaysDate, spotExchRate)
            {
                BaseCalendar = Calendar.UnitedStatesFederalReserve,
                QuoteCalendar = Calendar.TARGET,
                DayCounter = DayCounter.Actual360,
                ForwardPoints = new[]
                {
                    (new Period(1, TimeUnit.Weeks), -4.051701),
                    (new Period(2, TimeUnit.Weeks), -7.906924),
                    (new Period(3, TimeUnit.Weeks), -11.743311),
                    (new Period(1, TimeUnit.Months), -17.395392),
                    (new Period(2, TimeUnit.Months), -33.074375),
                    (new Period(3, TimeUnit.Months), -47.207796)
                }
            };
            return new FxForwardPointTermStructure(builder);
        }

        private static FxForwardPointTermStructure EurUsdFwdPointStructure(DateTime todaysDate)
        {
            ExchangeRate spotExchRate = ExchangeRateManager.LookUp(Currency.EUR, Currency.USD, todaysDate);
            if (spotExchRate.BaseCurrency != Currency.EUR)
                spotExchRate = spotExchRate.Inverse();
            var builder = new FxForwardPointTermStructure.Builder(todaysDate, spotExchRate)
            {
                BaseCalendar = Calendar.TARGET,
                QuoteCalendar = Calendar.UnitedStatesFederalReserve,
                DayCounter = DayCounter.Actual360,
                ForwardPoints = new[]
                {
                    (new Period(1, TimeUnit.Weeks),  4.9   ),
                    (new Period(2, TimeUnit.Weeks),  9.625 ),
                    (new Period(3, TimeUnit.Weeks),  14.305),
                    (new Period(1, TimeUnit.Months), 21.155),
                    (new Period(2, TimeUnit.Months), 40.669),
                    (new Period(3, TimeUnit.Months), 57.975)
                }
            };
            return new FxForwardPointTermStructure(builder);
        }

        private static FxForwardPointTermStructure GbpEurFwdPointStructure(DateTime todaysDate)
        {
            ExchangeRate spotExchRate = ExchangeRateManager.LookUp(Currency.GBP, Currency.EUR, todaysDate);
            if (spotExchRate.BaseCurrency != Currency.GBP)
                spotExchRate = spotExchRate.Inverse();
            var builder = new FxForwardPointTermStructure.Builder(todaysDate, spotExchRate)
            {
                BaseCalendar = Calendar.UnitedKingdomSettlement,
                QuoteCalendar = Calendar.TARGET,
                DayCounter = DayCounter.Actual360,
                ForwardPoints = new[]
                {
                    (new Period(1, TimeUnit.Weeks),  -2.8  ),
                    (new Period(1, TimeUnit.Months), -12.13),
                    (new Period(2, TimeUnit.Months), -24.16),
                    (new Period(3, TimeUnit.Months), -34.99)
                }
            };
            return new FxForwardPointTermStructure(builder);
        }

        private static FxForwardPointTermStructure EurGbpFwdPointStructure(DateTime todaysDate)
        {
            ExchangeRate spotExchRate = ExchangeRateManager.LookUp(Currency.EUR, Currency.GBP, todaysDate);
            if (spotExchRate.BaseCurrency != Currency.EUR)
                spotExchRate = spotExchRate.Inverse();
            var builder = new FxForwardPointTermStructure.Builder(todaysDate, spotExchRate)
            {
                BaseCalendar = Calendar.TARGET,
                QuoteCalendar = Calendar.UnitedKingdomSettlement,
                DayCounter = DayCounter.Actual360,
                ForwardPoints = new[]
                {
                    (new Period(1, TimeUnit.Weeks),  2.06 ),
                    (new Period(2, TimeUnit.Weeks),  4.01 ),
                    (new Period(3, TimeUnit.Weeks),  6.19 ),
                    (new Period(1, TimeUnit.Months), 8.98 ),
                    (new Period(2, TimeUnit.Months), 17.85),
                    (new Period(3, TimeUnit.Months), 25.97)
                }
            };
            return new FxForwardPointTermStructure(builder);
        }

        private static RelinkableYieldTermStructureHandle DiscountingEurCurve(DateTime todaysDate)
        {
            DayCounter termStructureDayCounter = new ActualActual(ActualActual.Convention.ISDA);

            // deposits
            int fixingDays = 0;
            Calendar calendar = new TARGET();
            var depositConv = BusinessDayConvention.ModifiedFollowing;
            Date settlementDate = calendar.advance(todaysDate, fixingDays, TimeUnit.Days);
            DayCounter depositDayCount = new Actual360();

            var d1WRate = new QuoteHandle(new SimpleQuote(-0.00518));
            var d1MRate = new QuoteHandle(new SimpleQuote(-0.00488));
            var d3MRate = new QuoteHandle(new SimpleQuote(-0.00424));
            var d6MRate = new QuoteHandle(new SimpleQuote(-0.00386));
            var d1YRate = new QuoteHandle(new SimpleQuote(-0.00311));

            var d1W = new DepositRateHelper(d1WRate, new Period(1, TimeUnit.Weeks), Convert.ToUInt32(fixingDays), calendar, depositConv, true, depositDayCount);
            var d1M = new DepositRateHelper(d1MRate, new Period(1, TimeUnit.Months), Convert.ToUInt32(fixingDays), calendar, depositConv, true, depositDayCount);
            var d3M = new DepositRateHelper(d3MRate, new Period(3, TimeUnit.Months), Convert.ToUInt32(fixingDays), calendar, depositConv, true, depositDayCount);
            var d6M = new DepositRateHelper(d6MRate, new Period(6, TimeUnit.Months), Convert.ToUInt32(fixingDays), calendar, depositConv, true, depositDayCount);
            var d1Y = new DepositRateHelper(d1YRate, new Period(1, TimeUnit.Years), Convert.ToUInt32(fixingDays), calendar, depositConv, true, depositDayCount);
            var depoSwapInstruments = new RateHelperVector {d1W, d1M, d3M, d6M, d1Y};

            var depoTermStructure = new PiecewiseLogLinearDiscount(settlementDate, depoSwapInstruments, termStructureDayCounter);
            return new RelinkableYieldTermStructureHandle(depoTermStructure);
        }

        private static RelinkableYieldTermStructureHandle DiscountingUsdCurve(DateTime todaysDate)
        {
            DayCounter termStructureDayCounter = new ActualActual(ActualActual.Convention.ISDA);

            // deposits
            int fixingDays = 0;
            Calendar calendar = new UnitedStates(UnitedStates.Market.FederalReserve);
            var depositConv = BusinessDayConvention.ModifiedFollowing;
            Date settlementDate = calendar.advance(todaysDate, fixingDays, TimeUnit.Days);
            DayCounter depositDayCount = new Actual360();

            var d1WRate = new QuoteHandle(new SimpleQuote(0.01568));
            var d1MRate = new QuoteHandle(new SimpleQuote(0.0151525));
            var d3MRate = new QuoteHandle(new SimpleQuote(0.0146275));
            var d6MRate = new QuoteHandle(new SimpleQuote(0.0139725));
            var d1YRate = new QuoteHandle(new SimpleQuote(0.013815));

            var d1W = new DepositRateHelper(d1WRate, new Period(1, TimeUnit.Weeks), Convert.ToUInt32(fixingDays), calendar, depositConv, true, depositDayCount);
            var d1M = new DepositRateHelper(d1MRate, new Period(1, TimeUnit.Months), Convert.ToUInt32(fixingDays), calendar, depositConv, true, depositDayCount);
            var d3M = new DepositRateHelper(d3MRate, new Period(3, TimeUnit.Months), Convert.ToUInt32(fixingDays), calendar, depositConv, true, depositDayCount);
            var d6M = new DepositRateHelper(d6MRate, new Period(6, TimeUnit.Months), Convert.ToUInt32(fixingDays), calendar, depositConv, true, depositDayCount);
            var d1Y = new DepositRateHelper(d1YRate, new Period(1, TimeUnit.Years), Convert.ToUInt32(fixingDays), calendar, depositConv, true, depositDayCount);
            var depoSwapInstruments = new RateHelperVector {d1W, d1M, d3M, d6M, d1Y};

            var depoTermStructure = new PiecewiseLogLinearDiscount(settlementDate, depoSwapInstruments, termStructureDayCounter);
            return new RelinkableYieldTermStructureHandle(depoTermStructure);
        }

        private static RelinkableYieldTermStructureHandle DiscountingGbpCurve(DateTime todaysDate)
        {
            DayCounter termStructureDayCounter = new ActualActual(ActualActual.Convention.ISDA);

            // deposits
            int fixingDays = 0;
            Calendar calendar = new UnitedKingdom(UnitedKingdom.Market.Settlement);
            var depositConv = BusinessDayConvention.ModifiedFollowing;
            Date settlementDate = calendar.advance(todaysDate, fixingDays, TimeUnit.Days);
            DayCounter depositDayCount = new Actual365Fixed(Actual365Fixed.Convention.Standard);

            var d1WRate = new QuoteHandle(new SimpleQuote(0.00681));
            var d1MRate = new QuoteHandle(new SimpleQuote(0.0067675));
            var d3MRate = new QuoteHandle(new SimpleQuote(0.0067275));
            var d6MRate = new QuoteHandle(new SimpleQuote(0.0068675));
            var d1YRate = new QuoteHandle(new SimpleQuote(0.0075038));

            var d1W = new DepositRateHelper(d1WRate, new Period(1, TimeUnit.Weeks), Convert.ToUInt32(fixingDays), calendar, depositConv, true, depositDayCount);
            var d1M = new DepositRateHelper(d1MRate, new Period(1, TimeUnit.Months), Convert.ToUInt32(fixingDays), calendar, depositConv, true, depositDayCount);
            var d3M = new DepositRateHelper(d3MRate, new Period(3, TimeUnit.Months), Convert.ToUInt32(fixingDays), calendar, depositConv, true, depositDayCount);
            var d6M = new DepositRateHelper(d6MRate, new Period(6, TimeUnit.Months), Convert.ToUInt32(fixingDays), calendar, depositConv, true, depositDayCount);
            var d1Y = new DepositRateHelper(d1YRate, new Period(1, TimeUnit.Years), Convert.ToUInt32(fixingDays), calendar, depositConv, true, depositDayCount);
            var depoSwapInstruments = new RateHelperVector {d1W, d1M, d3M, d6M, d1Y};

            var depoTermStructure = new PiecewiseLogLinearDiscount(settlementDate, depoSwapInstruments, termStructureDayCounter);
            return new RelinkableYieldTermStructureHandle(depoTermStructure);
        }
    }
}