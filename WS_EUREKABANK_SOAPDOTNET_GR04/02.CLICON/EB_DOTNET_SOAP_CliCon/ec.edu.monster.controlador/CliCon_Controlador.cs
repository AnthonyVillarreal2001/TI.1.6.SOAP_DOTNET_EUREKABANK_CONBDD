using EB_DOTNET_SOAP_CliCon.ec.edu.monster.modelo;
using EB_DOTNET_SOAP_CliCon.ServicioEB2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EB_DOTNET_SOAP_CliCon.ec.edu.monster.controlador
{
    public class CliCon_Controlador
    {
        // Cliente generado por el Connected Service
        private readonly CoreBancarioSoapClient servicio;

        public CliCon_Controlador()
        {
            // Usa el endpoint HTTPS del App.config (nombre: BasicHttpBinding_CoreBancarioSoap)
            servicio = new CoreBancarioSoapClient("BasicHttpBinding_CoreBancarioSoap");

            // Si quieres probar por HTTP, usa el otro endpoint:
            // servicio = new CoreBancarioSoapClient("BasicHttpBinding_CoreBancarioSoap1");
        }

        public string ValidarIngreso(string usuario, string password)
            => servicio.ValidarIngreso(usuario, password);

        public List<CliCon_Movimiento> ObtenerMovimientos(string cuenta)
        {
            var datos = servicio.TraerMovimientos(cuenta);
            var lista = new List<CliCon_Movimiento>();
            if (datos == null) return lista;

            foreach (var m in datos)
            {
                lista.Add(new CliCon_Movimiento
                {
                    Cuenta = m.CuentaCodigo,
                    NroMov = m.Numero,
                    Fecha = m.Fecha,
                    // AHORA: muestra la descripción y la acción según la tabla
                    Tipo = TipoDescripcion(m.TipoCodigo),
                    Accion = TipoAccion(m.TipoCodigo),
                    Importe = m.Importe
                });
            }
            return lista;
        }

        private static string TipoDescripcion(string codigo)
        {
            switch (codigo)
            {
                case "001": return "Apertura de Cuenta";
                case "002": return "Cancelar Cuenta";
                case "003": return "Deposito";
                case "004": return "Retiro";
                case "005": return "Interes";
                case "006": return "Mantenimiento";
                case "007": return "ITF";
                case "008": return "Transferencia";
                case "009": return "Transferencia";
                case "010": return "Cargo por Movimiento";
                default: return string.IsNullOrEmpty(codigo) ? "" : codigo;
            }
        }

        private static string TipoAccion(string codigo)
        {
            switch (codigo)
            {
                case "001": return "INGRESO";
                case "002": return "SALIDA";
                case "003": return "INGRESO";
                case "004": return "SALIDA";
                case "005": return "INGRESO";
                case "006": return "SALIDA";
                case "007": return "SALIDA";
                case "008": return "INGRESO";
                case "009": return "SALIDA";
                case "010": return "SALIDA";
                default: return "";
            }
        }

        public OperacionCuentaResponse RegistrarDeposito(string cuenta, decimal importe)
        {
            var r = servicio.RegDeposito(cuenta, importe, "0001");
            return new OperacionCuentaResponse { estado = r.Estado, saldo = r.Saldo };
        }

        public OperacionCuentaResponse RegistrarRetiro(string cuenta, decimal importe)
        {
            var r = servicio.RegRetiro(cuenta, importe, "0004");
            return new OperacionCuentaResponse { estado = r.Estado, saldo = r.Saldo };
        }

        public OperacionCuentaResponse RegistrarTransferencia(string cuentaOrigen, string cuentaDestino, decimal importe)
        {
            var r = servicio.RegTransferencia(cuentaOrigen, cuentaDestino, importe, "0004");
            return new OperacionCuentaResponse { estado = r.Estado, saldo = r.Saldo };
        }

        // CliCon_Controlador.cs (fragmento)
        private static string CodigoATexto(string tipo)
        {
            switch (tipo)
            {
                case "001": return "Apertura";
                case "003": return "Depósito";
                case "004": return "Retiro";
                case "008": return "Abono";
                case "009": return "Cargo";
                default: return tipo ?? "";
            }
        }



    }

    // Clase de respuesta para mantener tu UI tal cual
    public class OperacionCuentaResponse
    {
        public int estado { get; set; }
        public decimal saldo { get; set; }
    }
}
