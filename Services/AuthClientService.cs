using frontendnet.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace frontendnet.Services;

public class AuthClientService {
    private readonly HttpClient client;
    private readonly IHttpContextAccessor httpContextAccessor;

    public AuthClientService (HttpClient client, IHttpContextAccessor httpContextAccessor) {
        this.client = client;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthUser> ObtenTokenAsync (string email, string password) {
        Login usuario = new Login () {
            Email = email,
            Password = password
        };

        // Llamada al Web API
        var response = await client.PostAsJsonAsync ("api/auth", usuario);
        var token = await response.Content.ReadFromJsonAsync<AuthUser> ();

        return token!;
    }

    public async Task IniciaSesionAsync (List<Claim> claims) {
        var claimsIdentity = new ClaimsIdentity (claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties ();

        await httpContextAccessor.HttpContext?
            .SignInAsync (CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal (claimsIdentity),
                        authProperties)!;
    }
}
