using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SAMS_UI.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ClaimsPrincipal?> AuthenticateAndSignInAsync(LoginDTO dto, HttpContext httpContext)
    {
        var client = _httpClientFactory.CreateClient("BackendApi");
        var requestUrl = $"/api/auth/login";
        var response = await client.PostAsJsonAsync(requestUrl, dto);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var apiResponse = await response.Content.ReadFromJsonAsync<TokenResponseDTO>();
        if (apiResponse is null || string.IsNullOrEmpty(apiResponse.Token))
            return null;

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(apiResponse.Token);

        var claims = jwtToken.Claims.ToList();

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
