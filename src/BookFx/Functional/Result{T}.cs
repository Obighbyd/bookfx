﻿namespace BookFx.Functional
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static F;
    using Unit = System.ValueTuple;

    internal struct Result<T>
    {
        public static Func<T, Result<T>> Return = Valid;

        private readonly Option<T> _value;

        private readonly IEnumerable<Error> _errors;

        internal Result(T value)
        {
            _value = Some(value);
            _errors = Enumerable.Empty<Error>();
        }

        private Result(IEnumerable<Error> errors)
        {
            _value = None;
            _errors = errors;
        }

        public bool IsValid => _value.IsSome;

        public static implicit operator Result<T>(Error error) => new Result<T>(new[] { error });

        public static implicit operator Result<T>(Result.Invalid invalid) =>
            new Result<T>(invalid.Errors);

        public static implicit operator Result<T>(T value) => Valid(value);

        public static Result<T> Fail(IEnumerable<Error> errors) => new Result<T>(errors);

        public static Result<T> Fail(params Error[] errors) => new Result<T>(errors.AsEnumerable());

        public TR Match<TR>(Func<IEnumerable<Error>, TR> invalid, Func<T, TR> valid) =>
            IsValid ? valid(_value.ValueUnsafe()) : invalid(_errors);

        public Unit Match(Action<IEnumerable<Error>> invalid, Action<T> valid) =>
            Match(invalid.ToFunc(), valid.ToFunc());

        public IEnumerable<T> AsEnumerable() => _value.AsEnumerable();

        public override bool Equals(object obj) =>
            obj is Result<T> other &&
            IsValid == other.IsValid &&
            (IsValid
                ? _value.Equals(other._value)
                : _errors.SequenceEqual(other._errors));

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() =>
            Match(
                invalid: errors => $"Invalid([{string.Join(", ", errors)}])",
                valid: value => $"Valid({value})");
    }
}