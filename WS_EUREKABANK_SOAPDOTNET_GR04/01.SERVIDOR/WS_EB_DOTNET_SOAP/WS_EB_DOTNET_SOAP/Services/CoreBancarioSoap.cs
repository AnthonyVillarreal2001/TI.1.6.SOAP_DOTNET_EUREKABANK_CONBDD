using CoreWCF;
using WS_EB_DOTNET_SOAP.Contracts;
using WS_EB_DOTNET_SOAP.Models;

namespace WS_EB_DOTNET_SOAP.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class CoreBancarioSoap : ICoreBancarioSoap
    {
        private readonly EurekaService _service;
        public CoreBancarioSoap(EurekaService service) => _service = service;

        public string ValidarIngreso(string usuario, string password)
            => _service.ValidarIngreso(usuario, password) ? "Exitoso" : "Denegado";

        public Movimiento[] TraerMovimientos(string cuenta)
            => _service.ListarMovimientos(cuenta).ToArray();

        public OperacionCuentaResponse RegDeposito(string cuenta, decimal importe, string? empleado)
        {
            if (importe <= 0) return new() { Estado = -1, Saldo = -1 };
            try
            {
                var saldo = _service.RegistrarDeposito(cuenta, importe, empleado ?? "0001");
                return new() { Estado = 1, Saldo = saldo };
            }
            catch
            {
                return new() { Estado = -1, Saldo = -1 };
            }
        }

        public OperacionCuentaResponse RegRetiro(string cuenta, decimal importe, string? empleado)
        {
            if (importe <= 0) return new() { Estado = -1, Saldo = -1 };
            try
            {
                var saldo = _service.RegistrarRetiro(cuenta, importe, empleado ?? "0004");
                return new() { Estado = 1, Saldo = saldo };
            }
            catch
            {
                return new() { Estado = -1, Saldo = -1 };
            }
        }

        public OperacionCuentaResponse RegTransferencia(string cuentaOrigen, string cuentaDestino, decimal importe, string? empleado)
        {
            if (importe <= 0) return new() { Estado = -1, Saldo = -1 };
            try
            {
                var saldoOrigen = _service.RegistrarTransferencia(cuentaOrigen, cuentaDestino, importe, empleado ?? "0004");
                return new() { Estado = 1, Saldo = saldoOrigen };
            }
            catch
            {
                return new() { Estado = -1, Saldo = -1 };
            }
        }
    }
}
