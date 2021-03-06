namespace BookFx.Functional
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using static F;

    /// <summary>
    /// Represents the possibility of the absense of data.
    /// </summary>
    public struct Option<T> : IEquatable<Option.None>, IEquatable<Option<T>>
    {
        private readonly T _value;

        private Option(T value)
        {
            IsSome = true;
            _value = value;
        }

        /// <summary>
        /// Gets a value indicating whether is a value present.
        /// </summary>
        public bool IsSome { get; }

        /// <summary>
        /// Gets a value indicating whether is a value absense.
        /// </summary>
        public bool IsNone => !IsSome;

        /// <summary>
        /// Implicit convert from <see cref="Option.None"/> to <see cref="Option{T}"/>.
        /// </summary>
        public static implicit operator Option<T>(Option.None none) => default;

        /// <summary>
        /// Implicit convert from <see cref="Option.Some{T}"/> to <see cref="Option{T}"/>.
        /// </summary>
        public static implicit operator Option<T>(Option.Some<T> some) => new Option<T>(some.Value);

        /// <summary>
        /// Implicit convert from <typeparamref name="T"/> to <see cref="Option{T}"/>.
        /// </summary>
        /// <param name="value">
        /// A value of any type.
        /// </param>
        public static implicit operator Option<T>(T value) => Some(value);

        /// <summary>
        /// Equality operator.
        /// </summary>
        public static bool operator ==(Option<T> @this, Option<T> other) => @this.Equals(other);

        /// <summary>
        /// Inequality operator.
        /// </summary>
        public static bool operator !=(Option<T> @this, Option<T> other) => !(@this == other);

        /// <summary>
        /// Pattern matching.
        /// </summary>
        /// <typeparam name="TR">A type of the result.</typeparam>
        /// <param name="none">A function for the none case.</param>
        /// <param name="some">A function for the some case.</param>
        /// <returns>A result either of the <paramref name="none"/> or the <paramref name="some"/> function.</returns>
        public TR Match<TR>(Func<TR> none, Func<T, TR> some) => IsSome ? some(_value) : none();

        /// <summary>
        /// Converts an <see cref="Option{T}"/> to an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <returns>
        /// Either an empty <see cref="IEnumerable{T}"/> or an <see cref="IEnumerable{T}"/> containing one value.
        /// </returns>
        [Pure]
        public IEnumerable<T> AsEnumerable()
        {
            if (IsSome)
            {
                yield return _value;
            }
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Option<T> other && Equals(other);

        /// <inheritdoc />
        public bool Equals(Option.None other) => IsNone;

        /// <inheritdoc />
        public bool Equals(Option<T> other) =>
            IsSome == other.IsSome &&
            (IsNone || _value!.Equals(other._value));

        /// <inheritdoc />
        public override int GetHashCode() => IsSome ? _value!.GetHashCode() : 0;

        /// <inheritdoc />
        public override string ToString() =>
            Match(
                none: () => "None",
                some: value => $"Some({value})");
    }
}