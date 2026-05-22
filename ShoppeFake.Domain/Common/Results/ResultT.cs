namespace ShoppeFake.Domain.Common.Results
{
    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(T value)
            : base(true, Error.None)
        {
            Value = value;
        }

        private Result(Error error)
            : base(false, error)
        {
            Value = default;
        }

        public static Result<T> Success(T value)
            => new(value);

        public static Result<T> Fail(Error error)
            => new(error);
        public static Result<T> Fail(string code, string message)
            => new(new Error(code, message));
    }
}