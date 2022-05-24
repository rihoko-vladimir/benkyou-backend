namespace Auth.Api.Models;

public struct Result<T>
{
    public T? Value { get; }
    public Exception? Exception { get; }

    public string Message { get; }
    public bool IsSuccess { get; }

    public Result(T value)
    {
        IsSuccess = true;
        Exception = null;
        Value = value;
        Message = string.Empty;
    }

    public Result(string message)
    {
        IsSuccess = true;
        Exception = null;
        Value = default;
        Message = message;
    }

    public Result(Exception exception)
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        IsSuccess = false;
        Value = default;
        Message = string.Empty;
    }
}

public struct Result
{
    public Exception? Exception { get; }

    public string Message { get; }
    public bool IsSuccess { get; }

    public Result(bool success)
    {
        IsSuccess = success;
        Exception = null;
        Message = string.Empty;
    }

    public Result(Exception exception)
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        IsSuccess = false;
        Message = string.Empty;
    }

    public Result(string message)
    {
        IsSuccess = false;
        Exception = null;
        Message = message;
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

    public static Result Error(string message)
    {
        return new Result(message);
    }

    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Error<T>(Exception exception)
    {
        return new Result<T>(exception);
    }

    public static Result<T> Error<T>(string message)
    {
        return new Result<T>(message);
    }
}