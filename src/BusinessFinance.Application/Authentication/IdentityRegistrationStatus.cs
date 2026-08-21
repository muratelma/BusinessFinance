namespace BusinessFinance.Application.Authentication;

public enum IdentityRegistrationStatus
{
    Succeeded = 1,
    DuplicateEmail = 2,
    InvalidPassword = 3,
    InvalidRegistration = 4
}
