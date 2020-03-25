/*
 Copyright (C) 2005 Dominic Thuillier

 This file is part of QuantLib, a free-software/open-source library
 for financial quantitative analysts and developers - http://quantlib.org/

 QuantLib is free software: you can redistribute it and/or modify it
 under the terms of the QuantLib license.  You should have received a
 copy of the license along with this program; if not, please email
 <quantlib-dev@lists.sf.net>. The license is also available online at
 <http://quantlib.org/license.shtml>.

 This program is distributed in the hope that it will be useful, but WITHOUT
 ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 FOR A PARTICULAR PURPOSE.  See the license for more details.
*/

using System;
using QuantLib;

namespace BermudanSwaption
{
    internal class Program
    {
        private const int NUM_ROWS = 5;
        private const int NUM_COLS = 5;

        private static readonly int[] SWAP_LENGHTS = {1, 2, 3, 4, 5};

        private static readonly double[] SWAPTION_VOLS =
        {
            0.1490, 0.1340, 0.1228, 0.1189, 0.1148,
            0.1290, 0.1201, 0.1146, 0.1108, 0.1040,
            0.1149, 0.1112, 0.1070, 0.1010, 0.0957,
            0.1047, 0.1021, 0.0980, 0.0951, 0.1270,
            0.1000, 0.0950, 0.0900, 0.1230, 0.1160
        };

