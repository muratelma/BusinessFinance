namespace BusinessFinance.Application.Authentication;

public sealed record AuthenticatedIdentity(Guid UserId, string Email);
