using Microsoft.AspNetCore.Authorization;

namespace NetApiRaiden1.Api.Infrastructure.Authorization;

public static class Policies
{
    /// <summary>
    /// Function for add authorization policies
    /// </summary>
    /// <param name="options">Authorization options</param>
    internal static void AddPolicies(AuthorizationOptions options)
    {
        ////services.AddAuthorization(options =>
        ////{
        ////    options.AddPolicy(AuthorizationPolicies.AdminUsers, policy => policy
        ////        .RequireAuthenticatedUser()
        ////        .RequireClaim(
        ////            ClaimTypes.Roles,
        ////            "admin"));
        ////});
    }
}
