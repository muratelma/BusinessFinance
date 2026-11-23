using BusinessFinance.Api.Errors;
using BusinessFinance.Application.Profiles;

namespace BusinessFinance.Api.Features.Profiles;

public sealed record UserProfileResponse(bool HasBusiness);

public sealed record UpdateUserProfileRequest(bool HasBusiness);

public static class UserProfileEndpoints
{
    public static IEndpointRouteBuilder MapUserProfileEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/profile", GetAsync)
            .WithTags("Profile")
            .WithName("GetUserProfile")
            .WithSummary("Reads the authenticated user's onboarding answer")
            .WithDescription(
                "Tells the client whether the scope dimension should be shown at all.")
            .RequireAuthorization()
            .Produces<UserProfileResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        endpoints.MapPut("/api/v1/profile", UpdateAsync)
            .WithTags("Profile")
            .WithName("UpdateUserProfile")
            .WithSummary("Changes the onboarding answer")
            .WithDescription(
                "Only affects what the client shows. Default categories are installed " +
                "once, for a user with none, and are never replaced afterwards.")
            .RequireAuthorization()
            .Produces<UserProfileResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return endpoints;
    }

    private static async Task<IResult> GetAsync(
        GetUserProfileUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new UserProfileResponse(result.Value.HasBusiness))
            : result.Error.ToProblemResult(httpContext);
    }

    private static async Task<IResult> UpdateAsync(
        UpdateUserProfileRequest request,
        SetUserProfileUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new SetUserProfileCommand(request.HasBusiness),
            cancellationToken);
        return result.IsSuccess
            ? Results.Ok(new UserProfileResponse(result.Value.HasBusiness))
            : result.Error.ToProblemResult(httpContext);
    }
}
