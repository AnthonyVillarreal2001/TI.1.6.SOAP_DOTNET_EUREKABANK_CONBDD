using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ec.edu.monster.models;

namespace ec.edu.monster.services
{
    /// <summary>
    /// Cliente SOAP contra el servidor .NET (CoreWCF) de la carpeta 01.SERVIDOR.
    /// Usa BasicHttpBinding apuntando a /CoreBancario.svc.
    /// </summary>
    public class SoapEurekaApi : IEurekaApi, IDisposable
    {
        private readonly ChannelFactory<ICoreBancarioSoap> _factory;
        private readonly ICoreBancarioSoap _cli;

        [ServiceContract(Namespace = "urn:eurekabank", Name = "CoreBancarioSoap")]
        private interface ICoreBancarioSoap
        {
            [OperationContract]
            Task<string> ValidarIngresoAsync(string usuario, string password);

            [OperationContract]
            Task<MovimientoDto[]> TraerMovimientosAsync(string cuenta);

            [OperationContract]
            Task<OperacionCuentaResponseDto> RegDepositoAsync(string cuenta, decimal importe, string empleado);

            [OperationContract]
            Task<OperacionCuentaResponseDto> RegRetiroAsync(string cuenta, decimal importe, string empleado);

            [OperationContract]
            Task<OperacionCuentaResponseDto> RegTransferenciaAsync(string cuentaOrigen, string cuentaDestino, decimal importe, string empleado);
        }

        [DataContract(Namespace = "urn:eurekabank")]
        private class MovimientoDto
        {
            [DataMember(Order = 1)] public string CuentaCodigo { get; set; } = string.Empty;
            [DataMember(Order = 2)] public int Numero { get; set; }
            [DataMember(Order = 3)] public DateTime Fecha { get; set; }
            [DataMember(Order = 4)] public string EmpleadoCodigo { get; set; } = string.Empty;
            [DataMember(Order = 5)] public string TipoCodigo { get; set; } = string.Empty;
            [DataMember(Order = 6)] public decimal Importe { get; set; }
            [DataMember(Order = 7)] public string? ReferenciaCuenta { get; set; }
        }

        [DataContract(Namespace = "urn:eurekabank")]
        private class OperacionCuentaResponseDto
        {
            [DataMember(Order = 1)] public int Estado { get; set; }
            [DataMember(Order = 2)] public decimal Saldo { get; set; }
        }

        public SoapEurekaApi(string baseUrl = null, bool trustLocalhostCertificate = true)
        {
            var url = string.IsNullOrWhiteSpace(baseUrl)
                ? "https://localhost:7299/CoreBancario.svc"
                : baseUrl.Trim();

            var address = new EndpointAddress(url);
            var binding = address.Uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
                ? new BasicHttpBinding(BasicHttpSecurityMode.Transport)
                : new BasicHttpBinding(BasicHttpSecurityMode.None);

            binding.MaxReceivedMessageSize = 10 * 1024 * 1024;
            binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;

            if (trustLocalhostCertificate &&
                (address.Uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                 address.Uri.Host.Equals("127.0.0.1")))
            {
                ServicePointManager.ServerCertificateValidationCallback = (_, __, ___, ____) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            }

            _factory = new ChannelFactory<ICoreBancarioSoap>(binding, address);
            _cli = _factory.CreateChannel();
        }

        public async Task<bool> ValidarIngreso(string usuario, string password)
        {
            var r = await _cli.ValidarIngresoAsync(usuario, password);
            return string.Equals(r, "Exitoso", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<List<Movimiento>> TraerMovimientos(string cuenta)
        {
            var list = new List<Movimiento>();
            var movs = await _cli.TraerMovimientosAsync(cuenta) ?? Array.Empty<MovimientoDto>();

            foreach (var m in movs)
            {
                var (tipo, accion) = MapTipoAccion(m.TipoCodigo);
                list.Add(new Movimiento
                {
                    Cuenta = m.CuentaCodigo,
                    NroMov = m.Numero,
                    Fecha = m.Fecha,
                    Tipo = string.IsNullOrWhiteSpace(m.TipoCodigo) ? m.TipoCodigo : tipo,
                    Accion = accion,
                    Importe = m.Importe
                });
            }
            return list;
        }

        public async Task<OperacionCuentaResponse> RegDeposito(string cuenta, decimal importe)
        {
            var r = await _cli.RegDepositoAsync(cuenta, importe, "0001");
            return MapOperacion(r);
        }

        public async Task<OperacionCuentaResponse> RegRetiro(string cuenta, decimal importe)
        {
            var r = await _cli.RegRetiroAsync(cuenta, importe, "0004");
            return MapOperacion(r);
        }

        public async Task<OperacionCuentaResponse> RegTransferencia(string origen, string destino, decimal importe)
        {
            var r = await _cli.RegTransferenciaAsync(origen, destino, importe, "0004");
            return MapOperacion(r);
        }

        public Task<string> ProbarConexion()
        {
            return Task.FromResult("OK");
        }

        private static OperacionCuentaResponse MapOperacion(OperacionCuentaResponseDto dto) =>
            new OperacionCuentaResponse
            {
                Estado = dto?.Estado ?? -1,
                Saldo = dto?.Saldo ?? -1
            };

        private static (string Tipo, string Accion) MapTipoAccion(string codigo) =>
            codigo switch
            {
                "003" => ("DEPOSITO", "INGRESO"),
                "004" => ("RETIRO", "SALIDA"),
                "008" => ("TRANSFERENCIA", "INGRESO"),
                "009" => ("TRANSFERENCIA", "SALIDA"),
                "001" => ("APERTURA", "INGRESO"),
                _ => ("", "")
            };

        public void Dispose()
        {
            try { (_cli as ICommunicationObject)?.Close(); }
            catch { (_cli as ICommunicationObject)?.Abort(); }
            try { _factory?.Close(); }
            catch { _factory?.Abort(); }
        }
    }
}
