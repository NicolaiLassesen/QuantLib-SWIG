using System;
using CfAnalytics;
using CfAnalytics.QuantLib;
using CfAnalytics.QuantLib.Instruments;
using CfAnalytics.QuantLib.PricingEngines.Credit;
using CfAnalytics.QuantLib.TermStructures.DefaultProbabilityTermStructures;
using CfAnalytics.QuantLib.TermStructures.YieldTermStructures;
using CfAnalytics.Utilities;
using BusinessDayConvention = CfAnalytics.BusinessDayConvention;
using Currency = CfAnalytics.Currency;
using DayCounter = CfAnalytics.QuantLib.DayCounter;
using DefaultProbabilityTermStructure = CfAnalytics.QuantLib.TermStructures.DefaultProbabilityTermStructure;
using Frequency = CfAnalytics.Frequency;
using Period = CfAnalytics.QuantLib.Period;
using Settings = CfAnalytics.QuantLib.Settings;
using TimeUnit = CfAnalytics.TimeUnit;
using YieldTermStructure = CfAnalytics.QuantLib.TermStructures.YieldTermStructure;

// ReSharper disable InconsistentNaming

namespace CdsValuation
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;

            var todaysDate = new DateTime(2020, 9, 19);
            Settings.EvaluationDate = todaysDate;
            Console.WriteLine($"Today: {todaysDate:D}\n");

            var builder = new SwapCurveBootstrapBuilder(todaysDate)
            {
                SpotDays = 0,
                Currency = Currency.EUR,
                Calendar = CalendarName.WeekendsOnly,
                DayCountBasis = DayCounter.Actual365Fixed,
                DepositCalendar = CalendarName.TARGET,
                DepositRollConvention = BusinessDayConvention.ModifiedFollowing,
                DepositUseEndOfMonth = false,
                DepositBasis = DayCounter.Actual360,
                DepositRates = new[]
                {
                    (new Period(1, TimeUnit.Months), 0.000060),
                    (new Period(2, TimeUnit.Months), 0.000450),
                    (new Period(3, TimeUnit.Months), 0.000810),
                    (new Period(6, TimeUnit.Months), 0.001840),
                    (new Period(9, TimeUnit.Months), 0.002560),
                    (new Period(12, TimeUnit.Months), 0.003370)
                },
                SwapCalendar = CalendarName.TARGET,
                SwapFixedFrequency = Frequency.Annual,
                SwapFixedRollConvention = BusinessDayConvention.ModifiedFollowing,
                SwapFixedBasis = DayCounter.Thirty360,
                SwapFloatIndex = "EUR-EURIBOR-6M",
                SwapRates = new[]
                {
                    (new Period(2, TimeUnit.Years), 0.002230),
                    (new Period(3, TimeUnit.Years), 0.002760),
                    (new Period(4, TimeUnit.Years), 0.003530),
                    (new Period(5, TimeUnit.Years), 0.004520),
                    (new Period(6, TimeUnit.Years), 0.005720),
                    (new Period(7, TimeUnit.Years), 0.007050),
                    (new Period(8, TimeUnit.Years), 0.008420),
                    (new Period(9, TimeUnit.Years), 0.009720),
                    (new Period(10, TimeUnit.Years), 0.010900),
                    (new Period(12, TimeUnit.Years), 0.012870),
                    (new Period(15, TimeUnit.Years), 0.014970),
                    (new Period(20, TimeUnit.Years), 0.017000),
                    (new Period(30, TimeUnit.Years), 0.018210),
                }
            };
            var discountCurve = new YieldTermStructure(builder);

            // output rate curve
            Console.WriteLine("ISDA rate Curve:");
            foreach (var helper in builder.RateHelpers)
            {
                var zero = discountCurve.ZeroRate(helper.PillarDate);
                var discount = discountCurve.Discount(helper.PillarDate);
                Console.WriteLine($"{helper.PillarDate:d}\t{zero}\t{discount}");
            }

            var defProbBuilder = new DefaultCurvePiecewiseLogLinearSurvival(todaysDate)
            {
                SpotDays = 0,
                Calendar = CalendarName.WeekendsOnly,
                DayCountBasis = DayCounter.Actual365Fixed,
                Currency = Currency.EUR,
                CdsQuotes = new[] {(new Period(54, TimeUnit.Months), CdsQuoteType.Spread, 0.00672658551)},
                CdsQuoteRecoveryRate = 0.4,
                DiscountCurve = discountCurve
            };
            var defProbTs = new DefaultProbabilityTermStructure(defProbBuilder);
            //foreach (var helper in defProbBuilder.RateHelpers)
            //{
            //    var zero = discountCurve.ZeroRate(helper.PillarDate);
            //    var discount = discountCurve.Discount(helper.PillarDate);
            //    Console.WriteLine($"{helper.PillarDate:d}\t{zero}\t{discount}");
            //}
            DateTime lookAtDate = new DateTime(2020, 9, 23);
            double defProb = defProbTs.DefaultProbability(lookAtDate, true);
            double survProb = defProbTs.SurvivalProbability(lookAtDate, true);
            double hazard = defProbTs.HazardRate(lookAtDate, true);
            Console.WriteLine("ISDA credit curve:");
            Console.WriteLine($"{lookAtDate};{defProb};{survProb};{hazard}");

            defProbTs.AllowsExtrapolation = true;
            var cdsBuilder = new CreditDefaultSwap.Builder("TEST", Currency.EUR, "SNRFOR", "MM", new Quote(0.03, QuoteType.Bps))
            {
                Tenor = new Period(5, TimeUnit.Years),
                Notional = 100000000.0
            };
            var cds = new CreditDefaultSwap(cdsBuilder) {PricingEngine = new IsdaCdsEngine(defProbTs, 0.4, discountCurve)};
            var npv = cds.Npv;

            Console.WriteLine("Pricing of example trade with ISDA engine:");
            Console.WriteLine($"NPV = {npv:N}");

            DateTime endTime = DateTime.Now;
            TimeSpan delta = endTime - startTime;
            Console.WriteLine("\nRun completed in {0} s", delta.TotalSeconds);
            Console.WriteLine();
        }
    }
}