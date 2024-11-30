using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet.Controllers;

[Authorize (Roles = "Usuario")]
public class CarritoController () : Controller {
    public IActionResult Index () {
        return View ();
    }
}
