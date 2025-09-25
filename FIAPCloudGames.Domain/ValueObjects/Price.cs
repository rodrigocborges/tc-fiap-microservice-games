using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;

namespace FIAPCloudGames.Domain.ValueObjects
{
    public class Price
    {
        public decimal Value { get; set; }

        private Price() { }

        public Price(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Price cannot be negative.");
            Value = decimal.Round(value, 2);
        }

        public override bool Equals(object? obj) => obj is Price other && Value == other.Value;
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString("C2", CultureInfo.CurrentCulture);
    }
}
