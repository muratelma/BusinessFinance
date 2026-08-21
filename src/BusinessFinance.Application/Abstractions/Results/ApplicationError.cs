namespace BusinessFinance.Application.Abstractions.Results;

public sealed record ApplicationError(
    string Code,
    string Message,
    ApplicationErrorType Type);
