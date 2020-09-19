using System;
using CfAnalytics.QuantLib.InternalUtils;
using CfAnalytics.Utilities;
using QlFxFwd = QuantLib.ForeignExchangeForward;

namespace CfAnalytics.QuantLib.Instruments
{
    public class ForeignExchangeForward : Instrument<FxFwdImpl>
    {
        public enum Type
        {
            SellBaseBuyTermForward,
            BuyBaseSellTermForward
        }

        public ForeignExchangeForward(DateTime deliveryDate, Money baseNotional, ExchangeRate contractAllInRate)
            : base(new FxFwdImpl(new QlFxFwd(deliveryDate, baseNotional.QlObj, contractAllInRate.QlObj)))
        {
        }

        public DateTime DeliveryDate => Impl.QlObj.deliveryDate().AsDateTime();
        public ExchangeRate ContractAllInRate => new ExchangeRate(Impl.QlObj.contractAllInRate());
        public Currency BaseCurrency => Impl.QlObj.baseCurrency().ToCfCurrency();
        public Currency QuoteCurrency => Impl.QlObj.termCurrency().ToCfCurrency();
        public Type ForwardType => EnumUtils.GetEnumValue<Type>(Impl.QlObj.forwardType().ToString());

        //QlObj.foreignExchangeTerms().settlementDays();
        //QlObj.foreignExchangeTerms().businessDayConvention();
        //QlObj.foreignExchangeTerms().calendar();
        //QlObj.foreignExchangeTerms().dayCounter();

        public Money ContractNotionalAmountBase() => new Money(Impl.QlObj.contractNotionalAmountBase());
        public Money ContractNotionalAmountQuote() => new Money(Impl.QlObj.contractNotionalAmountTerm());

        public Money ForwardGrossValueBase() => new Money(Impl.QlObj.forwardGrossValueBase());
        public Money ForwardGrossValueQuote() => new Money(Impl.QlObj.forwardGrossValueTerm());

        public Money ForwardNetValueBase() => new Money(Impl.QlObj.forwardNetValueBase());
        public Money ForwardNetValueQuote() => new Money(Impl.QlObj.forwardNetValueTerm());

        public Money PresentNetValueBase() => new Money(Impl.QlObj.presentNetValueBase());
        public Money PresentNetValueQuote() => new Money(Impl.QlObj.presentNetValueTerm());

        public double FairForwardPoints() => Impl.QlObj.fairForwardPoints();
    }

    public class FxFwdImpl : InstrumentImpl<QlFxFwd>
    {
        public FxFwdImpl(QlFxFwd qlObj)
            : base(qlObj)
        {
        }
    }
}