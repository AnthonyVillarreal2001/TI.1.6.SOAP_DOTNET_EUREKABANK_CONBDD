using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _02.CLIMOV.Modelo;

namespace _02.CLIMOV.Servicio
{
    public interface ISoapService
    {
        // Métodos del servicio EurekaBank
        Task<bool> ProbarConexionAsync();
        Task<string> VerificarTablasAsync();
        Task<bool> ValidarIngresoAsync(string usuario, string password);
        Task<List<Movimiento>> TraerMovimientosAsync(string cuenta);
        Task<OperacionResponse> RegistrarDepositoAsync(string cuenta, double importe);
        Task<OperacionResponse> RegistrarRetiroAsync(string cuenta, double importe);
        Task<OperacionResponse> RegistrarTransferenciaAsync(string origen, string destino, double importe);
    }
}
