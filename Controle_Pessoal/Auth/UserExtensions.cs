using System.Security.Claims;

namespace Controle_Pessoal.Auth;

public static class UserExtensions
{
    public static int GetUserId(this HttpContext httpContext)
    {   
        var nameIdentifierClaim = httpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier);
        return Convert.ToInt32(nameIdentifierClaim.Value);
    }
}