using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ServicioEB2;
using System;
using System.Threading.Tasks;

namespace EurekaWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly CoreBancarioSoapClient _soap;
        public AccountController(CoreBancarioSoapClient soap) => _soap = soap;

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string usuario, string password)
        {
            var resultado = await _soap.ValidarIngresoAsync(usuario ?? "", password ?? "");
            if (string.Equals(resultado, "Exitoso", StringComparison.OrdinalIgnoreCase))
            {
                HttpContext.Session.SetString("usuario", usuario ?? "");
                return RedirectToAction("Menu", "Home");
            }
            ViewBag.Error = "Usuario o contraseña incorrectos.";
            return View();
        }
    }
}
