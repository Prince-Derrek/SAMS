using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SAMS_UI.Data;
using SAMS_UI.DTOs;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ClaimsPrincipal?> AuthenticateAndSignInAsync(LoginDTO dto, HttpContext httpContext)
    {
        if (!Guid.TryParse(dto.Id, out var userId))
            return null; // Invalid ID format

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId && u.UserSecret == dto.UserSecret);

        if (user == null)
            return null;

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role?.Name ?? "")
    };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
        };

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties
        );

        return principal;
    }

}
