using System.Security.Claims;

namespace WebIeltsFree.Models;

public static class ClaimsExtensions
{
    public static string GetFullName(this ClaimsPrincipal user)
    {
        if (!user.Identity.IsAuthenticated) return "Guest";

        var fullName = user.FindFirst("FullName")?.Value;
        if (!string.IsNullOrEmpty(fullName) && fullName != "User")
        {
            return fullName;
        }

        var name = user.FindFirst(ClaimTypes.Name)?.Value;
        if (!string.IsNullOrEmpty(name) && name != "User")
        {
            return name;
        }

        return user.FindFirst(ClaimTypes.Email)?.Value ?? "Guest";
    }

    public static string GetAvatar(this ClaimsPrincipal user)
    {
        if (!user.Identity.IsAuthenticated) return "";
        
        return user.FindFirst("Avatar")?.Value ?? "";
    }
}
