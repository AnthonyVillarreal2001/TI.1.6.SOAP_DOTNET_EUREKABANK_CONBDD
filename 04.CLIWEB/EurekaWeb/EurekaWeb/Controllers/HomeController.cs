using Microsoft.AspNetCore.Mvc;
using ServicioEB2;
using EurekaWeb.Models;

public class HomeController : Controller
{
    private readonly CoreBancarioSoapClient _soap;
    public HomeController(CoreBancarioSoapClient soap) => _soap = soap;

    [HttpGet]
    public IActionResult Menu()
    {
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario)) return RedirectToAction("Login", "Account");
        ViewBag.Usuario = usuario;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Operacion(string action, string? cuenta, string? cuentaOrigen, string? cuentaDestino, decimal importe)
    {
        // seguridad de sesión (lo hacías en JSP)
        var usuario = HttpContext.Session.GetString("usuario");
        if (string.IsNullOrEmpty(usuario)) return RedirectToAction("Login", "Account");

        // normalizaciones
        string Pad8(string? c) => string.IsNullOrWhiteSpace(c) ? "" : c.Trim().PadLeft(8, '0');

        var vm = new ResultadoVm();
        try
        {
            switch ((action ?? "").ToLowerInvariant())
            {
                case "deposito":
                    {
                        var r = await _soap.RegDepositoAsync(Pad8(cuenta), importe, null);
                        vm.EsExitoso = r.Estado == 1;
                        vm.Saldo = r.Saldo;
                        vm.Cuenta = Pad8(cuenta);
                        vm.Mensaje = vm.EsExitoso ? "Depósito realizado correctamente." : "Error al realizar el depósito.";
                        break;
                    }
                case "retiro":
                    {
                        var r = await _soap.RegRetiroAsync(Pad8(cuenta), importe, null);
                        vm.EsExitoso = r.Estado == 1;
                        vm.Saldo = r.Saldo;
                        vm.Cuenta = Pad8(cuenta);
                        vm.Mensaje = vm.EsExitoso ? "Retiro realizado correctamente." : "Error al realizar el retiro.";
                        break;
                    }
                case "transferencia":
                    {
                        var r = await _soap.RegTransferenciaAsync(Pad8(cuentaOrigen), Pad8(cuentaDestino), importe, null);
                        vm.EsExitoso = r.Estado == 1;
                        vm.Saldo = r.Saldo;              // saldo de origen según tu servicio
                        vm.Cuenta = Pad8(cuentaOrigen);
                        vm.Mensaje = vm.EsExitoso ? "Transferencia realizada correctamente." : "Error al realizar la transferencia.";
                        break;
                    }
                default:
                    vm.EsExitoso = false;
                    vm.Mensaje = "Acción no válida.";
                    break;
            }
        }
        catch (Exception ex)
        {
            vm.EsExitoso = false;
            vm.Mensaje = $"Error: {ex.Message}";
        }

        return View("Resultado", vm); // -> View Razor que replica tu JSP
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }
}
