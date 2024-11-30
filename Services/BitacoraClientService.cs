using frontendnet.Models;

namespace frontendnet.Services;

public class BitacoraClientService {
    private readonly HttpClient client;

    public BitacoraClientService (HttpClient client) {
        this.client = client;
    }

    public async Task<List<Bitacora>?> GetAsync () {
        return await client.GetFromJsonAsync<List<Bitacora>> ("api/bitacora");
    }
}
