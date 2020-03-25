using System;
using System.Linq;
using QuantLib;

namespace FxForwardValuation
{
    class Program
    {
        private static readonly Currency EUR_CCY = new EURCurrency();
        private static readonly Currency USD_CCY = new USDCurrency();
        private static readonly Currency GBP_CCY = new GBPCurrency();
        private static readonly Currency CHF_CCY = new CHFCurrency();

        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;

            var todaysDate = new DateTime(2020, 2, 28);
            Settings.instance().setEvaluationDate(todaysDate);
            Money.setConversionType(Money.ConversionType.AutomatedConversion);
            Console.WriteLine($"Today: {todaysDate:D}\n");

            ExchangeRateManager.instance().add(new ExchangeRate(USD_CCY, EUR_CCY, 0.9103736341), todaysDate);
            ExchangeRateManager.instance().add(new ExchangeRate(GBP_CCY, EUR_CCY, 1.1628202171), todaysDate);
            ExchangeRateManager.instance().add(new ExchangeRate(CHF_CCY, EUR_CCY, 0.9405171323), todaysDate);

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
            var baseNotionalAmount = new Money(12925000, USD_CCY);
            var contractAllInRate = new ExchangeRate(USD_CCY, EUR_CCY, 0.897487215294618);

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
            Money contractTermNotional = fxFwd.contractNotionalAmountTerm();
            Money forwardTermGross = fxFwd.forwardGrossValueTerm();
            Money forwardNetValue = fxFwd.forwardNetValueTerm();
            Money presentNetValue = fxFwd.presentNetValueTerm();
            Console.WriteLine($"Fair forward points: {fxFwd.fairForwardPoints()}");
            Console.WriteLine($"Forward base leg value: {contractTermNotional}");
            Console.WriteLine($"Forward term leg value: {forwardTermGross}");
            Console.WriteLine($"Forward net value: {forwardNetValue}");
            Console.WriteLine($"Present net value: {presentNetValue}");
        }

        private static FxForwardPointTermStructureHandle UsdEurFwdPointStructure(DateTime todaysDate)
        {
            Calendar calendar = new JointCalendar(new TARGET(), new UnitedStates(UnitedStates.Market.FederalReserve));
            DayCounter dayCounter = new Actual360();
            ExchangeRate spotExchRate = ExchangeRateManager.instance().lookup(USD_CCY, EUR_CCY, todaysDate);
            if (spotExchRate.source() != USD_CCY)
                spotExchRate = ExchangeRate.inverse(spotExchRate);

            var fwdExchRates = new ForwardExchangeRateVector
            {
                new ForwardExchangeRate(spotExchRate, -4.051701, new Period(1, TimeUnit.Weeks)),
                new ForwardExchangeRate(spotExchRate, -7.906924, new Period(2, TimeUnit.Weeks)),
                new ForwardExchangeRate(spotExchRate, -11.743311, new Period(3, TimeUnit.Weeks)),
                new ForwardExchangeRate(spotExchRate, -17.395392, new Period(1, TimeUnit.Months)),
                new ForwardExchangeRate(spotExchRate, -33.074375, new Period(2, TimeUnit.Months)),
                new ForwardExchangeRate(spotExchRate, -47.207796, new Period(3, TimeUnit.Months))
            };

            var fwdPtCurve = new InterpolatedFxForwardPointTermStructureLinear(todaysDate, fwdExchRates, dayCounter, calendar);
            return new FxForwardPointTermStructureHandle(fwdPtCurve);
        }

