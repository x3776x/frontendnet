using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace frontendnet.Controllers;

[Authorize (Roles = "Administrador")]
public class UsuariosController : Controller {
    private readonly UsuariosClientService usuariosClientService;
    private readonly RolesClientService rolesClientService;

    public UsuariosController (UsuariosClientService usuariosClientService, RolesClientService rolesClientService) {
        this.usuariosClientService = usuariosClientService;
        this.rolesClientService = rolesClientService;
    }

    public async Task<IActionResult> Index () {
        List<Usuario>? lista = new List<Usuario> ();

        try {
            lista = await usuariosClientService.GetAsync ();
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        return View (lista);
    }

    public async Task<IActionResult> Detalle (string id) {
        Usuario? item = null;

        try {
            item = await usuariosClientService.GetAsync (id);
            if (item == null)
                return NotFound ();
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        return View (item);
    }

    public async Task<IActionResult> Crear () {
        await RolesDropDownListAsync ();
        return View ();
    }

    [HttpPost]
    public async Task<IActionResult> CrearAsync (UsuarioPwd newItem) {
        if (ModelState.IsValid) {
            try {
                await usuariosClientService.PostAsync (newItem);
                return RedirectToAction (nameof (Index));
            } catch (HttpRequestException ex) {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction ("Salir", "Auth");
            }
        }

        ModelState.AddModelError ("Email", "No ha sido posible realizar la acción. Inténtelo nuevamente.");
        await RolesDropDownListAsync ();
        return View (newItem);
    }

    [HttpGet ("[controller]/[action]/{email}")]
    public async Task<IActionResult> EditarAsync (string email) {
        Usuario? editItem = null;

        try {
            editItem = await usuariosClientService.GetAsync (email);
            if (editItem == null)
                return NotFound ();
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        ViewBag.PuedeEditar = !(User.Identity?.Name == email);
        await RolesDropDownListAsync (editItem?.Rol);
        return View (editItem);
    }

    [HttpPost ("[controller]/[action]/{email}")]
    public async Task<IActionResult> EditarAsync (string email, Usuario editItem) {
        if (email != editItem.Email)
            return NotFound ();

        if (ModelState.IsValid) {
            try {
                await usuariosClientService.PutAsync (editItem);
                return RedirectToAction (nameof (Index));
            } catch (HttpRequestException ex) {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction ("Salir", "Auth");
            }
        }

        ModelState.AddModelError ("Nombre", "No ha sido posible realizar la acción. Inténtelo nuevamente.");
        ViewBag.PuedeEditar = !(User.Identity?.Name == email);
        await RolesDropDownListAsync (editItem?.Rol);
        return View (editItem);
    }

    public async Task<IActionResult> Eliminar (string id, bool? showError = false) {
        Usuario? deleteItem = null;

        try {
            deleteItem = await usuariosClientService.GetAsync (id);
            if (deleteItem == null)
                return NotFound ();

            if (showError.GetValueOrDefault ())
                ViewData["ErrorMessage"] = "No ha sido posible realizar la acción. Inténtelo nuevamente.";
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        ViewBag.PuedeEditar = !(User.Identity?.Name == id);
        return View (deleteItem);
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar (string id) {
        if (ModelState.IsValid) {
            try {
                await usuariosClientService.DeleteAsync (id);
                return RedirectToAction (nameof (Index));
            } catch (HttpRequestException ex) {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction ("Salir", "Auth");
            }
        }

        return RedirectToAction (nameof (Eliminar), new {
            id,
            showError = true
        });
    }

    private async Task RolesDropDownListAsync (object? rolSeleccionado = null) {
        var listado = await rolesClientService.GetAsync ();
        ViewBag.Rol = new SelectList (listado, "Nombre", "Nombre", rolSeleccionado);
    }
}
