using Microsoft.AspNetCore.Mvc;
using ServicioEB2;
using EurekaWeb.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EurekaWeb.Controllers
{
    public class MovimientosController : Controller
    {
        private readonly CoreBancarioSoapClient _soap;
        public MovimientosController(CoreBancarioSoapClient soap) => _soap = soap;

        [HttpGet]
        public async Task<IActionResult> Index(string cuenta)
        {
            if (string.IsNullOrWhiteSpace(cuenta))
                return RedirectToAction("Menu", "Home");

            cuenta = cuenta.Trim();
            if (cuenta.Length < 8) cuenta = cuenta.PadLeft(8, '0');

            var datos = await _soap.TraerMovimientosAsync(cuenta);

            System.Diagnostics.Debug.WriteLine($"[Movimientos] Cuenta={cuenta}  Cantidad={datos?.Length ?? 0}");

            var vm = new MovimientosVm
            {
                Cuenta = cuenta,
                Movs = (datos ?? System.Array.Empty<Movimiento>())
                       .Select(MovimientosVm.Item.FromDto)
                       .ToList()
            };
            return View(vm);
        }
    }
}
