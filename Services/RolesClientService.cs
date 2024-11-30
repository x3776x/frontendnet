using frontendnet.Models;

namespace frontendnet.Services;

public class RolesClientService {
    private readonly HttpClient client;

    public RolesClientService (HttpClient client) {
        this.client = client;
    }

    public async Task<List<Rol>?> GetAsync () {
        return await client.GetFromJsonAsync<List<Rol>> ("api/roles");
    }
}
