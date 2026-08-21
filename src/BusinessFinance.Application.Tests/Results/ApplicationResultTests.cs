using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Accounts;

namespace BusinessFinance.Application.Tests.Results;

public sealed class ApplicationResultTests
{
    [Fact]
    public void Success_ContainsValueAndRejectsErrorAccess()
    {
        var result = ApplicationResult<string>.Success("created");

        Assert.True(result.IsSuccess);
        Assert.Equal("created", result.Value);
        Assert.Throws<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    public void Failure_ContainsErrorAndRejectsValueAccess()
    {
        var error = AccountErrors.NotFound(Guid.NewGuid());

        var result = ApplicationResult<string>.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void AccountErrors_ExposeStableApiMappableTypesAndCodes()
    {
        var accountId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        var errors = new[]
        {
            AccountErrors.Validation("Invalid input."),
            AccountErrors.AuthenticationRequired,
            AccountErrors.Forbidden,
            AccountErrors.NotFound(accountId),
            AccountErrors.DuplicateName("Daily Cash")
        };

        Assert.Equal(
            [
                ApplicationErrorType.Validation,
                ApplicationErrorType.Unauthorized,
                ApplicationErrorType.Forbidden,
                ApplicationErrorType.NotFound,
                ApplicationErrorType.Conflict
            ],
            errors.Select(error => error.Type));
        Assert.Equal(
            [
                "accounts.validation",
                "authentication.required",
                "accounts.forbidden",
                "accounts.not_found",
                "accounts.duplicate_name"
            ],
            errors.Select(error => error.Code));
    }
}
