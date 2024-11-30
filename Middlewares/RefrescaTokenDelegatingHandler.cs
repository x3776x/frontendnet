using frontendnet.Services;
using System.Security.Claims;

namespace frontendnet.Middlewares;

public class RefrescaTokenDelegatingHandler : DelegatingHandler {
    private readonly AuthClientService auth;
    private readonly IHttpContextAccessor httpContextAccessor;

    public RefrescaTokenDelegatingHandler (AuthClientService auth, IHttpContextAccessor httpContextAccessor) {
        this.auth = auth;
        this.httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, CancellationToken cancellationToken) {
        var response = await base.SendAsync (request, cancellationToken);
        response.EnsureSuccessStatusCode ();

        // Revisa si el servidor envió nuevo token
        if (response.Headers.Contains ("Set-Authorization")) {
            string jwt = response.Headers.GetValues ("Set-Authorization").FirstOrDefault ()!;
            var claims = new List<Claim>
            {
                // Todo se guarda en la cookie
                new Claim(ClaimTypes.Name, httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)!),
                new Claim(ClaimTypes.GivenName, httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.GivenName)!),
                new Claim("jwt", jwt),
                new Claim(ClaimTypes.Role, httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role)!),
            };
            await auth.IniciaSesionAsync (claims);
        }

        return response;
    }
}
