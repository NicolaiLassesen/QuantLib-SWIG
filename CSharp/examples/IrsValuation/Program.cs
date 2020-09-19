using System;
using CfAnalytics;
using CfAnalytics.QuantLib;
using CfAnalytics.QuantLib.PricingEngines.Credit;
using CfAnalytics.QuantLib.TermStructures.DefaultProbabilityTermStructures;
using CfAnalytics.QuantLib.TermStructures.YieldTermStructures;
using QuantLib;
using BusinessDayConvention = CfAnalytics.BusinessDayConvention;
using Currency = CfAnalytics.Currency;
using DayCounter = CfAnalytics.QuantLib.DayCounter;
using DefaultProbabilityTermStructure = CfAnalytics.QuantLib.TermStructures.DefaultProbabilityTermStructure;
using DiscountingSwapEngine = CfAnalytics.QuantLib.PricingEngines.Ir.DiscountingSwapEngine;
using Frequency = CfAnalytics.Frequency;
using IborIndex = CfAnalytics.QuantLib.IborIndex;
using IndexManager = CfAnalytics.QuantLib.IndexManager;
using Period = CfAnalytics.QuantLib.Period;
using Settings = CfAnalytics.QuantLib.Settings;
using TimeUnit = CfAnalytics.TimeUnit;
using VanillaSwap = CfAnalytics.QuantLib.Instruments.VanillaSwap;
using YieldTermStructure = CfAnalytics.QuantLib.TermStructures.YieldTermStructure;

// ReSharper disable InconsistentNaming

namespace CdsValuation
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;

            var todaysDate = new DateTime(2020, 9, 17);
            Settings.EvaluationDate = todaysDate;
            Console.WriteLine($"Today: {todaysDate:D}\n");

            var builder = new SwapCurveBootstrapBuilder(todaysDate)
            {
                SpotDays = 0,
                Currency = Currency.EUR,
                Calendar = CalendarName.TARGET,
                DayCountBasis = DayCounter.Actual365Fixed,
                DepositCalendar = CalendarName.TARGET,
                DepositRollConvention = BusinessDayConvention.ModifiedFollowing,
                DepositUseEndOfMonth = false,
                DepositBasis = DayCounter.Actual360,
                DepositRates = new[]
                {
                    (new Period(1, TimeUnit.Months), -0.00526),
                    (new Period(3, TimeUnit.Months), -0.00487),
                    (new Period(6, TimeUnit.Months), -0.00462),
                    (new Period(12, TimeUnit.Months), -0.00419)
                },
                SwapCalendar = CalendarName.TARGET,
                SwapFixedFrequency = Frequency.Annual,
                SwapFixedRollConvention = BusinessDayConvention.ModifiedFollowing,
                SwapFixedBasis = DayCounter.Thirty360,
                SwapFloatIndex = "EUR-EURIBOR-6M",
                SwapRates = new[]
                {
                    (new Period(2, TimeUnit.Years), -0.004835),
                    (new Period(3, TimeUnit.Years), -0.00476),
                    (new Period(4, TimeUnit.Years), -0.00456),
                    (new Period(5, TimeUnit.Years), -0.00428),
                    (new Period(6, TimeUnit.Years), -0.00393),
                    (new Period(7, TimeUnit.Years), -0.00358),
                    (new Period(8, TimeUnit.Years), -0.00317),
                    (new Period(9, TimeUnit.Years), -0.00274),
                    (new Period(10, TimeUnit.Years), -0.002298),
                    (new Period(12, TimeUnit.Years), -0.00147),
                    (new Period(15, TimeUnit.Years), -0.0004285),
                    (new Period(20, TimeUnit.Years), 0.00033),
                    (new Period(30, TimeUnit.Years), 0.00003)
                }
            };
            var swapCurve = new YieldTermStructure(builder);

            // output rate curve
            Console.WriteLine("Swap Curve:");
            foreach (var helper in builder.RateHelpers)
            {
                var zero = swapCurve.ZeroRate(helper.PillarDate);
                var discount = swapCurve.Discount(helper.PillarDate);
                Console.WriteLine($"{helper.PillarDate:d}\t{zero}\t{discount}");
            }

            var idx = IborIndex.GetIborIndex(Currency.EUR, "EURIBOR", "6M", swapCurve);
            var irsBuilder = new VanillaSwap.Builder(new DateTime(2020, 9, 10), new DateTime(2024, 9, 10))
            {
                Type = VanillaSwap.Type.Payer,
                Notional = 1000000.0,
                Calendar = CalendarName.TARGET,
                FixedRate = -0.00449,
                FixedFrequency = Frequency.Annual,
                FixedConvention = BusinessDayConvention.ModifiedFollowing,
                FixedDaycount = DayCounter.Thirty360,
                FloatFrequency = Frequency.SemiAnnual,
                FloatConvention = BusinessDayConvention.ModifiedFollowing,
                FloatDaycount = DayCounter.Actual360,
                FloatIndex = idx
            };
            var irs = new VanillaSwap(irsBuilder) {PricingEngine = new DiscountingSwapEngine(swapCurve)};

            idx.AddFixing(new DateTime(2020, 9, 8), -0.00459);
            var npv = irs.Npv;

            Console.WriteLine("\nPricing of IRS with simple discounting engine:");
            Console.WriteLine($"NPV = {npv:N}");
        }
    }
}