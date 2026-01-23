using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace DonaRogApp.ValueObjects
{
    /// <summary>
    /// Value Object: Percentage
    /// Represents a percentage value with validation and arithmetic operations.
    /// Immutable, self-validating.
    /// </summary>
    public class Percentage : ValueObject
    {
        // --------------------------------------------------------------
        // CONSTANTS
        // --------------------------------------------------------------

        public const decimal MinValue = 0m;
        public const decimal MaxValue = 100m;

        // --------------------------------------------------------------
        // PROPERTIES
        // --------------------------------------------------------------

        /// <summary>
        /// Percentage value (0-100)
        /// Example: 25.5 means 25.5%
        /// </summary>
        public decimal Value { get; }

        /// <summary>
        /// Decimal representation (0-1)
        /// Example: 0.255 for 25.5%
        /// </summary>
        public decimal DecimalValue => Value / 100m;

        // --------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------

        private Percentage()
        {
            // EF Core needs parameterless constructor
        }

        public Percentage(decimal value)
        {
            if (value < MinValue || value > MaxValue)
                throw new ArgumentException(
                    $"Percentage must be between {MinValue} and {MaxValue}. Got: {value}",
                    nameof(value)
                );

            Value = Math.Round(value, 2); // Always 2 decimals
        }

        // --------------------------------------------------------------
        // STATIC FACTORIES
        // --------------------------------------------------------------

        /// <summary>
        /// Creates percentage from decimal (0-1)
        /// Example: FromDecimal(0.255) = 25.5%
        /// </summary>
        public static Percentage FromDecimal(decimal decimalValue)
        {
            if (decimalValue < 0 || decimalValue > 1)
                throw new ArgumentException(
                    $"Decimal value must be between 0 and 1. Got: {decimalValue}",
                    nameof(decimalValue)
                );

            return new Percentage(decimalValue * 100);
        }

        /// <summary>
        /// Creates percentage from fraction (numerator / denominator)
        /// Example: FromFraction(3, 4) = 75%
        /// </summary>
        public static Percentage FromFraction(decimal numerator, decimal denominator)
        {
            if (denominator == 0)
                throw new ArgumentException("Denominator cannot be zero", nameof(denominator));

            var value = (numerator / denominator) * 100;
            return new Percentage(value);
        }

        public static Percentage Zero() => new Percentage(0);

        public static Percentage Full() => new Percentage(100);

        public static Percentage Half() => new Percentage(50);

        // --------------------------------------------------------------
        // ARITHMETIC OPERATIONS
        // --------------------------------------------------------------

        /// <summary>
        /// Adds two percentages
        /// </summary>
        public static Percentage operator +(Percentage left, Percentage right)
        {
            Check.NotNull(left, nameof(left));
            Check.NotNull(right, nameof(right));

            var result = left.Value + right.Value;

            if (result > MaxValue)
                throw new InvalidOperationException(
                    $"Sum exceeds maximum percentage: {result}%"
                );

            return new Percentage(result);
        }

        /// <summary>
        /// Subtracts two percentages
        /// </summary>
        public static Percentage operator -(Percentage left, Percentage right)
        {
            Check.NotNull(left, nameof(left));
            Check.NotNull(right, nameof(right));

            var result = left.Value - right.Value;

            if (result < MinValue)
                throw new InvalidOperationException(
                    $"Result is negative: {result}%"
                );

            return new Percentage(result);
        }

        /// <summary>
        /// Multiplies percentage by a scalar
        /// </summary>
        public static Percentage operator *(Percentage percentage, decimal multiplier)
        {
            Check.NotNull(percentage, nameof(percentage));

            return new Percentage(percentage.Value * multiplier);
        }

        /// <summary>
        /// Divides percentage by a scalar
        /// </summary>
        public static Percentage operator /(Percentage percentage, decimal divisor)
        {
            Check.NotNull(percentage, nameof(percentage));

            if (divisor == 0)
                throw new DivideByZeroException("Cannot divide percentage by zero");

            return new Percentage(percentage.Value / divisor);
        }

        // --------------------------------------------------------------
        // COMPARISON OPERATIONS
        // --------------------------------------------------------------

        public static bool operator >(Percentage left, Percentage right)
        {
            Check.NotNull(left, nameof(left));
            Check.NotNull(right, nameof(right));

            return left.Value > right.Value;
        }

        public static bool operator <(Percentage left, Percentage right)
        {
            Check.NotNull(left, nameof(left));
            Check.NotNull(right, nameof(right));

            return left.Value < right.Value;
        }

        public static bool operator >=(Percentage left, Percentage right)
        {
            return left > right || left == right;
        }

        public static bool operator <=(Percentage left, Percentage right)
        {
            return left < right || left == right;
        }

        // --------------------------------------------------------------
        // QUERY METHODS
        // --------------------------------------------------------------

        public bool IsZero() => Value == 0;

        public bool IsFull() => Value == 100;

        public bool IsHalf() => Value == 50;

        public bool IsBetween(decimal min, decimal max)
        {
            return Value >= min && Value <= max;
        }

        // --------------------------------------------------------------
        // CALCULATION METHODS
        // --------------------------------------------------------------

        /// <summary>
        /// Calculates percentage of a value
        /// Example: 25%.Of(200) = 50
        /// </summary>
        public decimal Of(decimal value)
        {
            return value * DecimalValue;
        }

        /// <summary>
        /// Calculates percentage of Money
        /// Example: 25%.Of(Money.Euros(200)) = Money.Euros(50)
        /// </summary>
        public Money Of(Money money)
        {
            Check.NotNull(money, nameof(money));

            return new Money(money.Amount * DecimalValue, money.Currency);
        }

        /// <summary>
        /// Applies percentage to a value (adds the percentage)
        /// Example: 10%.ApplyTo(100) = 110
        /// </summary>
        public decimal ApplyTo(decimal value)
        {
            return value + Of(value);
        }

        /// <summary>
        /// Removes percentage from a value (subtracts the percentage)
        /// Example: 10%.RemoveFrom(110) = 99
        /// </summary>
        public decimal RemoveFrom(decimal value)
        {
            return value - Of(value);
        }

        /// <summary>
        /// Inverts percentage
        /// Example: 25% → 75%
        /// </summary>
        public Percentage Invert()
        {
            return new Percentage(100 - Value);
        }

        // --------------------------------------------------------------
        // FORMATTING
        // --------------------------------------------------------------

        public override string ToString()
        {
            return $"{Value}%";
        }

        public string ToString(string format)
        {
            return $"{Value.ToString(format)}%";
        }

        public string ToDecimalString(int decimals = 4)
        {
            return DecimalValue.ToString($"F{decimals}");
        }

        // --------------------------------------------------------------
        // VALUE OBJECT IMPLEMENTATION
        // --------------------------------------------------------------

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
