﻿using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IRateForecaster
    {
        Task<RateForecast> ForecastAsync(RateHistory history, DateRange dateRange);
    }
}