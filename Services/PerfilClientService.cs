namespace frontendnet.Services;

public class PerfilClientService {
    private readonly HttpClient client;

    public PerfilClientService (HttpClient client) {
        this.client = client;
    }

    public async Task<string> ObtenTiempoAsync () {
        return await client.GetStringAsync ($"api/auth/tiempo");
    }
}
