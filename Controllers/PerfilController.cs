using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace frontendnet.Controllers;

[Authorize]
public class PerfilController : Controller {
    private readonly PerfilClientService _perfilClientService;

    public PerfilController (PerfilClientService perfilClientService) {
        _perfilClientService = perfilClientService;
    }

    public async Task<IActionResult> IndexAsync () {
        AuthUser? usuario = null;
        try {
            ViewBag.timeRemaining = await _perfilClientService.ObtenTiempoAsync ();

            usuario = new AuthUser {
                Email = User.FindFirstValue (ClaimTypes.Name)!,
                Nombre = User.FindFirstValue (ClaimTypes.GivenName)!,
                Rol = User.FindFirstValue (ClaimTypes.Role)!,
                Jwt = User.FindFirstValue ("jwt")!,
            };
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        return View (usuario);
    }
}
