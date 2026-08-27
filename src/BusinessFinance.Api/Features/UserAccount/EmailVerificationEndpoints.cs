using BusinessFinance.Api.Errors;
using BusinessFinance.Api.Extensions;
using BusinessFinance.Application.Verification;

namespace BusinessFinance.Api.Features.UserAccount;

public sealed record SendEmailVerificationResponseBody(bool AlreadyConfirmed, bool CodeSent);

public sealed record ConfirmEmailRequest(string Code);

public sealed record RequestPasswordResetRequest(string Email);

public sealed record ResetPasswordRequest(string Email, string Code, string NewPassword);

public static class EmailVerificationEndpoints
{
    public static IEndpointRouteBuilder MapEmailVerificationEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var account = endpoints
            .MapGroup("/api/v1/account/email-verification")
            .WithTags("Account")
            .RequireAuthorization()
            .RequireRateLimiting(RateLimitingExtensions.AuthenticationPolicy);

        account.MapPost("", SendAsync)
            .WithName("SendEmailVerification")
            .WithSummary("Sends a verification code to the user's own address")
            .WithDescription(
                "An unverified account is never locked; the code only clears a warning.")
            .Produces<SendEmailVerificationResponseBody>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status429TooManyRequests);

        account.MapPost("/confirm", ConfirmAsync)
            .WithName("ConfirmEmail")
            .WithSummary("Confirms the address with the emailed code")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status429TooManyRequests);

        // Sıfırlama yolu kimlik doğrulaması istemez: parolasını unutan kullanıcı
        // giriş yapamıyor. Bu yüzden auth uçlarıyla aynı hız sınırına girer.
        var reset = endpoints
            .MapGroup("/api/v1/auth/password-reset")
            .WithTags("Authentication")
            .RequireRateLimiting(RateLimitingExtensions.AuthenticationPolicy);

        reset.MapPost("", RequestResetAsync)
            .WithName("RequestPasswordReset")
            .WithSummary("Sends a reset code if the address has an account")
            .WithDescription(
                "Always succeeds: an unknown address and a known one answer the same.")
            .Produces(StatusCodes.Status202Accepted)
            .ProducesProblem(StatusCodes.Status429TooManyRequests);

        reset.MapPost("/confirm", ResetAsync)
            .WithName("ConfirmPasswordReset")
            .WithSummary("Sets a new password with the emailed code")
            .WithDescription("Every session is revoked when the password is reset.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status429TooManyRequests);

        return endpoints;
    }

    private static async Task<IResult> SendAsync(
        SendEmailVerificationUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new SendEmailVerificationResponseBody(
                result.Value.AlreadyConfirmed,
                result.Value.CodeSent))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> ConfirmAsync(
        ConfirmEmailRequest request,
        ConfirmEmailUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new ConfirmEmailCommand(request.Code),
            cancellationToken);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> RequestResetAsync(
        RequestPasswordResetRequest request,
        RequestPasswordResetUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new RequestPasswordResetCommand(request.Email),
            cancellationToken);

        return result.IsSuccess
            ? Results.Accepted()
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> ResetAsync(
        ResetPasswordRequest request,
        ResetPasswordUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new ResetPasswordCommand(request.Email, request.Code, request.NewPassword),
            cancellationToken);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToProblemResult(httpContext);
    }
}
