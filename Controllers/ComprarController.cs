using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet.Controllers;

[Authorize (Roles = "Usuario")]
public class ComprarController : Controller {
    private readonly ProductosClientService _productosClientService;
    private readonly IConfiguration _configuration;

    public ComprarController (ProductosClientService productosClientService, IConfiguration configuration) {
        _productosClientService = productosClientService;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index (string? s) {
        List<Producto>? lista = new ();

        try {
            lista = await _productosClientService.GetAsync (s);
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        ViewBag.Url = _configuration["UrlWebAPI"];
        ViewBag.search = s;
        return View (lista);
    }
}