        private static void CalibrateModel(ShortRateModel model, CalibrationHelperVector helpers, double lambda)
        {
            var om = new Simplex(lambda);
            model.calibrate(helpers, om, new EndCriteria(1000, 250, 1e-7, 1e-7, 1e-7));

            // Output the implied Black volatilities
            for (int i = 0; i < NUM_ROWS; i++)
            {
                int j = NUM_COLS - i - 1; // 1x5, 2x4, 3x3, 4x2, 5x1
                int k = i * NUM_COLS + j;
                double npv = NQuantLibc.as_black_helper(helpers[i]).modelValue();
                double implied = NQuantLibc.as_black_helper(helpers[i]).impliedVolatility(npv, 1e-4, 1000, 0.05, 0.50);
                double diff = implied - SWAPTION_VOLS[k];

                Console.WriteLine("{0}x{1}: model {2}, market {3} ({4})", i + 1, SWAP_LENGHTS[j], implied, SWAPTION_VOLS[k], diff);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            DateTime startTime = DateTime.Now;

            var todaysDate = new DateTime(2002, 2, 15);
            Settings.instance().setEvaluationDate(todaysDate);

            Calendar calendar = new TARGET();
            var settlementDate = new Date(19, Month.February, 2002);

            // flat yield term structure impling 1x5 swap at 5%
            Quote flatRate = new SimpleQuote(0.04875825);
            var myTermStructure = new FlatForward(settlementDate, new QuoteHandle(flatRate), new Actual365Fixed());
            var rhTermStructure = new RelinkableYieldTermStructureHandle();
            rhTermStructure.linkTo(myTermStructure);

            // Define the ATM/OTM/ITM swaps
            var fixedLegTenor = new Period(1, TimeUnit.Years);
            const BusinessDayConvention fixedLegConvention = BusinessDayConvention.Unadjusted;
            const BusinessDayConvention floatingLegConvention = BusinessDayConvention.ModifiedFollowing;
            DayCounter fixedLegDayCounter = new Thirty360(Thirty360.Convention.European);
            var floatingLegTenor = new Period(6, TimeUnit.Months);
            const double dummyFixedRate = 0.03;
            IborIndex indexSixMonths = new Euribor6M(rhTermStructure);

            Date startDate = calendar.advance(settlementDate, 1, TimeUnit.Years, floatingLegConvention);
            Date maturity = calendar.advance(startDate, 5, TimeUnit.Years, floatingLegConvention);
            var fixedSchedule = new Schedule(startDate, maturity, fixedLegTenor, calendar, fixedLegConvention, fixedLegConvention, DateGeneration.Rule.Forward, false);
            var floatSchedule = new Schedule(startDate, maturity, floatingLegTenor, calendar, floatingLegConvention, floatingLegConvention, DateGeneration.Rule.Forward, false);
            var swap = new VanillaSwap(VanillaSwap.Type.Payer, 1000.0,
                fixedSchedule, dummyFixedRate, fixedLegDayCounter,
                floatSchedule, indexSixMonths, 0.0, indexSixMonths.dayCounter());
            var swapEngine = new DiscountingSwapEngine(rhTermStructure);
            swap.setPricingEngine(swapEngine);
            double fixedAtmRate = swap.fairRate();
            double fixedOtmRate = fixedAtmRate * 1.2;
            double fixedItmRate = fixedAtmRate * 0.8;

            var atmSwap = new VanillaSwap(VanillaSwap.Type.Payer, 1000.0,
                fixedSchedule, fixedAtmRate, fixedLegDayCounter,
                floatSchedule, indexSixMonths, 0.0,
                indexSixMonths.dayCounter());
            var otmSwap = new VanillaSwap(VanillaSwap.Type.Payer, 1000.0,
                fixedSchedule, fixedOtmRate, fixedLegDayCounter,
                floatSchedule, indexSixMonths, 0.0,
                indexSixMonths.dayCounter());
            var itmSwap = new VanillaSwap(VanillaSwap.Type.Payer, 1000.0,
                fixedSchedule, fixedItmRate, fixedLegDayCounter,
                floatSchedule, indexSixMonths, 0.0,
                indexSixMonths.dayCounter());
            atmSwap.setPricingEngine(swapEngine);
            otmSwap.setPricingEngine(swapEngine);
            itmSwap.setPricingEngine(swapEngine);

            // defining the swaptions to be used in model calibration
            var swaptionMaturities = new PeriodVector
            {
                new Period(1, TimeUnit.Years),
                new Period(2, TimeUnit.Years),
                new Period(3, TimeUnit.Years),
                new Period(4, TimeUnit.Years),
                new Period(5, TimeUnit.Years)
            };

            var swaptions = new CalibrationHelperVector();

            // List of times that have to be included in the timegrid
            var times = new DoubleVector();

            for (int i = 0; i < NUM_ROWS; i++)
            {
                int j = NUM_COLS - i - 1; // 1x5, 2x4, 3x3, 4x2, 5x1
                int k = i * NUM_COLS + j;
                Quote vol = new SimpleQuote(SWAPTION_VOLS[k]);
                var helper = new SwaptionHelper(swaptionMaturities[i], new Period(SWAP_LENGHTS[j], TimeUnit.Years),
                    new QuoteHandle(vol),
                    indexSixMonths, indexSixMonths.tenor(),
                    indexSixMonths.dayCounter(),
                    indexSixMonths.dayCounter(),
                    rhTermStructure);
                swaptions.Add(helper);
                times.AddRange(helper.times());
            }

            // Building time-grid
            var grid = new TimeGrid(times, 30);

            // defining the models
            // G2 modelG2 = new G2(rhTermStructure));
            var modelHw = new HullWhite(rhTermStructure);
            var modelHw2 = new HullWhite(rhTermStructure);
            var modelBk = new BlackKarasinski(rhTermStructure);

            // model calibrations
            Console.WriteLine("Hull-White (analytic formulae) calibration");
            foreach (CalibrationHelper calibrationHelper in swaptions)
                NQuantLibc.as_black_helper(calibrationHelper).setPricingEngine(new JamshidianSwaptionEngine(modelHw));
            CalibrateModel(modelHw, swaptions, 0.05);

            Console.WriteLine("Hull-White (numerical) calibration");
            foreach (CalibrationHelper calibrationHelper in swaptions)
                NQuantLibc.as_black_helper(calibrationHelper).setPricingEngine(new TreeSwaptionEngine(modelHw2, grid));
            CalibrateModel(modelHw2, swaptions, 0.05);

            Console.WriteLine("Black-Karasinski (numerical) calibration");
            foreach (CalibrationHelper calibrationHelper in swaptions)
                NQuantLibc.as_black_helper(calibrationHelper).setPricingEngine(new TreeSwaptionEngine(modelBk, grid));
            CalibrateModel(modelBk, swaptions, 0.05);

            // ATM Bermudan swaption pricing
            Console.WriteLine("Payer bermudan swaption struck at {0} (ATM)", fixedAtmRate);

            var bermudanDates = new DateVector();
            var schedule = new Schedule(startDate, maturity,
                new Period(3, TimeUnit.Months), calendar,
                BusinessDayConvention.Following,
                BusinessDayConvention.Following,
                DateGeneration.Rule.Forward, false);

            for (uint i = 0; i < schedule.size(); i++)
                bermudanDates.Add(schedule.date(i));
            Exercise bermudaExercise = new BermudanExercise(bermudanDates);

            var bermudanSwaption = new Swaption(atmSwap, bermudaExercise);
            bermudanSwaption.setPricingEngine(new TreeSwaptionEngine(modelHw, 50));
            Console.WriteLine("HW: " + bermudanSwaption.NPV());

            bermudanSwaption.setPricingEngine(new TreeSwaptionEngine(modelHw2, 50));
            Console.WriteLine("HW (num): " + bermudanSwaption.NPV());

            bermudanSwaption.setPricingEngine(new TreeSwaptionEngine(modelBk, 50));
            Console.WriteLine("BK (num): " + bermudanSwaption.NPV());

            DateTime endTime = DateTime.Now;
            TimeSpan delta = endTime - startTime;
            Console.WriteLine();
            Console.WriteLine("Run completed in {0} s", delta.TotalSeconds);
            Console.WriteLine();
        }
    }
}