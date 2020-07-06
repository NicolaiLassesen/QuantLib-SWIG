using System;
using CfAnalytics.Utilities;
using QuantLib;
using QlFxFwd = QuantLib.ForeignExchangeForward;
// ReSharper disable InconsistentNaming

namespace CfAnalytics.QuantLib.Instruments
{
    public class ForeignExchangeForward : Instrument<QlFxFwd>
    {
        public enum Type
        {
            SellBaseBuyTermForward,
            BuyBaseSellTermForward
        }

        public ForeignExchangeForward(DateTime deliveryDate, Money baseNotional, ExchangeRate contractAllInRate)
            : base(new QlFxFwd(deliveryDate, baseNotional.QlObj, contractAllInRate.QlObj))
        {
        }

        public DateTime DeliveryDate => QlObj.deliveryDate().AsDateTime();
        public ExchangeRate ContractAllInRate=>new ExchangeRate(QlObj.contractAllInRate());
        public Currency BaseCurrency => EnumUtils.GetCurrency(QlObj.baseCurrency().code());
        public Currency QuoteCurrency => EnumUtils.GetCurrency(QlObj.termCurrency().code());
        public Type ForwardType => EnumUtils.GetEnumValue<Type>(QlObj.forwardType().ToString());

        //QlObj.foreignExchangeTerms().settlementDays();
        //QlObj.foreignExchangeTerms().businessDayConvention();
        //QlObj.foreignExchangeTerms().calendar();
        //QlObj.foreignExchangeTerms().dayCounter();

        public Money ContractNotionalAmountBase() => new Money(QlObj.contractNotionalAmountBase());
        public Money ContractNotionalAmountQuote() => new Money(QlObj.contractNotionalAmountTerm());

        public Money ForwardGrossValueBase() => new Money(QlObj.forwardGrossValueBase());
        public Money ForwardGrossValueQuote() => new Money(QlObj.forwardGrossValueTerm());

        public Money ForwardNetValueBase() => new Money(QlObj.forwardNetValueBase());
        public Money ForwardNetValueQuote() => new Money(QlObj.forwardNetValueTerm());

        public Money PresentNetValueBase() => new Money(QlObj.presentNetValueBase());
        public Money PresentNetValueQuote() => new Money(QlObj.presentNetValueTerm());

        public double FairForwardPoints() => QlObj.fairForwardPoints();
    }
}