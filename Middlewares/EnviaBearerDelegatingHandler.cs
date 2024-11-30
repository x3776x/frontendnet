using System.Security.Claims;

namespace frontendnet.Middlewares;

public class EnviaBearerDelegatingHandler : DelegatingHandler {
    private readonly IHttpContextAccessor httpContextAccessor;

    public EnviaBearerDelegatingHandler (IHttpContextAccessor httpContextAccessor) {
        this.httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, CancellationToken cancellationToken) {
        var token = httpContextAccessor.HttpContext?.User.FindFirstValue ("jwt");
        if (!string.IsNullOrEmpty (token)) {
            request.Headers.Add ("Authorization", "Bearer " + token);
        }

        return base.SendAsync (request, cancellationToken);
    }
}
