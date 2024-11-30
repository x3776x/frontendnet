using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet.Controllers;

[Authorize (Roles = "Administrador")]
public class CategoriasController : Controller {
    private readonly CategoriasClientService _categoriasClientService;

    public CategoriasController (CategoriasClientService categoriasClientService) {
        _categoriasClientService = categoriasClientService;
    }

    public async Task<IActionResult> Index () {
        List<Categoria>? lista = new ();

        try {
            lista = await _categoriasClientService.GetAsync ();
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        return View (lista);
    }

    public async Task<IActionResult> Detalle (int id) {
        Categoria? item = null;

        try {
            item = await _categoriasClientService.GetAsync (id);
            if (item == null)
                return NotFound ();
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        return View (item);
    }

    public IActionResult Crear () {
        return View ();
    }

    [HttpPost]
    public async Task<IActionResult> Crear (Categoria newItem) {
        if (ModelState.IsValid) {
            try {
                await _categoriasClientService.PostAsync (newItem);
                return RedirectToAction (nameof (Index));
            } catch (HttpRequestException ex) {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction ("Salir", "Auth");
            }
        }

        ModelState.AddModelError ("Nombre", "No ha sido posible realizar la acción. Inténtelo nuevamente.");
        return View (newItem);
    }

    public async Task<IActionResult> Editar (int id) {
        Categoria? editItem = null;

        try {
            editItem = await _categoriasClientService.GetAsync (id);
            if (editItem == null)
                return NotFound ();
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        return View (editItem);
    }

    [HttpPost]
    public async Task<IActionResult> Editar (int id, Categoria editItem) {
        if (id != editItem.CategoriaId)
            return NotFound ();

        if (ModelState.IsValid) {
            try {
                await _categoriasClientService.PutAsync (editItem);
                return RedirectToAction (nameof (Index));
            } catch (HttpRequestException ex) {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction ("Salir", "Auth");
            }
        }

        ModelState.AddModelError ("Nombre", "No ha sido posible realizar la acción. Inténtelo nuevamente.");
        return View (editItem);
    }

    public async Task<IActionResult> Eliminar (int id, bool? showError = false) {
        Categoria? deleteItem = null;

        try {
            deleteItem = await _categoriasClientService.GetAsync (id);
            if (deleteItem == null)
                return NotFound ();

            if (showError.GetValueOrDefault ())
                ViewData["ErrorMessage"] = "No ha sido posible realizar la acción. Inténtelo nuevamente.";
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        return View (deleteItem);
    }

    [HttpPost]
    public async Task<IActionResult> EliminarConfirmado (int id) {
        try {
            await _categoriasClientService.DeleteAsync (id);
            return RedirectToAction (nameof (Index));
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        return RedirectToAction (nameof (Eliminar), new {
            id,
            showError = true
        });
    }
}
