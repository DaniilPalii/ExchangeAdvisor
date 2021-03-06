﻿using System;
using System.Collections.Generic;

namespace ExchangeAdvisor.Domain.Values
{
    public readonly struct DateRange
    {
        public readonly DateTime Start;

        public readonly DateTime End;

        public IEnumerable<DateTime> Days
        {
            get
            {
                for (var day = Start; day <= End; day = day.AddDays(1))
                {
                    yield return day;
                }
            }
        }

        private DateRange(DateTime start, DateTime end)
        {
            start = start.Date;
            end = end.Date;

            if (end < start)
            {
                throw new ArgumentException(
                    $"Start date is {start} and end is {end} "
                        + "but end date should be greater or equal to start date");
            }

            Start = start;
            End = end;
        }

        public static IDateRangeFrom FromMinDate() => From(DateTime.MinValue);

        public static IDateRangeFrom FromToday() => From(DateTime.Today);

        public static IDateRangeFrom From(int year, int month, int day) => From(new DateTime(year, month, day));

        public static IDateRangeFrom From(DateTime start) => new DateRangeFrom(start);

        public DateRange MakeStartingAt(DateTime start) => new DateRange(start, End);

        public DateRange MakeEndingAt(DateTime end) => new DateRange(Start, end);

        public bool Contains(DateTime date) => Start <= date.Date && End >= date.Date;

        public override bool Equals(object obj)
        {
            return obj is DateRange dateRange
                && dateRange.Start == Start
                && dateRange.End == End;
        }

        public override int GetHashCode() => (Start, End).GetHashCode();

        public static bool operator ==(DateRange left, DateRange right) => left.Equals(right);

        public static bool operator !=(DateRange left, DateRange right) => !(left == right);

        private readonly struct DateRangeFrom : IDateRangeFrom
        {
            public DateRangeFrom(DateTime start)
            {
                this.start = start;
            }

            public DateRange UntilMaxDate() => Until(DateTime.MaxValue);

            public DateRange UntilToday() => Until(DateTime.Today);

            public DateRange Until(TimeSpan offset) => Until(start + offset);

            public DateRange Until(int year, int month, int day) => Until(new DateTime(year, month, day));

            public DateRange Until(DateTime end) => new DateRange(start, end);

            private readonly DateTime start;
        }
    }

    public interface IDateRangeFrom
    {
        DateRange Until(DateTime end);

        DateRange Until(int year, int month, int day);

        DateRange Until(TimeSpan offset);

        DateRange UntilToday();

        DateRange UntilMaxDate();
    }
}
