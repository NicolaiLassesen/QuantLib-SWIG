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

#ifndef quantlib_fxtermstructures_i
#define quantlib_fxtermstructures_i

%include termstructures.i
%include exchangerates.i
%include interpolation.i

%{
using QuantLib::FxForwardPointTermStructure;
%}

%shared_ptr(FxForwardPointTermStructure);
class FxForwardPointTermStructure : public TermStructure {
  private:
    FxForwardPointTermStructure();
  public:
    const Currency& source() const;
    const Currency& target() const;
    Decimal forwardPoints(const Date& d, bool extrapolate = false) const;
    Decimal forwardPoints(Time t, bool extrapolate = false) const;
    ForwardExchangeRate forwardExchangeRate(const Date& d, bool extrapolate = false) const;
    ForwardExchangeRate forwardExchangeRate(Time t, bool extrapolate = false) const;
};

%template(FxForwardPointTermStructureHandle) Handle<FxForwardPointTermStructure>;
%template(RelinkableFxForwardPointTermStructureHandle) RelinkableHandle<FxForwardPointTermStructure>;

%{
using QuantLib::InterpolatedFxForwardPointTermStructure;
%}

namespace std {
    %template(ForwardExchangeRateVector) vector<ForwardExchangeRate>;
}

%shared_ptr(InterpolatedFxForwardPointTermStructure<Linear>);
template <class Interpolator>
class InterpolatedFxForwardPointTermStructure : public FxForwardPointTermStructure {
  public:
    InterpolatedFxForwardPointTermStructure(const Date& referenceDate,
                                            const ExchangeRate& spotExchangeRate,
                                            const std::vector<Date>& dates,
                                            const std::vector<Decimal>& forwardPoints,
                                            const DayCounter& dayCounter,
                                            const Calendar& calendar = Calendar(),
                                            const Interpolator& interpolator = Interpolator());
    InterpolatedFxForwardPointTermStructure(const Date& referenceDate,
    	                                      const std::vector<ForwardExchangeRate>& fwdExchangeRates,
                                            const DayCounter& dayCounter,
                                            const Calendar& calendar = Calendar(),
                                            const Interpolator& interpolator = Interpolator());
};

%template(InterpolatedFxForwardPointTermStructureLinear) InterpolatedFxForwardPointTermStructure<Linear>;

#endif