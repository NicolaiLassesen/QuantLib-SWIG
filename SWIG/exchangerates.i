
/*
 Copyright (C) 2000, 2001, 2002, 2003 RiskMap srl
 Copyright (C) 2003, 2004 StatPro Italia srl
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

#ifndef quantlib_exchange_rates_i
#define quantlib_exchange_rates_i

%include money.i

%{
using QuantLib::ExchangeRate;
using QuantLib::ExchangeRateManager;
using QuantLib::ForwardExchangeRate;
%}

#if defined(SWIGCSHARP)
%typemap(cscode) ExchangeRate %{
    public override string ToString()
    {
        return this.__str__();
    }
%}
#endif

class ExchangeRate {
  public:
    enum Type { Direct, Derived };
    ExchangeRate(const Currency& source,
                 const Currency& target,
                 Decimal rate);

    const Currency& source() const;
    const Currency& target() const;
    Type type() const;
    Decimal rate() const;

    Money exchange(const Money& amount) const;
    static ExchangeRate chain(const ExchangeRate& r1,
                              const ExchangeRate& r2);
    static ExchangeRate inverse(const ExchangeRate& r);

    %extend {
        std::string __str__() {
            std::ostringstream out;
            out << self->source() << self->target() << " " << self->rate();
            return out.str();
        }
    }
};

#if defined(SWIGCSHARP)
%typemap(cscode) ForwardExchangeRate %{
    public override string ToString()
    {
        return this.__str__();
    }
%}
#endif

class ForwardExchangeRate {
  public:
	ForwardExchangeRate(const ExchangeRate& spotRate,
	                    Decimal forwardPoints,
                        const Period& tenor = Period());
	
    const Currency& source() const;
    const Currency& target() const;
    ExchangeRate::Type type() const;
    const ExchangeRate& spotExchangeRate() const;
    Decimal spotRate() const;
    Decimal forwardPoints() const;
    Decimal forwardRate() const;
    const Period& tenor() const;

    Money exchange(const Money& amount) const;
    static ForwardExchangeRate chain(const ForwardExchangeRate& r1,
                                     const ForwardExchangeRate& r2);
    static ForwardExchangeRate inverse(const ForwardExchangeRate& r);

    %extend {
        std::string __str__() {
            std::ostringstream out;
            out << self->source() << self->target() << " " << self->forwardRate();
            return out.str();
        }
    }
};


class ExchangeRateManager {
  private:
    ExchangeRateManager();
  public:
    static ExchangeRateManager& instance();
    void add(const ExchangeRate&,
             const Date& startDate = Date::minDate(),
             const Date& endDate = Date::maxDate());
    ExchangeRate lookup(const Currency& source,
                        const Currency& target,
                        const Date& date,
                        ExchangeRate::Type type = ExchangeRate::Derived) const;
    void clear();
};


#endif

