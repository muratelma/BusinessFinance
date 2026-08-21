using BusinessFinance.Api.Errors;
using BusinessFinance.Api.Extensions;
using BusinessFinance.Application.Authentication.LoginUser;
using BusinessFinance.Application.Authentication.Logout;
using BusinessFinance.Application.Authentication.RefreshTokens;
using BusinessFinance.Application.Authentication.RegisterUser;

namespace BusinessFinance.Api.Features.Authentication;

public static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/v1/auth")
            .WithTags("Authentication")
            .RequireRateLimiting(RateLimitingExtensions.AuthenticationPolicy);

        group.MapPost("/register", RegisterAsync)
            .WithName("RegisterUser")
            .Produces<RegisterResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status429TooManyRequests);
        group.MapPost("/login", LoginAsync)
            .WithName("LoginUser")
            .Produces<TokenPairResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status429TooManyRequests);
        group.MapPost("/refresh", RefreshAsync)
            .WithName("RefreshTokens")
            .Produces<RefreshTokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status429TooManyRequests);
        group.MapPost("/logout", LogoutAsync)
            .WithName("Logout")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status429TooManyRequests);

        return endpoints;
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        RegisterUserUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new RegisterUserCommand(request.Email, request.Password),
            cancellationToken);

        return result.IsSuccess
            ? Results.Created(
                $"/api/v1/auth/users/{result.Value.UserId}",
                new RegisterResponse(result.Value.UserId, result.Value.Email))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        LoginUserUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new LoginUserCommand(request.Email, request.Password),
            cancellationToken);

        return result.IsSuccess
            ? Results.Ok(new TokenPairResponse(
                result.Value.UserId,
                result.Value.Email,
                result.Value.AccessToken,
                result.Value.AccessTokenExpiresAtUtc,
                result.Value.RefreshToken,
                result.Value.RefreshTokenExpiresAtUtc))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> RefreshAsync(
        RefreshTokenRequest request,
        RefreshTokensUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new RefreshTokensCommand(request.RefreshToken),
            cancellationToken);

        return result.IsSuccess
            ? Results.Ok(new RefreshTokenResponse(
                result.Value.AccessToken,
                result.Value.AccessTokenExpiresAtUtc,
                result.Value.RefreshToken,
                result.Value.RefreshTokenExpiresAtUtc))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> LogoutAsync(
        LogoutRequest request,
        LogoutUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new LogoutCommand(request.RefreshToken),
            cancellationToken);

        return result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToProblemResult(httpContext);
    }
}
