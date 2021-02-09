/*
 Copyright (C) 2007 Eric H. Jensen

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

namespace EquityOption
{
    internal class EquityOption
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;

            var optionType = Option.Type.Put;
            double underlyingPrice = 36;
            double strikePrice = 40;
            double dividendYield = 0.0;
            double riskFreeRate = 0.06;
            double volatility = 0.2;

            var todaysDate = new Date(15, Month.May, 1998);
            Settings.instance().setEvaluationDate(todaysDate);

            var settlementDate = new Date(17, Month.May, 1998);
            var maturityDate = new Date(17, Month.May, 1999);

            Calendar calendar = new TARGET();

            var exerciseDates = new DateVector(4);
            for (int i = 1; i <= 4; i++)
            {
                var forwardPeriod = new Period(3 * i, TimeUnit.Months);
                Date forwardDate = settlementDate.Add(forwardPeriod);
                exerciseDates.Add(forwardDate);
            }

            var europeanExercise = new EuropeanExercise(maturityDate);
            var bermudanExercise = new BermudanExercise(exerciseDates);
            var americanExercise = new AmericanExercise(settlementDate, maturityDate);

            // bootstrap the yield/dividend/vol curves and create a BlackScholesMerton stochastic process
            DayCounter dayCounter = new Actual365Fixed();
            var flatRateTSH = new YieldTermStructureHandle(new FlatForward(settlementDate, riskFreeRate, dayCounter));
            var flatDividendTSH = new YieldTermStructureHandle(new FlatForward(settlementDate, dividendYield, dayCounter));
            var flatVolTSH = new BlackVolTermStructureHandle(new BlackConstantVol(settlementDate, calendar, volatility, dayCounter));
            var underlyingQuoteH = new QuoteHandle(new SimpleQuote(underlyingPrice));
            var stochasticProcess = new BlackScholesMertonProcess(underlyingQuoteH, flatDividendTSH, flatRateTSH, flatVolTSH);
            var payoff = new PlainVanillaPayoff(optionType, strikePrice);

            // options
            var europeanOption = new VanillaOption(payoff, europeanExercise);
            var bermudanOption = new VanillaOption(payoff, bermudanExercise);
            var americanOption = new VanillaOption(payoff, americanExercise);

            // report the parameters we are using
            ReportParameters(optionType, underlyingPrice, strikePrice, dividendYield, riskFreeRate, volatility, maturityDate);

            // write out the column headings
            ReportHeadings();

            #region Analytic Formulas

            // Black-Scholes for European
            try
            {
                europeanOption.setPricingEngine(new AnalyticEuropeanEngine(stochasticProcess));
                ReportResults("Black-Scholes", europeanOption.NPV(), null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            // Barone-Adesi and Whaley approximation for American
            try
            {
                americanOption.setPricingEngine(new BaroneAdesiWhaleyEngine(stochasticProcess));
                ReportResults("Barone-Adesi/Whaley", null, null, americanOption.NPV());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            // Bjerksund and Stensland approximation for American
            try
            {
                americanOption.setPricingEngine(new BjerksundStenslandEngine(stochasticProcess));
                ReportResults("Bjerksund/Stensland", null, null, americanOption.NPV());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            // Integral
            try
            {
                europeanOption.setPricingEngine(new IntegralEngine(stochasticProcess));
                ReportResults("Integral", europeanOption.NPV(), null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            uint timeSteps = 801;

            // Finite differences
            try
            {
                europeanOption.setPricingEngine(new FdBlackScholesVanillaEngine(stochasticProcess, timeSteps, timeSteps - 1));
                bermudanOption.setPricingEngine(new FdBlackScholesVanillaEngine(stochasticProcess, timeSteps, timeSteps - 1));
                americanOption.setPricingEngine(new FdBlackScholesVanillaEngine(stochasticProcess, timeSteps, timeSteps - 1));
                ReportResults("Finite differences", europeanOption.NPV(), bermudanOption.NPV(), americanOption.NPV());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            //Variance Gamma
            try
            {
                var vgProcess = new VarianceGammaProcess(underlyingQuoteH, flatDividendTSH, flatRateTSH, volatility, 0.01, 0.0);
                europeanOption.setPricingEngine(new VarianceGammaEngine(vgProcess));
                ReportResults("Variance-Gamma", europeanOption.NPV(), null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            #endregion Analytic Formulas

            #region Binomial Methods

            // Binomial Jarrow-Rudd
            try
            {
                europeanOption.setPricingEngine(new BinomialJRVanillaEngine(stochasticProcess, timeSteps));
                bermudanOption.setPricingEngine(new BinomialJRVanillaEngine(stochasticProcess, timeSteps));
                americanOption.setPricingEngine(new BinomialJRVanillaEngine(stochasticProcess, timeSteps));
                ReportResults("Binomial Jarrow-Rudd", europeanOption.NPV(), bermudanOption.NPV(), americanOption.NPV());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            // Binomial Cox-Ross-Rubinstein
            try
            {
                europeanOption.setPricingEngine(new BinomialCRRVanillaEngine(stochasticProcess, timeSteps));
                bermudanOption.setPricingEngine(new BinomialCRRVanillaEngine(stochasticProcess, timeSteps));
                americanOption.setPricingEngine(new BinomialCRRVanillaEngine(stochasticProcess, timeSteps));
                ReportResults("Binomial Cox-Ross-Rubinstein", europeanOption.NPV(), bermudanOption.NPV(), americanOption.NPV());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            // Additive Equiprobabilities
            try
            {
                europeanOption.setPricingEngine(new BinomialEQPVanillaEngine(stochasticProcess, timeSteps));
                bermudanOption.setPricingEngine(new BinomialEQPVanillaEngine(stochasticProcess, timeSteps));
                americanOption.setPricingEngine(new BinomialEQPVanillaEngine(stochasticProcess, timeSteps));
                ReportResults("Additive Equiprobabilities", europeanOption.NPV(), bermudanOption.NPV(), americanOption.NPV());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            // Binomial Trigeorgis
            try
            {
                europeanOption.setPricingEngine(new BinomialTrigeorgisVanillaEngine(stochasticProcess, timeSteps));
                bermudanOption.setPricingEngine(new BinomialTrigeorgisVanillaEngine(stochasticProcess, timeSteps));
                americanOption.setPricingEngine(new BinomialTrigeorgisVanillaEngine(stochasticProcess, timeSteps));
                ReportResults("Binomial Trigeorgis", europeanOption.NPV(), bermudanOption.NPV(), americanOption.NPV());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            // Binomial Tian
            try
            {
                europeanOption.setPricingEngine(new BinomialTianVanillaEngine(stochasticProcess, timeSteps));
                bermudanOption.setPricingEngine(new BinomialTianVanillaEngine(stochasticProcess, timeSteps));
                americanOption.setPricingEngine(new BinomialTianVanillaEngine(stochasticProcess, timeSteps));
                ReportResults("Binomial Tian", europeanOption.NPV(), bermudanOption.NPV(), americanOption.NPV());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            // Binomial Leisen-Reimer
            try
            {
                europeanOption.setPricingEngine(new BinomialLRVanillaEngine(stochasticProcess, timeSteps));
                bermudanOption.setPricingEngine(new BinomialLRVanillaEngine(stochasticProcess, timeSteps));
                americanOption.setPricingEngine(new BinomialLRVanillaEngine(stochasticProcess, timeSteps));
                ReportResults("Binomial Leisen-Reimer", europeanOption.NPV(), bermudanOption.NPV(), americanOption.NPV());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            // Binomial Joshi
            try
            {
                europeanOption.setPricingEngine(new BinomialJ4VanillaEngine(stochasticProcess, timeSteps));
                bermudanOption.setPricingEngine(new BinomialJ4VanillaEngine(stochasticProcess, timeSteps));
                americanOption.setPricingEngine(new BinomialJ4VanillaEngine(stochasticProcess, timeSteps));
                ReportResults("Binomial Joshi", europeanOption.NPV(), bermudanOption.NPV(), americanOption.NPV());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            #endregion Binomial Methods

            #region Monte Carlo Methods

            // quantlib appears to use max numeric (int and real) values to test for 'null' (or rather 'default') values

            // MC (crude)
            try
            {
                int mcTimeSteps = 1;
                int timeStepsPerYear = int.MaxValue;
                bool brownianBridge = false;
                bool antitheticVariate = false;
                int requiredSamples = int.MaxValue;
                double requiredTolerance = 0.02;
                int maxSamples = int.MaxValue;
                int seed = 42;
                europeanOption.setPricingEngine(new MCPREuropeanEngine(stochasticProcess, mcTimeSteps, timeStepsPerYear, brownianBridge, antitheticVariate, requiredSamples,
                    requiredTolerance, maxSamples, seed));
                ReportResults("MC (crude)", europeanOption.NPV(), null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            // MC (Sobol)
            try
            {
                int mcTimeSteps = 1;
                int timeStepsPerYear = int.MaxValue;
                bool brownianBridge = false;
                bool antitheticVariate = false;
                int requiredSamples = 32768; // 2^15
                double requiredTolerance = double.MaxValue;
                int maxSamples = int.MaxValue;
                int seed = 0;
                europeanOption.setPricingEngine(new MCLDEuropeanEngine(stochasticProcess, mcTimeSteps, timeStepsPerYear, brownianBridge, antitheticVariate, requiredSamples,
                    requiredTolerance, maxSamples, seed));
                ReportResults("MC (Sobol)", europeanOption.NPV(), null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            #endregion Monte Carlo Methods

            DateTime endTime = DateTime.Now;
            TimeSpan delta = endTime - startTime;
            Console.WriteLine();
            Console.WriteLine("Run completed in {0} s", delta.TotalSeconds);
            Console.WriteLine();
        }

        private static void ReportParameters(Option.Type optionType, double underlyingPrice, double strikePrice, double dividendYield, double riskFreeRate, double volatility,
                                             Date maturityDate)
        {
            Console.WriteLine();
            Console.WriteLine("Option type = {0}", optionType);
            Console.WriteLine("Maturity = {0} {1} {2}", maturityDate.year(), maturityDate.month(), maturityDate.dayOfMonth());
            Console.WriteLine("Underlying price = ${0}", underlyingPrice);
            Console.WriteLine("Strike = ${0}", strikePrice);
            Console.WriteLine("Risk-free interest rate = {0}%", riskFreeRate * 100.0);
            Console.WriteLine("Dividend yield = {0}%", dividendYield * 100.0);
            Console.WriteLine("Volatility = {0}%", volatility * 100);
            Console.WriteLine();
        }

        private static int[] columnWidths = {35, 14, 14, 14};

        private static void ReportHeadings()
        {
            Console.Write("Method".PadRight(columnWidths[0]));
            Console.Write("European".PadRight(columnWidths[1]));
            Console.Write("Bermudan".PadRight(columnWidths[2]));
            Console.Write("American".PadRight(columnWidths[3]));
            Console.WriteLine();
        }

        private static void ReportResults(string methodName, double? european, double? bermudan, double? american)
        {
            string strNA = "N/A";
            string format = "{0:N6}";
            Console.Write(methodName.PadRight(columnWidths[0]));
            Console.Write(string.Format((european == null) ? strNA : format, european).PadRight(columnWidths[1]));
            Console.Write(string.Format((bermudan == null) ? strNA : format, bermudan).PadRight(columnWidths[2]));
            Console.Write(string.Format((american == null) ? strNA : format, american).PadRight(columnWidths[3]));
            Console.WriteLine();
        }
    }
}