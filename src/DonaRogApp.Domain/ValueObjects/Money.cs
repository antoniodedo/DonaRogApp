using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace DonaRogApp.ValueObjects
{
    /// <summary>
    /// Value Object: Money (Amount + Currency)
    /// Immutable, self-validating, with arithmetic operations.
    /// </summary>
    public class Money : ValueObject
    {
        // --------------------------------------------------------------
        // PROPERTIES
        // --------------------------------------------------------------

        /// <summary>
        /// Amount value (always positive or zero)
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        /// Currency code (ISO 4217)
        /// Examples: EUR, USD, GBP
        /// </summary>
        public string Currency { get; }

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------

        private Money()
        {
            // EF Core needs parameterless constructor
            Currency = "EUR";
        }

        public Money(decimal amount, string currency = "EUR")
        {
            // Validation
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required", nameof(currency));

            if (currency.Length != 3)
                throw new ArgumentException("Currency must be 3 characters (ISO 4217)", nameof(currency));

            // Normalize
            Amount = Math.Round(amount, 2); // Always 2 decimals
            Currency = currency.ToUpperInvariant();
        }

        // --------------------------------------------------------------
        // STATIC FACTORIES
        // --------------------------------------------------------------

        public static Money Zero(string currency = "EUR") => new Money(0, currency);

        public static Money Euros(decimal amount) => new Money(amount, "EUR");

        public static Money Dollars(decimal amount) => new Money(amount, "USD");

        public static Money Pounds(decimal amount) => new Money(amount, "GBP");

        // --------------------------------------------------------------
        // ARITHMETIC OPERATIONS
        // --------------------------------------------------------------

        /// <summary>
        /// Adds two Money values (must have same currency).
        /// </summary>
        public static Money operator +(Money left, Money right)
        {
            Check.NotNull(left, nameof(left));
            Check.NotNull(right, nameof(right));

            if (left.Currency != right.Currency)
                throw new InvalidOperationException(
                    $"Cannot add different currencies: {left.Currency} and {right.Currency}"
                );

            return new Money(left.Amount + right.Amount, left.Currency);
        }

        /// <summary>
        /// Subtracts two Money values (must have same currency).
        /// </summary>
        public static Money operator -(Money left, Money right)
        {
            Check.NotNull(left, nameof(left));
            Check.NotNull(right, nameof(right));

            if (left.Currency != right.Currency)
                throw new InvalidOperationException(
                    $"Cannot subtract different currencies: {left.Currency} and {right.Currency}"
                );

            return new Money(left.Amount - right.Amount, left.Currency);
        }

        /// <summary>
        /// Multiplies Money by a scalar.
        /// </summary>
        public static Money operator *(Money money, decimal multiplier)
        {
            Check.NotNull(money, nameof(money));

            return new Money(money.Amount * multiplier, money.Currency);
        }

        /// <summary>
        /// Divides Money by a scalar.
        /// </summary>
        public static Money operator /(Money money, decimal divisor)
        {
            Check.NotNull(money, nameof(money));

            if (divisor == 0)
                throw new DivideByZeroException("Cannot divide money by zero");

            return new Money(money.Amount / divisor, money.Currency);
        }

        // --------------------------------------------------------------
        // COMPARISON OPERATIONS
        // --------------------------------------------------------------

        public static bool operator >(Money left, Money right)
        {
            Check.NotNull(left, nameof(left));
            Check.NotNull(right, nameof(right));

            if (left.Currency != right.Currency)
                throw new InvalidOperationException(
                    $"Cannot compare different currencies: {left.Currency} and {right.Currency}"
                );

            return left.Amount > right.Amount;
        }

        public static bool operator <(Money left, Money right)
        {
            Check.NotNull(left, nameof(left));
            Check.NotNull(right, nameof(right));

            if (left.Currency != right.Currency)
                throw new InvalidOperationException(
                    $"Cannot compare different currencies: {left.Currency} and {right.Currency}"
                );

            return left.Amount < right.Amount;
        }

        public static bool operator >=(Money left, Money right)
        {
            return left > right || left == right;
        }

        public static bool operator <=(Money left, Money right)
        {
            return left < right || left == right;
        }

        // --------------------------------------------------------------
        // QUERY METHODS
        // --------------------------------------------------------------

        public bool IsZero() => Amount == 0;

        public bool IsPositive() => Amount > 0;

        public bool HasSameCurrency(Money other) => Currency == other.Currency;

        // --------------------------------------------------------------
        // FORMATTING
        // --------------------------------------------------------------

        public override string ToString()
        {
            return $"{Amount:N2} {Currency}";
        }

        public string ToString(string format)
        {
            return $"{Amount.ToString(format)} {Currency}";
        }

        public string ToStringWithSymbol()
        {
            var symbol = Currency switch
            {
                "EUR" => "€",
                "USD" => "$",
                "GBP" => "£",
                _ => Currency
            };

            return $"{symbol} {Amount:N2}";
        }

        // --------------------------------------------------------------
        // VALUE OBJECT IMPLEMENTATION
        // --------------------------------------------------------------

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}
