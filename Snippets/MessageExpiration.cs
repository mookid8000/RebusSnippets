using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rebus.Messages;

namespace Snippets
{
    [TestFixture]
    public class MessageExpiration
    {

    }

    [Header(Headers.TimeToBeReceived, "00:01:00")]
    public class PriceCurveUpdated
    {
        public PriceCurveUpdated(string fondsCode, PriceCurve priceCurve, DateTime updateTimeUtc)
        {
            FondsCode = fondsCode;
            PriceCurve = priceCurve;
            UpdateTimeUtc = updateTimeUtc;
        }

        public string FondsCode { get; }
        public PriceCurve PriceCurve { get; }
        public DateTime UpdateTimeUtc { get; }
    }

    public class PriceCurve
    {
        public PriceCurve(IEnumerable<Price> prices)
        {
            Prices = prices.ToArray();
        }

        public IReadOnlyCollection<Price> Prices { get; }
    }

    public class Price
    {
        public Price(Date date, decimal value)
        {
            Date = date;
            Value = value;
        }

        public Date Date { get; }
        public decimal Value { get; }
    }

    public class Date
    {
        public Date(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public int Year { get; }
        public int Month { get; }
        public int Day { get; }
    }
}