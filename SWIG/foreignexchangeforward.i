/*
 Copyright (C) 2020 Nicolai Lassesen

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

#ifndef quantlib_foreignexchangeforward_i
#define quantlib_foreignexchangeforward_i

%include instruments.i
%include exchangerates.i
%include termstructures.i
%include fxtermstructures.i

%{
using QuantLib::FxTerms;
using QuantLib::ForeignExchangeForward;
using QuantLib::ForwardPointsEngine;
%}

class FxTerms {
  public:
    FxTerms(const DayCounter& dayCounter,
            const Calendar& calendar,
            BusinessDayConvention businessDayConvention,
            Natural settlementDays);
    FxTerms(const Currency& baseCurrency, const Currency& termCurrency);
    FxTerms(const ExchangeRate& exchangeRate);
    const DayCounter& dayCounter() const;
    const Calendar& calendar() const;
    BusinessDayConvention businessDayConvention() const;
    Natural settlementDays() const;
};

#if defined(SWIGCSHARP)
%typemap(cscode) ForeignExchangeForward %{
    public override string ToString()
    {
        return this.__str__();
    }
%}
#endif

%shared_ptr(ForeignExchangeForward)
class ForeignExchangeForward : public Instrument {
  public:
  	enum class Type { SellBaseBuyTermForward, BuyBaseSellTermForward };
  	ForeignExchangeForward(const Date& deliveryDate,
                           const Money& baseNotionalAmount,
                           const ExchangeRate& contractAllInRate,
                           Type forwardType = Type::SellBaseBuyTermForward);
    ForeignExchangeForward(const Date& deliveryDate,
                           const Money& baseNotionalAmount,
                           const ExchangeRate& contractAllInRate,
                           Type forwardType,
                           const FxTerms& terms);
                           
    const Type forwardType() const;
    const Date& deliveryDate() const;
    const Currency& baseCurrency() const;
    const Currency& termCurrency() const;
    const ExchangeRate& contractAllInRate() const;
    const Money& contractNotionalAmountBase() const;
    Money contractNotionalAmountTerm() const;
    const FxTerms& foreignExchangeTerms() const;

    Decimal fairForwardPoints() const;
    Money forwardNetValueBase() const;
    Money forwardNetValueTerm() const;
    Money presentNetValueBase() const;
    Money presentNetValueTerm() const;
    Money forwardGrossValueBase() const;
    Money forwardGrossValueTerm() const;

    %extend {
        std::string __str__() {
            std::ostringstream out;
            out << *self;
            return out.str();
        }
    }
};

%shared_ptr(ForwardPointsEngine)
class ForwardPointsEngine : public PricingEngine {
  public:
    ForwardPointsEngine(const ExchangeRate& spotExchangeRate,
                        const Handle<FxForwardPointTermStructure>& forwardPointsCurve = Handle<FxForwardPointTermStructure>(),
                        const Handle<YieldTermStructure>& baseDiscountCurve = Handle<YieldTermStructure>(),
                        const Handle<YieldTermStructure>& termDiscountCurve = Handle<YieldTermStructure>());
};

#endif