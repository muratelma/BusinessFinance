namespace BusinessFinance.Application.Authentication;

public sealed record IdentityRegistrationResult(
    IdentityRegistrationStatus Status,
    Guid? UserId,
    string? Email);