        private static FxForwardPointTermStructureHandle EurUsdFwdPointStructure(DateTime todaysDate)
        {
            Calendar calendar = new JointCalendar(new TARGET(), new UnitedStates(UnitedStates.Market.FederalReserve));
            DayCounter dayCounter = new Actual360();
            ExchangeRate spotExchRate = ExchangeRateManager.instance().lookup(EUR_CCY, USD_CCY, todaysDate);
            if (spotExchRate.source() != EUR_CCY)
                spotExchRate = ExchangeRate.inverse(spotExchRate);

            var fwdExchRates = new ForwardExchangeRateVector
            {
                new ForwardExchangeRate(spotExchRate, 4.9, new Period(1, TimeUnit.Weeks)),
                new ForwardExchangeRate(spotExchRate, 9.625, new Period(2, TimeUnit.Weeks)),
                new ForwardExchangeRate(spotExchRate, 14.305, new Period(3, TimeUnit.Weeks)),
                new ForwardExchangeRate(spotExchRate, 21.155, new Period(1, TimeUnit.Months)),
                new ForwardExchangeRate(spotExchRate, 40.669, new Period(2, TimeUnit.Months)),
                new ForwardExchangeRate(spotExchRate, 57.975, new Period(3, TimeUnit.Months))
            };

            FxForwardPointTermStructure fwdPtCurve = new InterpolatedFxForwardPointTermStructureLinear(todaysDate, fwdExchRates, dayCounter, calendar);
            return new FxForwardPointTermStructureHandle(fwdPtCurve);
        }

        private static FxForwardPointTermStructureHandle GbpEurFwdPointStructure(DateTime todaysDate)
        {
            Calendar calendar = new JointCalendar(new TARGET(), new UnitedKingdom(UnitedKingdom.Market.Settlement));
            DayCounter dayCounter = new Actual360();
            ExchangeRate spotExchRate = ExchangeRateManager.instance().lookup(GBP_CCY, EUR_CCY, todaysDate);
            if (spotExchRate.source() != GBP_CCY)
                spotExchRate = ExchangeRate.inverse(spotExchRate);

            var fwdExchRates = new ForwardExchangeRateVector
            {
                new ForwardExchangeRate(spotExchRate, -2.8, new Period(1, TimeUnit.Weeks)),
                //new ForwardExchangeRate(spotExchRate, -6.91,new Period(2, TimeUnit.Weeks)),
                //new ForwardExchangeRate(spotExchRate, -9.74, new Period(3, TimeUnit.Weeks)),
                new ForwardExchangeRate(spotExchRate, -12.13, new Period(1, TimeUnit.Months)),
                new ForwardExchangeRate(spotExchRate, -24.16, new Period(2, TimeUnit.Months)),
                new ForwardExchangeRate(spotExchRate, -34.99, new Period(3, TimeUnit.Months))
            };

            FxForwardPointTermStructure fwdPtCurve = new InterpolatedFxForwardPointTermStructureLinear(todaysDate, fwdExchRates, dayCounter, calendar);
            return new FxForwardPointTermStructureHandle(fwdPtCurve);
        }

        private static FxForwardPointTermStructureHandle EurGbpFwdPointStructure(DateTime todaysDate)
        {
            Calendar calendar = new JointCalendar(new TARGET(), new UnitedKingdom(UnitedKingdom.Market.Settlement));
            DayCounter dayCounter = new Actual360();
            ExchangeRate spotExchRate = ExchangeRateManager.instance().lookup(EUR_CCY, GBP_CCY, todaysDate);
            if (spotExchRate.source() != EUR_CCY)
                spotExchRate = ExchangeRate.inverse(spotExchRate);

            var fwdExchRates = new ForwardExchangeRateVector
            {
                new ForwardExchangeRate(spotExchRate, 2.06, new Period(1, TimeUnit.Weeks)),
                new ForwardExchangeRate(spotExchRate, 4.01, new Period(2, TimeUnit.Weeks)),
                new ForwardExchangeRate(spotExchRate, 6.19, new Period(3, TimeUnit.Weeks)),
                new ForwardExchangeRate(spotExchRate, 8.98, new Period(1, TimeUnit.Months)),
                new ForwardExchangeRate(spotExchRate, 17.85, new Period(2, TimeUnit.Months)),
                new ForwardExchangeRate(spotExchRate, 25.97, new Period(3, TimeUnit.Months))
            };

            FxForwardPointTermStructure fwdPtCurve = new InterpolatedFxForwardPointTermStructureLinear(todaysDate, fwdExchRates, dayCounter, calendar);
            return new FxForwardPointTermStructureHandle(fwdPtCurve);
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

    public class TestEquality : IEquatable<TestEquality>
    {
        private readonly string _value;

        public TestEquality(string value)
        {
            _value = value;
        }

        #region Equality members

        public bool Equals(TestEquality other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestEquality)obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(TestEquality left, TestEquality right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TestEquality left, TestEquality right)
        {
            return !Equals(left, right);
        }

        #endregion
    }

}