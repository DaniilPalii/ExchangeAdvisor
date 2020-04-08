﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExchangeAdvisor.DB.Context;
using ExchangeAdvisor.DB.Entities;
using ExchangeAdvisor.Domain.Helpers;
using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Values;
using Microsoft.EntityFrameworkCore;
using RateHistoryEntity = ExchangeAdvisor.DB.Entities.RateHistory;
using DomainRateHistory = ExchangeAdvisor.Domain.Values.Rate.RateHistory;

namespace ExchangeAdvisor.DB.Repositories
{
    public class RateHistoryRepository : RepositoryBase, IRateHistoryRepository
    {
        public RateHistoryRepository(IDatabaseConnectionStringReader connectionStringReader)
            : base(connectionStringReader)
        { }

        public async Task<bool> ExistsAsync(CurrencyPair currencyPair)
        {
            await using var dbc = CreateDatabaseContext();

            return await dbc.RateHistory.AnyAsync(By(currencyPair));
        }

        public async Task<DateTime> GetLastDayAsync(CurrencyPair currencyPair)
        {
            await using var dbc = CreateDatabaseContext();

            var history = await dbc.RateHistory.SingleAsync(By(currencyPair));

            return await dbc.HistoricalRate.Where(r => r.History.Id == history.Id).MaxAsync(r => r.Day);
        }

        public async Task<DomainRateHistory> GetAsync(CurrencyPair currencyPair)
        {
            await using var dbc = CreateDatabaseContext();

            return (await GetFullHistoryAsync(dbc, currencyPair)).ToDomain();
        }

        public async Task<DomainRateHistory> UpdateAsync(DomainRateHistory history)
        {
            await using var dbc = CreateDatabaseContext();

            var existingHistory = await GetFullHistoryAsync(dbc, history.CurrencyPair);
            var existingRateDays = existingHistory.Rates.Select(r => r.Day).ToHashSet();
            var newRateEntities = history.Rates.Where(r => !existingRateDays.Contains(r.Day));

            existingHistory.Rates.Add(newRateEntities.Select(r => new HistoricalRate(r, existingHistory)));

            dbc.Entry(existingHistory).Property(h => h.Rates).IsModified = true;
            await dbc.SaveChangesAsync();

            return (await GetFullHistoryAsync(dbc, history.CurrencyPair)).ToDomain();
        }

        private static Task<RateHistory> GetFullHistoryAsync(DatabaseContext dbc, CurrencyPair currencyPair)
        {
            return dbc.RateHistory.Include(h => h.Rates).SingleAsync(By(currencyPair));
        }

        private static Expression<Func<RateHistoryEntity, bool>> By(CurrencyPair currencyPair)
        {
            return rateHistory => currencyPair.Equals(rateHistory.BaseCurrency, rateHistory.ComparingCurrency);
        }
    }
}