using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace frontendnet.Controllers;

public class AuthController : Controller {
    private readonly AuthClientService _authClientService;

    public AuthController (AuthClientService authClientService) {
        _authClientService = authClientService;
    }

    [AllowAnonymous]
    public IActionResult Index () {
        return View ();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index (Login model) {
        if (ModelState.IsValid) {
            try {
                // Verifica en el backend que el correo y la contraseña sean válidos
                var token = await _authClientService.ObtenTokenAsync (model.Email, model.Password);
                var claims = new List<Claim>
                {
                    // Se guarda en la cookie
                    new Claim(ClaimTypes.Name, token.Email),
                    new Claim(ClaimTypes.GivenName, token.Nombre),
                    new Claim("jwt", token.Jwt),
                    new Claim(ClaimTypes.Role, token.Rol),
                };

                // Inicia la sesión del usuario
                var claimsIdentity = new ClaimsIdentity (claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties {
                    IsPersistent = true, // Mantener sesión activa después de cerrar el navegador
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours (1) // Expiración de la sesión
                };

                await HttpContext.SignInAsync (
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal (claimsIdentity),
                    authProperties);

                // Redirige según el rol del usuario
                if (token.Rol == "Administrador")
                    return RedirectToAction ("Index", "Productos");
                else
                    return RedirectToAction ("Index", "Home");
            } catch (Exception) {
                ModelState.AddModelError ("Email", "Credenciales no válidas. Inténtelo nuevamente.");
            }
        }

        return View (model);
    }

    [Authorize (Roles = "Administrador, Usuario")]
    public async Task<IActionResult> Salir () {
        // Cerrar sesión
        await HttpContext.SignOutAsync (CookieAuthenticationDefaults.AuthenticationScheme);

        // Redirigir a la página de inicio de sesión
        return RedirectToAction ("Index", "Auth");
    }
}
