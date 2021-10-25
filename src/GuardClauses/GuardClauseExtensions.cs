using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bajonczak.GuardClauses
{
    /// <summary>
    /// A collection of common guard clauses, implemented as extensions.
    /// </summary>
    /// <example>
    /// Guard.Against.Null(input, nameof(input));
    /// </example>
    public static class GuardClauseExtensions
    {
        /// <summary>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="input" /> is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not null.</returns>
        public static T Null<T>( this IGuardClause guardClause, [NotNull] [ValidatedNotNull] T input,  string parameterName, string? message = null)
        {
            if (input is null)
            {
                if (string.IsNullOrEmpty(message))
                {
                    throw new ArgumentNullException(parameterName);
                }
                throw new ArgumentNullException(message, (Exception?)null);
            }

            return input;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="input" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is an empty string.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not an empty string or null.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string NullOrEmpty( this IGuardClause guardClause, [NotNull] [ValidatedNotNull] string? input,  string parameterName, string? message = null)
        {
            Guard.Against.Null(input, parameterName);
            if (input == string.Empty)
            {
                throw new ArgumentException(message ?? $"Required input {parameterName} was empty.", parameterName);
            }

            return input;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="input" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is an empty guid.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not an empty guid or null.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Guid NullOrEmpty( this IGuardClause guardClause, [NotNull] [ValidatedNotNull] Guid? input,  string parameterName, string? message = null)
        {
            Guard.Against.Null(input, parameterName);
            if (input == Guid.Empty)
            {
                throw new ArgumentException(message ?? $"Required input {parameterName} was empty.", parameterName);
            }

            return input.Value;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="input" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is an empty enumerable.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not an empty enumerable or null.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<T> NullOrEmpty<T>( this IGuardClause guardClause, [NotNull] [ValidatedNotNull] IEnumerable<T>? input,  string parameterName, string? message = null)
        {
            Guard.Against.Null(input, parameterName);
            if (!input.Any())
            {
                throw new ArgumentException(message ?? $"Required input {parameterName} was empty.", parameterName);
            }

            return input;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="input" /> is null.
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is an empty or white space string.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not an empty or whitespace string.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string NullOrWhiteSpace( this IGuardClause guardClause, [NotNull] [ValidatedNotNull] string? input,  string parameterName, string? message = null)
        {
            Guard.Against.NullOrEmpty(input, parameterName);
            if (String.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException(message ?? $"Required input {parameterName} was empty.", parameterName);
            }

            return input;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException" /> if <paramref name="input" /> is not in the range of valid SqlDateTime values.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is in the range of valid SqlDateTime values.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DateTime OutOfSQLDateRange( this IGuardClause guardClause, DateTime input,  string parameterName, string? message = null)
        {
            // System.Data is unavailable in .NET Standard so we can't use SqlDateTime.
            const long sqlMinDateTicks = 552877920000000000;
            const long sqlMaxDateTicks = 3155378975999970000;

            return OutOfRange<DateTime>(guardClause, input, parameterName, new DateTime(sqlMinDateTicks), new DateTime(sqlMaxDateTicks), message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException" /> if <paramref name="input"/> is less than <paramref name="rangeFrom"/> or greater than <paramref name="rangeTo"/>.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="rangeFrom"></param>
        /// <param name="rangeTo"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not out of range.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static T OutOfRange<T>(this IGuardClause guardClause, T input, string parameterName, T rangeFrom, T rangeTo, string? message = null) where T : IComparable, IComparable<T>
        {
            if (rangeFrom.CompareTo(rangeTo) > 0)
            {
                throw new ArgumentException(message ?? $"{nameof(rangeFrom)} should be less or equal than {nameof(rangeTo)}");
            }

            if (input.CompareTo(rangeFrom) < 0 || input.CompareTo(rangeTo) > 0)
            {
                if (string.IsNullOrEmpty(message))
                {
                    throw new ArgumentOutOfRangeException(parameterName, $"Input {parameterName} was out of range");
                }
                throw new ArgumentOutOfRangeException(message, (Exception?)null);
            }

            return input;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is zero.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not zero.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int Zero( this IGuardClause guardClause, int input,  string parameterName, string? message = null)
        {
            return Zero<int>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is zero.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not zero.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static long Zero( this IGuardClause guardClause, long input,  string parameterName, string? message = null)
        {
            return Zero<long>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is zero.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not zero.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static decimal Zero( this IGuardClause guardClause, decimal input,  string parameterName, string? message = null)
        {
            return Zero<decimal>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is zero.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not zero.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static float Zero( this IGuardClause guardClause, float input,  string parameterName, string? message = null)
        {
            return Zero<float>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is zero.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not zero.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static double Zero( this IGuardClause guardClause, double input,  string parameterName, string? message = null)
        {
            return Zero<double>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is zero.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <returns><paramref name="input" /> if the value is not zero.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static TimeSpan Zero( this IGuardClause guardClause, TimeSpan input,  string parameterName)
        {
            return Zero<TimeSpan>(guardClause, input, parameterName);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is zero.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not zero.</returns>
        /// <exception cref="ArgumentException"></exception>
        private static T Zero<T>( this IGuardClause guardClause, T input,  string parameterName, string? message = null) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(input, default(T)))
            {
                throw new ArgumentException(message ?? $"Required input {parameterName} cannot be zero.", parameterName);
            }

            return input;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input"/> is negative.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not negative.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int Negative( this IGuardClause guardClause, int input,  string parameterName, string? message = null)
        {
            return Negative<int>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input"/> is negative.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not negative.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static long Negative( this IGuardClause guardClause, long input,  string parameterName, string? message = null)
        {
            return Negative<long>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input"/> is negative.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not negative.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static decimal Negative( this IGuardClause guardClause, decimal input,  string parameterName, string? message = null)
        {
            return Negative<decimal>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input"/> is negative.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not negative.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static float Negative( this IGuardClause guardClause, float input,  string parameterName, string? message = null)
        {
            return Negative<float>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input"/> is negative.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not negative.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static double Negative( this IGuardClause guardClause, double input,  string parameterName, string? message = null)
        {
            return Negative<double>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input"/> is negative.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not negative.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static TimeSpan Negative( this IGuardClause guardClause, TimeSpan input,  string parameterName, string? message = null)
        {
            return Negative<TimeSpan>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input"/> is negative.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not negative.</returns>
        /// <exception cref="ArgumentException"></exception>
        private static T Negative<T>( this IGuardClause guardClause, T input,  string parameterName, string? message = null) where T : struct, IComparable
        {
            if (input.CompareTo(default(T)) < 0)
            {
                throw new ArgumentException(message ?? $"Required input {parameterName} cannot be negative.", parameterName);
            }

            return input;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if <paramref name="input"/> is negative or zero.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not negative or zero.</returns>
        public static int NegativeOrZero( this IGuardClause guardClause, int input,  string parameterName, string? message = null)
        {
            return NegativeOrZero<int>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if <paramref name="input"/> is negative or zero.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not negative or zero.</returns>
        public static long NegativeOrZero( this IGuardClause guardClause, long input,  string parameterName, string? message = null)
        {
            return NegativeOrZero<long>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if <paramref name="input"/> is negative or zero.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not negative or zero.</returns>
        public static decimal NegativeOrZero( this IGuardClause guardClause, decimal input,  string parameterName, string? message = null)
        {
            return NegativeOrZero<decimal>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if <paramref name="input"/> is negative or zero.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not negative or zero.</returns>
        public static float NegativeOrZero( this IGuardClause guardClause, float input,  string parameterName, string? message = null)
        {
            return NegativeOrZero<float>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if <paramref name="input"/> is negative or zero.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not negative or zero.</returns>
        public static double NegativeOrZero( this IGuardClause guardClause, double input,  string parameterName, string? message = null)
        {
            return NegativeOrZero<double>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if <paramref name="input"/> is negative or zero.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not negative or zero.</returns>
        public static TimeSpan NegativeOrZero( this IGuardClause guardClause, TimeSpan input,  string parameterName, string? message = null)
        {
            return NegativeOrZero<TimeSpan>(guardClause, input, parameterName, message);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if <paramref name="input"/> is negative or zero. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not negative or zero.</returns>
        private static T NegativeOrZero<T>( this IGuardClause guardClause, T input,  string parameterName, string? message = null) where T : struct, IComparable
        {
            if (input.CompareTo(default(T)) <= 0)
            {
                throw new ArgumentException(message ?? $"Required input {parameterName} cannot be zero or negative.", parameterName);
            }

            return input;
        }

        /// <summary>
        /// Throws an <see cref="InvalidEnumArgumentException" /> if <paramref name="input"/> is not a valid enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not out of range.</returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static int OutOfRange<T>( this IGuardClause guardClause, int input,  string parameterName, string? message = null) where T : struct, Enum
        {
            if (!Enum.IsDefined(typeof(T), input))
            {
                if (string.IsNullOrEmpty(message))
                {
                    throw new InvalidEnumArgumentException(parameterName, input, typeof(T));
                }
                throw new InvalidEnumArgumentException(message);
            }

            return input;
        }

        /// <summary>
        /// Throws an <see cref="InvalidEnumArgumentException" /> if <paramref name="input"/> is not a valid enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not out of range.</returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static T OutOfRange<T>( this IGuardClause guardClause, T input,  string parameterName, string? message = null) where T : struct, Enum
        {
            if (!Enum.IsDefined(typeof(T), input))
            {
                if (string.IsNullOrEmpty(message))
                {
                    throw new InvalidEnumArgumentException(parameterName, Convert.ToInt32(input), typeof(T));
                }
                throw new InvalidEnumArgumentException(message);
            }

            return input;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is default for that type.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if the value is not default for that type.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static T Default<T>( this IGuardClause guardClause, [AllowNull] [NotNull]  T input,  string parameterName, string? message = null)
        {
            if (EqualityComparer<T>.Default.Equals(input, default(T)!) || input is null)
            {
                throw new ArgumentException(message ?? $"Parameter [{parameterName}] is default value for type {typeof(T).Name}", parameterName);
            }

            return input;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if  <paramref name="input"/> doesn't match the <paramref name="regexPattern"/>.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="regexPattern"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string InvalidFormat( this IGuardClause guardClause,  string input,  string parameterName,  string regexPattern, string? message = null)
        {
            if (input != Regex.Match(input, regexPattern).Value)
            {
                throw new ArgumentException(message ?? $"Input {parameterName} was not in required format", parameterName);
            }

            return input;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if  <paramref name="input"/> doesn't satisfy the <paramref name="predicate"/> function.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="predicate"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T InvalidInput<T>( this IGuardClause guardClause,  T input,  string parameterName, Func<T, bool> predicate, string? message = null)
        {
            if (!predicate(input))
            {
                throw new ArgumentException(message ?? $"Input {parameterName} did not satisfy the options", parameterName);
            }

            return input;
        }


        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException" /> if  any <paramref name="input"/>'s item is less than <paramref name="rangeFrom"/> or greater than <paramref name="rangeTo"/>.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <param name="rangeFrom"></param>
        /// <param name="rangeTo"></param>
        /// <param name="message">Optional. Custom error message</param>
        /// <returns><paramref name="input" /> if any item is not out of range.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static IEnumerable<T> OutOfRange<T>(this IGuardClause guardClause, IEnumerable<T> input, string parameterName, T rangeFrom, T rangeTo, string? message = null) where T : IComparable, IComparable<T>
        {
            if (rangeFrom.CompareTo(rangeTo) > 0)
            {
                throw new ArgumentException(message ?? $"{nameof(rangeFrom)} should be less or equal than {nameof(rangeTo)}");
            }

            if (input.Any(x => x.CompareTo(rangeFrom) < 0 || x.CompareTo(rangeTo) > 0))
            {
                if (string.IsNullOrEmpty(message))
                {
                    throw new ArgumentOutOfRangeException(parameterName, message ?? $"Input {parameterName} had out of range item(s)");
                }
                throw new ArgumentOutOfRangeException(message, (Exception?)null);
            }

            return input;
        }

        /// <summary>
        /// Throws an <see cref="NotFoundException" /> if <paramref name="input" /> with <paramref name="key" /> is not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="guardClause"></param>
        /// <param name="key"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <returns><paramref name="input" /> if the value is not null.</returns>
        /// <exception cref="NotFoundException"></exception>
        public static T NotFound<T>( this IGuardClause guardClause, [NotNull] [ValidatedNotNull] string key, [NotNull] [ValidatedNotNull] T input,  string parameterName)
        {
            guardClause.NullOrEmpty(key, nameof(key));

            if (input is null)
            {
                throw new NotFoundException(key, parameterName);
            }

            return input;
        }

        /// <summary>
        /// Throws an <see cref="NotFoundException" /> if <paramref name="input" /> with <paramref name="key" /> is not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="guardClause"></param>
        /// <param name="key"></param>
        /// <param name="input"></param>
        /// <param name="parameterName"></param>
        /// <returns><paramref name="input" /> if the value is not null.</returns>
        /// <exception cref="NotFoundException"></exception>
        public static T NotFound<TKey, T>( this IGuardClause guardClause, [NotNull] [ValidatedNotNull] TKey key, [NotNull] [ValidatedNotNull] T input,  string parameterName) where TKey : struct
        {
            guardClause.Null(key, nameof(key));

            if (input is null)
            {
                // TODO: Can we safely consider that ToString() won't return null for struct?
                throw new NotFoundException(key.ToString()!, parameterName);
            }

            return input;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="func"/> evaluates to false for given <paramref name="input"/> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="message"></param>
        /// <returns><paramref name="input"/> if the <paramref name="func"/> evaluates to true </returns>
        /// <exception cref="ArgumentException"></exception>
        public static T AgainstExpression<T>( this IGuardClause guardClause,  Func<T, bool> func, T input, string message) where T : struct
        {
            if (!func(input))
            {
                throw new ArgumentException(message);
            }

            return input;
        }
    }
}
