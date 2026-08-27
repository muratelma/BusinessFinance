using Microsoft.AspNetCore.Mvc;
using BusinessFinance.Api.Errors;
using BusinessFinance.Api.Extensions;
using BusinessFinance.Application.UserAccount;

namespace BusinessFinance.Api.Features.UserAccount;

public sealed record UserAccountResponse(
    Guid UserId,
    string Email,
    bool EmailConfirmed,
    DateTimeOffset CreatedAtUtc,
    int ActiveSessionCount);

public sealed record UserSessionResponse(
    Guid SessionId,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset ExpiresAtUtc);

public sealed record UserSessionListResponse(IReadOnlyList<UserSessionResponse> Items);

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public sealed record ChangePasswordApiResponse(
    Guid SessionId,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc);

public sealed record DeleteUserAccountRequest(string Password, bool Confirmed = false);

public static class UserAccountEndpoints
{
    public static IEndpointRouteBuilder MapUserAccountEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/v1/account")
            .WithTags("Account")
            .RequireAuthorization();

        group.MapGet("", GetAsync)
            .WithName("GetUserAccount")
            .WithSummary("Reads the authenticated user's own account")
            .Produces<UserAccountResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/sessions", ListSessionsAsync)
            .WithName("ListUserSessions")
            .WithSummary("Lists the user's open sessions")
            .WithDescription("Rows carry no token and no token hash.")
            .Produces<UserSessionListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapDelete("/sessions/{sessionId:guid}", RevokeSessionAsync)
            .WithName("RevokeUserSession")
            .WithSummary("Closes one open session")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // Parola ve hesap silme kimlik yollarıdır: giriş uçlarıyla aynı hız
        // sınırına girerler, yoksa parola denemesi buradan yapılabilirdi.
        group.MapPost("/password", ChangePasswordAsync)
            .WithName("ChangeUserPassword")
            .WithSummary("Changes the password and closes every session")
            .WithDescription(
                "Every refresh session is revoked; the calling device receives a fresh " +
                "token pair so it is not signed out.")
            .RequireRateLimiting(RateLimitingExtensions.AuthenticationPolicy)
            .Produces<ChangePasswordApiResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status429TooManyRequests);

        group.MapDelete("", DeleteAsync)
            .WithName("DeleteUserAccount")
            .WithSummary("Deletes the account and every record it owns")
            .WithDescription(
                "Irreversible. Requires the password and an explicit confirmation.")
            .RequireRateLimiting(RateLimitingExtensions.AuthenticationPolicy)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status429TooManyRequests);

        return endpoints;
    }

    private static async Task<IResult> GetAsync(
        GetUserAccountUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new UserAccountResponse(
                result.Value.UserId,
                result.Value.Email,
                result.Value.EmailConfirmed,
                result.Value.CreatedAtUtc,
                result.Value.ActiveSessionCount))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> ListSessionsAsync(
        ListUserSessionsUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new UserSessionListResponse(result.Value
                .Select(session => new UserSessionResponse(
                    session.SessionId,
                    session.CreatedAtUtc,
                    session.ExpiresAtUtc))
                .ToArray()))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> RevokeSessionAsync(
        Guid sessionId,
        RevokeUserSessionUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new RevokeUserSessionCommand(sessionId),
            cancellationToken);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> ChangePasswordAsync(
        ChangePasswordRequest request,
        ChangePasswordUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new ChangePasswordCommand(request.CurrentPassword, request.NewPassword),
            cancellationToken);

        return result.IsSuccess
            ? Results.Ok(new ChangePasswordApiResponse(
                result.Value.SessionId,
                result.Value.AccessToken,
                result.Value.AccessTokenExpiresAtUtc,
                result.Value.RefreshToken,
                result.Value.RefreshTokenExpiresAtUtc))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> DeleteAsync(
        [FromBody] DeleteUserAccountRequest request,
        DeleteUserAccountUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new DeleteUserAccountCommand(request.Password, request.Confirmed),
            cancellationToken);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToProblemResult(httpContext);
    }
}
