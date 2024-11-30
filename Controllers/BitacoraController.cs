using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet.Controllers;

[Authorize (Roles = "Administrador")]
public class BitacoraController : Controller {
    private readonly BitacoraClientService _bitacoraClientService;

    public BitacoraController (BitacoraClientService bitacoraClientService) {
        _bitacoraClientService = bitacoraClientService;
    }

    public async Task<IActionResult> Index () {
        List<Bitacora>? lista = new List<Bitacora> ();

        try {
            lista = await _bitacoraClientService.GetAsync ();
        } catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
            return RedirectToAction ("Salir", "Auth");
        } catch (Exception ex) {
            // Manejo de errores genéricos (opcional, según necesidades)
            ModelState.AddModelError (string.Empty, $"Ocurrió un error: {ex.Message}");
        }

        return View (lista);
    }
}
