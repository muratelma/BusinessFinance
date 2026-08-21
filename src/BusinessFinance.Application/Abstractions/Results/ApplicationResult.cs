namespace BusinessFinance.Application.Abstractions.Results;

public sealed class ApplicationResult<T>
    where T : notnull
{
    private readonly T? _value;
    private readonly ApplicationError? _error;

    public bool IsSuccess { get; }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("A failed result does not contain a value.");

    public ApplicationError Error => !IsSuccess
        ? _error!
        : throw new InvalidOperationException("A successful result does not contain an error.");

    private ApplicationResult(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        IsSuccess = true;
        _value = value;
    }

    private ApplicationResult(ApplicationError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        IsSuccess = false;
        _error = error;
    }

    public static ApplicationResult<T> Success(T value)
    {
        return new ApplicationResult<T>(value);
    }

    public static ApplicationResult<T> Failure(ApplicationError error)
    {
        return new ApplicationResult<T>(error);
    }
}
