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
// ReSharper disable InconsistentNaming

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

            ExchangeRate spotUsdEurRate = ExchangeRateManager.Lookup(Currency.USD, Currency.EUR, todaysDate);

            var usdEurFwdCurve = UsdEurFwdPointStructure(todaysDate);
            var eurDiscountCurve = DiscountingEurCurve(todaysDate);
            var usdDiscountCurve = DiscountingUsdCurve(todaysDate);

            fxFwd.PricingEngine = new ForwardPointsEngine(spotUsdEurRate, usdEurFwdCurve, usdDiscountCurve, eurDiscountCurve);

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
            var baseNotionalAmount = new Money(40300000, Currency.GBP);
            var contractAllInRate = new ExchangeRate(Currency.GBP, Currency.EUR, 1.16992588519517);

            var fxFwd = new ForeignExchangeForward(deliveryDate, baseNotionalAmount, contractAllInRate);
            Console.WriteLine("Valuation of FxFwd " + fxFwd);

            ExchangeRate spotBaseTermRate = ExchangeRateManager.Lookup(Currency.GBP, Currency.EUR, todaysDate);
            var termBaseFwdCurve = EurGbpFwdPointStructure(todaysDate);
            var baseTermFwdCurve = GbpEurFwdPointStructure(todaysDate);
            var termDiscountCurve = DiscountingEurCurve(todaysDate);
            var baseDiscountCurve = DiscountingGbpCurve(todaysDate);

            fxFwd.PricingEngine= new ForwardPointsEngine(spotBaseTermRate, baseTermFwdCurve, baseDiscountCurve, termDiscountCurve);

            PrintResults(fxFwd);

            // Base Leg:  47,148,013.17 EUR
            // Term Leg: -46,843,587.57 EUR
            // ----------------------------
            // NPV:          304,425.60 EUR
            // ============================
        }

        private static void LongUsdEurExample(DateTime todaysDate)
        {
            var deliveryDate = new DateTime(2020, 5, 28);
            var baseNotionalAmount = new Money(24750000, Currency.USD);
            var contractAllInRate = new ExchangeRate(Currency.USD, Currency.EUR, 0.919214806712107);

            var fxFwd = new ForeignExchangeForward(deliveryDate, baseNotionalAmount, contractAllInRate);
            Console.WriteLine("Valuation of FxFwd " + fxFwd);

            ExchangeRate spotUsdEurRate = ExchangeRateManager.Lookup(Currency.USD, Currency.EUR, todaysDate);
            var eurUsdFwdCurve = EurUsdFwdPointStructure(todaysDate);
            var usdEurFwdCurve = UsdEurFwdPointStructure(todaysDate);
            var eurDiscountCurve = DiscountingEurCurve(todaysDate);
            var usdDiscountCurve = DiscountingUsdCurve(todaysDate);

            fxFwd.PricingEngine = new ForwardPointsEngine(spotUsdEurRate, usdEurFwdCurve, usdDiscountCurve, eurDiscountCurve);

            PrintResults(fxFwd);

            // Base Leg:  22,750,566.47 EUR
            // Term Leg: -22,412,996.84 EUR
            // ----------------------------
            // NPV:          337,569.62 EUR
            // ============================
        }

        private static void LongGbpEurExample(DateTime todaysDate)
        {
            var deliveryDate = new DateTime(2020, 5, 28);
            var baseNotionalAmount = new Money(16925000, Currency.GBP);
            var contractAllInRate = new ExchangeRate(Currency.GBP, Currency.EUR, 1.19394431443717);

            var fxFwd = new ForeignExchangeForward(deliveryDate, baseNotionalAmount, contractAllInRate);
            Console.WriteLine("Valuation of FxFwd " + fxFwd);

            ExchangeRate spotBaseTermRate = ExchangeRateManager.Lookup(Currency.GBP, Currency.EUR, todaysDate);
            var termBaseFwdCurve = EurGbpFwdPointStructure(todaysDate);
            var baseTermFwdCurve = GbpEurFwdPointStructure(todaysDate);
            var termDiscountCurve = DiscountingEurCurve(todaysDate);
            var baseDiscountCurve = DiscountingGbpCurve(todaysDate);

            fxFwd.PricingEngine = new ForwardPointsEngine(spotBaseTermRate, baseTermFwdCurve, baseDiscountCurve, termDiscountCurve);

            PrintResults(fxFwd);

            // Base Leg:  20,207,507.52 EUR
            // Term Leg: -19,621,824.42 EUR
            // ----------------------------
            // NPV:          585,683.10 EUR
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
            ExchangeRate spotExchRate = ExchangeRateManager.Lookup(Currency.USD, Currency.EUR, todaysDate);
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
            ExchangeRate spotExchRate = ExchangeRateManager.Lookup(Currency.EUR, Currency.USD, todaysDate);
            if (spotExchRate.BaseCurrency != Currency.EUR)
                spotExchRate = spotExchRate.Inverse();
            var builder = new FxForwardPointTermStructure.Builder(todaysDate, spotExchRate)
            {
                BaseCalendar = Calendar.TARGET,
                QuoteCalendar = Calendar.UnitedStatesFederalReserve,
                DayCounter = DayCounter.Actual360,
                ForwardPoints = new[]
                {
                    (new Period(1, TimeUnit.Weeks), 4.9),
                    (new Period(2, TimeUnit.Weeks), 9.625),
                    (new Period(3, TimeUnit.Weeks), 14.305),
                    (new Period(1, TimeUnit.Months), 21.155),
                    (new Period(2, TimeUnit.Months), 40.669),
                    (new Period(3, TimeUnit.Months), 57.975)
                }
            };
            return new FxForwardPointTermStructure(builder);
        }

        private static FxForwardPointTermStructure GbpEurFwdPointStructure(DateTime todaysDate)
        {
            ExchangeRate spotExchRate = ExchangeRateManager.Lookup(Currency.GBP, Currency.EUR, todaysDate);
            if (spotExchRate.BaseCurrency != Currency.GBP)
                spotExchRate = spotExchRate.Inverse();
            var builder = new FxForwardPointTermStructure.Builder(todaysDate, spotExchRate)
            {
                BaseCalendar = Calendar.UnitedKingdomSettlement,
                QuoteCalendar = Calendar.TARGET,
                DayCounter = DayCounter.Actual360,
                ForwardPoints = new[]
                {
                    (new Period(1, TimeUnit.Weeks), -2.8),
                    (new Period(1, TimeUnit.Months), -12.13),
                    (new Period(2, TimeUnit.Months), -24.16),
                    (new Period(3, TimeUnit.Months), -34.99)
                }
            };
            return new FxForwardPointTermStructure(builder);
        }

        private static FxForwardPointTermStructure EurGbpFwdPointStructure(DateTime todaysDate)
        {
            ExchangeRate spotExchRate = ExchangeRateManager.Lookup(Currency.EUR, Currency.GBP, todaysDate);
            if (spotExchRate.BaseCurrency != Currency.EUR)
                spotExchRate = spotExchRate.Inverse();
            var builder = new FxForwardPointTermStructure.Builder(todaysDate, spotExchRate)
            {
                BaseCalendar = Calendar.TARGET,
                QuoteCalendar = Calendar.UnitedKingdomSettlement,
                DayCounter = DayCounter.Actual360,
                ForwardPoints = new[]
                {
                    (new Period(1, TimeUnit.Weeks), 2.06),
                    (new Period(2, TimeUnit.Weeks), 4.01),
                    (new Period(3, TimeUnit.Weeks), 6.19),
                    (new Period(1, TimeUnit.Months), 8.98),
                    (new Period(2, TimeUnit.Months), 17.85),
                    (new Period(3, TimeUnit.Months), 25.97)
                }
            };
            return new FxForwardPointTermStructure(builder);
        }

        private static YieldTermStructure DiscountingEurCurve(DateTime todaysDate)
        {
            var builder = new YieldTermStructure.Builder(todaysDate)
            {
                SpotDays = 0,
                Currency = Currency.EUR,
                Calendar = Calendar.TARGET,
                DayCounter = DayCounter.ActualActualISDA,
                DepositConvention = BusinessDayConvention.ModifiedFollowing,
                DepositDayCounter = DayCounter.Actual360,
                DepositRates = new[]
                {
                    (new Period(1, TimeUnit.Weeks), -0.00518),
                    (new Period(1, TimeUnit.Months), -0.00488),
                    (new Period(3, TimeUnit.Months), -0.00424),
                    (new Period(6, TimeUnit.Months), -0.00386),
                    (new Period(1, TimeUnit.Years), -0.00311)
                }
            };
            return new YieldTermStructure(builder);
        }

        private static YieldTermStructure DiscountingUsdCurve(DateTime todaysDate)
        {
            var builder = new YieldTermStructure.Builder(todaysDate)
            {
                SpotDays = 0,
                Currency = Currency.USD,
                Calendar = Calendar.UnitedStatesFederalReserve,
                DayCounter = DayCounter.ActualActualISDA,
                DepositConvention = BusinessDayConvention.ModifiedFollowing,
                DepositDayCounter = DayCounter.Actual360,
                DepositRates = new[]
                {
                    (new Period(1, TimeUnit.Weeks), 0.01568  ),
                    (new Period(1, TimeUnit.Months),0.0151525),
                    (new Period(3, TimeUnit.Months),0.0146275),
                    (new Period(6, TimeUnit.Months),0.0139725),
                    (new Period(1, TimeUnit.Years), 0.013815 )
                }
            };
            return new YieldTermStructure(builder);
        }

        private static YieldTermStructure DiscountingGbpCurve(DateTime todaysDate)
        {
            var builder = new YieldTermStructure.Builder(todaysDate)
            {
                SpotDays = 0,
                Currency = Currency.GBP,
                Calendar = Calendar.UnitedKingdomSettlement,
                DayCounter = DayCounter.ActualActualISDA,
                DepositConvention = BusinessDayConvention.ModifiedFollowing,
                DepositDayCounter = DayCounter.Actual365FixedStandard,
                DepositRates = new[]
                {
                    (new Period(1, TimeUnit.Weeks), 0.00681  ),
                    (new Period(1, TimeUnit.Months),0.0067675),
                    (new Period(3, TimeUnit.Months),0.0067275),
                    (new Period(6, TimeUnit.Months),0.0068675),
                    (new Period(1, TimeUnit.Years), 0.0075038)
                }
            };
            return new YieldTermStructure(builder);
        }
    }
}