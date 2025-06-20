using System.Security.Claims;

namespace PurpleMallard.Products.Api.Features.ProductAssistant;

/// <summary>
/// Extension methods for ClaimsPrincipal to extract user information
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Extracts the user ID from claims, trying multiple claim types in order of preference
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal to extract the user ID from</param>
    /// <param name="fallbackUserId">The fallback user ID if no claims are found (default: "anonymous")</param>
    /// <returns>The user ID from claims or the fallback value</returns>
    public static string GetUserId(this ClaimsPrincipal principal, string fallbackUserId = "anonymous")
    {
        // Try different claim types in order of preference
        var userId = principal.FindFirst("UserId")?.Value 
                     ?? principal.FindFirst("sid")?.Value 
                     ?? principal.FindFirst("sub")?.Value 
                     ?? fallbackUserId;
        
        return userId;
    }
}
