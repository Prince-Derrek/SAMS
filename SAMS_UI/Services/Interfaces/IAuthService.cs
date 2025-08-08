using SAMS_UI.DTOs;
using System.Security.Claims;

public interface IAuthService
{
    Task<ClaimsPrincipal?> AuthenticateAndSignInAsync(LoginDTO dto, HttpContext httpContext);
}
