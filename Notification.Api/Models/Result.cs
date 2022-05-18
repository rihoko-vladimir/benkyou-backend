namespace Notification.Api.Models;

public struct Result<T>
{
    public T? Value { get; }
    public Exception? Exception { get; }
    public bool IsSuccess { get; }

    public Result(T value)
    {
        IsSuccess = true;
        Exception = null;
        Value = value;
    }

    public Result(Exception exception)
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        IsSuccess = false;
        Value = default;
    }
}

public struct Result
{
    public Exception? Exception { get; }
    public bool IsSuccess { get; }

    public Result(bool success)
    {
        IsSuccess = success;
        Exception = null;
    }

    public Result(Exception exception)
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        IsSuccess = false;
    }

    public static Result Success()
    {
        return new Result(true);
    }

    public static Result Error()
    {
        return new Result(false);
    }

    public static Result Error(Exception exception)
    {
        return new Result(exception);
    }

    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Error<T>(Exception exception)
    {
        return new Result<T>(exception);
    }
}