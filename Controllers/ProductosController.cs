using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace frontendnet.Controllers;

[Authorize (Roles = "Administrador")]
public class ProductosController : Controller {
    private readonly ProductosClientService _productosClientService;
    private readonly CategoriasClientService _categoriasClientService;
    private readonly ArchivosClientService _archivosClientService;
    private readonly IConfiguration _configuration;

    public ProductosController (ProductosClientService productosClientService,
                               CategoriasClientService categoriasClientService,
                               ArchivosClientService archivosClientService,
                               IConfiguration configuration) {
        _productosClientService = productosClientService;
        _categoriasClientService = categoriasClientService;
        _archivosClientService = archivosClientService;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index (string? s) {
        List<Producto>? lista = new List<Producto> ();

        try {
            lista = await _productosClientService.GetAsync (s);
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        if (User.FindFirstValue (ClaimTypes.Role) == "Administrador")
            ViewBag.SoloAdmin = true;

        ViewBag.Url = _configuration["UrlWebAPI"];
        ViewBag.search = s;

        return View (lista);
    }

    public async Task<IActionResult> Detalle (int id) {
        Producto? item = null;
        ViewBag.Url = _configuration["UrlWebAPI"];

        try {
            item = await _productosClientService.GetAsync (id);
            if (item == null)
                return NotFound ();
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        return View (item);
    }

    public async Task<IActionResult> Crear () {
        await ProductosDropDownListAsync ();
        return View ();
    }

    [HttpPost]
    public async Task<IActionResult> CrearAsync (Producto newItem) {
        ViewBag.Url = _configuration["UrlWebAPI"];

        try {
            await _productosClientService.PostAsync (newItem);
            return RedirectToAction (nameof (Index));
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        await ProductosDropDownListAsync ();
        ModelState.AddModelError ("Error", "No ha sido posible realizar la acción. Inténtelo nuevamente.");
        return View (newItem);
    }

    public async Task<IActionResult> EditarAsync (int id) {
        Producto? editItem = null;
        ViewBag.Url = _configuration["UrlWebAPI"];

        try {
            editItem = await _productosClientService.GetAsync (id);
            if (editItem == null)
                return NotFound ();
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        await ProductosDropDownListAsync ();
        return View (editItem);
    }

    [HttpPost]
    public async Task<IActionResult> EditarAsync (int id, Producto editItem) {
        if (id != editItem.ProductoId)
            return NotFound ();

        ViewBag.Url = _configuration["UrlWebAPI"];
        if (ModelState.IsValid) {
            try {
                await _productosClientService.PutAsync (editItem);
                return RedirectToAction (nameof (Index));
            } catch (HttpRequestException ex) {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction ("Salir", "Auth");
            }
        }

        await ProductosDropDownListAsync ();
        ModelState.AddModelError ("Nombre", "No ha sido posible realizar la acción. Inténtelo nuevamente.");
        return View (editItem);
    }

    public async Task<IActionResult> Eliminar (int id, bool? showError = false) {
        Producto? deleteItem = null;

        try {
            deleteItem = await _productosClientService.GetAsync (id);
            if (deleteItem == null)
                return NotFound ();
            if (showError.GetValueOrDefault ())
                ViewData["ErrorMessage"] = "No ha sido posible realizar la acción. Inténtelo nuevamente.";
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

        if (ModelState.IsValid) {
            try {
                await _productosClientService.DeleteAsync (id);
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

    [AcceptVerbs ("GET", "POST")]
    public IActionResult ValidaPoster (string poster) {
        if (Uri.IsWellFormedUriString (poster, UriKind.Absolute) || poster.Equals ("N/A"))
            return Json (true);
        return Json (false);
    }

    public async Task<IActionResult> Categorias (int id) {
        Producto? item = null;

        try {
            item = await _productosClientService.GetAsync (id);
            if (item == null)
                return NotFound ();
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        ViewBag["ProductoId"] = item?.ProductoId;
        return View (item);
    }

    public async Task<IActionResult> CategoriasAgregar (int id) {
        ProductoCategoria? item = null;

        try {
            Producto? producto = await _productosClientService.GetAsync (id);
            if (producto == null)
                return NotFound ();

            await CategoriasDropDownListAsync ();
            item = new ProductoCategoria { Producto = producto };
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        return View (item);
    }

    [HttpPost]
    public async Task<IActionResult> CategoriasAgregar (int id, int categoriaid) {
        Producto? producto = null;

        if (ModelState.IsValid) {
            try {
                producto = await _productosClientService.GetAsync (id);
                if (producto == null)
                    return NotFound ();

                Categoria? categoria = await _categoriasClientService.GetAsync (categoriaid);
                if (categoria == null)
                    return NotFound ();

                await _productosClientService.PostAsync (id, categoriaid);
                return RedirectToAction (nameof (Categorias), new {
                    id
                });
            } catch (HttpRequestException ex) {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction ("Salir", "Auth");
            }
        }

        ModelState.AddModelError ("Id", "No ha sido posible realizar la acción. Inténtelo nuevamente.");
        await CategoriasDropDownListAsync ();
        return View (new ProductoCategoria { Producto = producto });
    }

    public async Task<IActionResult> CategoriasRemover (int id, int categoriaid, bool? showError = false) {
        ProductoCategoria? deleteItem = null;

        try {
            Producto? producto = await _productosClientService.GetAsync (id);
            if (producto == null)
                return NotFound ();

            Categoria? categoria = await _categoriasClientService.GetAsync (categoriaid);
            if (categoria == null)
                return NotFound ();

            deleteItem = new ProductoCategoria {
                Producto = producto,
                CategoriaId = categoriaid,
                Nombre = categoria.Nombre
            };

            if (showError.GetValueOrDefault ())
                ViewData["ErrorMessage"] = "No ha sido posible realizar la acción. Inténtelo nuevamente.";
        } catch (HttpRequestException ex) {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction ("Salir", "Auth");
        }

        return View (deleteItem);
    }

    [HttpPost]
    public async Task<IActionResult> CategoriasRemover (int id, int categoriaid) {
        if (ModelState.IsValid) {
            try {
                await _productosClientService.DeleteAsync (id, categoriaid);
                return RedirectToAction (nameof (Categorias), new {
                    id
                });
            } catch (HttpRequestException ex) {
                if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction ("Salir", "Auth");
            }
        }

        return RedirectToAction (nameof (CategoriasRemover), new {
            id,
            categoriaid,
            showError = true
        });
    }

    private async Task CategoriasDropDownListAsync (object? itemSelected = null) {
        var listado = await _categoriasClientService.GetAsync ();
        ViewBag.categoria = new SelectList (listado, "CategoriaId", "Nombre", itemSelected);
    }

    private async Task ProductosDropDownListAsync (object? itemSelected = null) {
        var listado = await _archivosClientService.GetAsync ();
        ViewBag.Archivo = new SelectList (listado, "ArchivoId", "Nombre", itemSelected);
    }
}
