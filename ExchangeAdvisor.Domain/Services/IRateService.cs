﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IRateService
    {
        Task RefreshSavedDataIfNeed(CurrencyPair currencyPair);
        
        Task<RateHistory> GetHistoryAsync(CurrencyPair currencyPair, DateRange dateRange);

        Task<RateForecast> GetNewestForecastAsync(CurrencyPair currencyPair, DateRange dateRange);

        Task<RateForecast> GetForecastAsync(CurrencyPair currencyPair, DateTime creationDay);

        Task<IEnumerable<RateForecastMetadata>> GetForecastsMetadataAsync(CurrencyPair currencyPair);

        Task<RateForecastMetadata> UpdateForecastMetadataAsync(RateForecastMetadata modifiedDescription);
    }
}