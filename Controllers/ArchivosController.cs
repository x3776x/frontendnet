using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet.Controllers;

[Authorize (Roles = "Administrador")]
public class ArchivosController : Controller {
    private readonly ArchivosClientService _archivosClientService;
    private readonly IConfiguration _configuration;

    public ArchivosController (ArchivosClientService archivosClientService, IConfiguration configuration) {
        _archivosClientService = archivosClientService;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index () {
        List<Archivo>? lista = new ();

        try {
            lista = await _archivosClientService.GetAsync ();
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        ViewBag.Url = _configuration["UrlWebAPI"];
        return View (lista);
    }

    public async Task<IActionResult> Detalle (int id) {
        Archivo? item = null;
        try {
            item = await _archivosClientService.GetAsync (id);
            if (item == null)
                return NotFound ();
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        ViewBag.Url = _configuration["UrlWebAPI"];
        return View (item);
    }

    public IActionResult Crear () {
        return View ();
    }

    [HttpPost]
    public async Task<IActionResult> Crear (Upload newItem) {
        ViewBag.Url = _configuration["UrlWebAPI"];

        if (ModelState.IsValid) {
            try {
                if ((newItem.Portada.Length / 1024) > 100) {
                    ModelState.AddModelError ("Portada", $"El archivo de {newItem.Portada.Length / 1024} KB supera el tamaño máximo permitido.");
                    return View (newItem);
                }

                if (newItem.Portada.ContentType != "image/jpeg") {
                    ModelState.AddModelError ("Portada", $"El archivo {newItem.Portada.FileName} no tiene una extensión válida.");
                    return View (newItem);
                }

                await _archivosClientService.PostAsync (newItem);
                return RedirectToAction (nameof (Index));
            } catch (HttpRequestException ex) {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction ("Salir", "Auth");
            }
        }

        ModelState.AddModelError ("Portada", "No ha sido posible realizar la acción. Inténtelo nuevamente.");
        return View (newItem);
    }

    public async Task<IActionResult> Editar (int id) {
        ViewBag.Url = _configuration["UrlWebAPI"];

        try {
            Archivo? editItem = await _archivosClientService.GetAsync (id);
            if (editItem == null)
                return NotFound ();

            ViewBag.ArchivoId = editItem.ArchivoId;
            ViewBag.Nombre = editItem.Nombre;

            return View (editItem);
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        return View ();
    }

    [HttpPost]
    public async Task<IActionResult> Editar (int id, Upload editItem) {
        if (id != editItem.ArchivoId)
            return NotFound ();

        ViewBag.Url = _configuration["UrlWebAPI"];

        if (ModelState.IsValid) {
            try {
                if ((editItem.Portada.Length / 1024) > 100) {
                    ModelState.AddModelError ("Portada", $"El archivo de {editItem.Portada.Length / 1024} KB supera el tamaño máximo permitido.");
                    return View (editItem);
                }

                if (editItem.Portada.ContentType != "image/jpeg") {
                    ModelState.AddModelError ("Portada", $"El archivo {editItem.Portada.FileName} no tiene una extensión válida.");
                    return View (editItem);
                }

                await _archivosClientService.PutAsync (editItem);
                return RedirectToAction (nameof (Index));
            } catch (HttpRequestException ex) {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction ("Salir", "Auth");
            }
        }

        ModelState.AddModelError ("Portada", "No ha sido posible realizar la acción. Inténtelo nuevamente.");
        return View (editItem);
    }

    public async Task<IActionResult> Eliminar (int id, bool? showError = false) {
        Archivo? deleteItem = null;
        try {
            deleteItem = await _archivosClientService.GetAsync (id);
            if (deleteItem == null)
                return NotFound ();

            if (showError.GetValueOrDefault ())
                ViewData["ErrorMessage"] = "No ha sido posible realizar la acción. Inténtelo más tarde.";
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        ViewBag.Url = _configuration["UrlWebAPI"];
        return View (deleteItem);
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar (int id) {
        ViewBag.Url = _configuration["UrlWebAPI"];

        try {
            await _archivosClientService.DeleteAsync (id);
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