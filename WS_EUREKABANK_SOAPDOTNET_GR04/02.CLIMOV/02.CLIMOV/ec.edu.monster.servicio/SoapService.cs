using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using _02.CLIMOV.Modelo;

namespace _02.CLIMOV.Servicio
{
    public class SoapService : ISoapService
    {
        private readonly string _endpointUrl;
        private readonly HttpClient _httpClient;

        public SoapService()
        {
            // 🔧 CONFIGURACIÓN DEL ENDPOINT SOAP
            // ===================================
            
            // 📱 Servidor HTTPS según WSDL
            _endpointUrl = "https://10.40.27.236:7299/CoreBancario.svc";
            
            // ALTERNATIVA HTTP (sin SSL):
            // _endpointUrl = "http://localhost:5070/soap/CoreBancario.svc";

            // Configurar HttpClient con SSL
            var handler = new HttpClientHandler();
            
            // Bypass de validación SSL (solo para desarrollo)
            handler.ServerCertificateCustomValidationCallback = 
                (message, cert, chain, sslPolicyErrors) => true;
            
            // Deshabilitar compresión automática que puede causar problemas
            handler.AutomaticDecompression = System.Net.DecompressionMethods.None;
            
            _httpClient = new HttpClient(handler);
            _httpClient.Timeout = TimeSpan.FromSeconds(60); // Aumentar timeout a 60 segundos
            
            System.Diagnostics.Debug.WriteLine($"🔧 SoapService configurado para: {_endpointUrl}");
        }

        private async Task<string> CallSoapServiceAsync(string methodName, Dictionary<string, string> parameters)
        {
            var soapEnvelope = BuildSoapEnvelope(methodName, parameters);
            
            try
            {
                // Configurar el contenido con el tipo MIME correcto para SOAP
                var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
                
                // Configurar SOAPAction específico para cada método
                var request = new HttpRequestMessage(HttpMethod.Post, _endpointUrl)
                {
                    Content = content
                };
                
                // SOAPAction debe estar entre comillas dobles según el estándar SOAP 1.1
                request.Headers.Add("SOAPAction", $"\"urn:eurekabank/CoreBancarioSoap/{methodName}\"");
                
                System.Diagnostics.Debug.WriteLine($"=== SOAP REQUEST ===");
                System.Diagnostics.Debug.WriteLine($"🌐 URL: {_endpointUrl}");
                System.Diagnostics.Debug.WriteLine($"📤 SOAPAction: \"urn:eurekabank/CoreBancarioSoap/{methodName}\"");
                System.Diagnostics.Debug.WriteLine($"📦 Envelope:\n{soapEnvelope}");
                
                System.Diagnostics.Debug.WriteLine($"⏳ Enviando request...");
                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                System.Diagnostics.Debug.WriteLine($"✅ Response recibida");
                
                System.Diagnostics.Debug.WriteLine($"📥 Status Code: {response.StatusCode}");
                
                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"📄 Response Length: {responseContent.Length} bytes");
                
                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error Response:\n{responseContent}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Success Response:\n{responseContent}");
                }
                
                response.EnsureSuccessStatusCode();
                return responseContent;
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Timeout en llamada SOAP: {ex.Message}");
                throw new Exception($"Timeout: El servidor no respondió a tiempo. Verifique que el servidor esté funcionando correctamente.", ex);
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error HTTP en llamada SOAP: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Inner Exception: {ex.InnerException.Message}");
                }
                throw new Exception($"No se puede conectar al servidor SOAP: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error en llamada SOAP: {ex.Message}");
                throw;
            }
        }

        private string BuildSoapEnvelope(string methodName, Dictionary<string, string> parameters)
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");
            sb.Append("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" ");
            sb.Append("xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            sb.Append("<soap:Body>");
            sb.Append($"<{methodName} xmlns=\"urn:eurekabank\">");
            
            foreach (var param in parameters)
            {
                sb.Append($"<{param.Key}>{System.Security.SecurityElement.Escape(param.Value)}</{param.Key}>");
            }
            
            sb.Append($"</{methodName}>");
            sb.Append("</soap:Body>");
            sb.Append("</soap:Envelope>");
            
            return sb.ToString();
        }

        public async Task<bool> ProbarConexionAsync()
        {
            try
            {
                var response = await CallSoapServiceAsync("probarConexion", new Dictionary<string, string>());
                return response.Contains("CONEXIÓN EXITOSA");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al probar conexión: {ex.Message}");
                return false;
            }
        }

        public async Task<string> VerificarTablasAsync()
        {
            try
            {
                var response = await CallSoapServiceAsync("verificarTablas", new Dictionary<string, string>());
                var xdoc = XDocument.Parse(response);
                var ns = XNamespace.Get("http://tempuri.org/");
                return xdoc.Descendants(ns + "verificarTablasResult").FirstOrDefault()?.Value ?? "Error";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al verificar tablas: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        public async Task<bool> ValidarIngresoAsync(string usuario, string password)
        {
            try
            {
                // Debug: Mostrar qué estamos enviando
                System.Diagnostics.Debug.WriteLine($"=== VALIDAR INGRESO ===");
                System.Diagnostics.Debug.WriteLine($"Usuario enviado: '{usuario}'");
                System.Diagnostics.Debug.WriteLine($"Password enviado: '{password}'");
                
                var parameters = new Dictionary<string, string>
                {
                    { "usuario", usuario },
                    { "password", password }
                };
                
                // Debug: Mostrar el SOAP que se enviará
                var soapEnvelope = BuildSoapEnvelope("ValidarIngreso", parameters);
                System.Diagnostics.Debug.WriteLine($"SOAP Request:\n{soapEnvelope}");
                
                var response = await CallSoapServiceAsync("ValidarIngreso", parameters);
                System.Diagnostics.Debug.WriteLine($"SOAP Response:\n{response}");
                
                var xdoc = XDocument.Parse(response);
                
                // Buscar en todos los namespaces posibles
                var result = xdoc.Descendants()
                    .Where(x => x.Name.LocalName == "ValidarIngresoResult")
                    .FirstOrDefault()?.Value;
                
                System.Diagnostics.Debug.WriteLine($"Resultado parseado: '{result}'");
                System.Diagnostics.Debug.WriteLine($"¿Es 'Exitoso'? {result == "Exitoso"}");
                
                return result == "Exitoso";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al validar ingreso: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<List<Movimiento>> TraerMovimientosAsync(string cuenta)
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "cuenta", cuenta }
                };
                
                var response = await CallSoapServiceAsync("TraerMovimientos", parameters);
                System.Diagnostics.Debug.WriteLine($"TraerMovimientos Response: {response}");
                
                var movimientos = new List<Movimiento>();
                var xdoc = XDocument.Parse(response);
                
                // Buscar elementos "Movimiento" (con mayúscula, según el WSDL real)
                foreach (var mov in xdoc.Descendants().Where(x => x.Name.LocalName == "Movimiento"))
                {
                    // Los campos según la respuesta real del servidor:
                    // CuentaCodigo, Numero, Fecha, TipoCodigo, Importe
                    var cuentaCodigo = mov.Elements().FirstOrDefault(x => x.Name.LocalName == "CuentaCodigo")?.Value ?? "";
                    var numero = mov.Elements().FirstOrDefault(x => x.Name.LocalName == "Numero")?.Value ?? "0";
                    var fecha = mov.Elements().FirstOrDefault(x => x.Name.LocalName == "Fecha")?.Value ?? DateTime.Now.ToString();
                    var tipoCodigo = mov.Elements().FirstOrDefault(x => x.Name.LocalName == "TipoCodigo")?.Value ?? "";
                    var importe = mov.Elements().FirstOrDefault(x => x.Name.LocalName == "Importe")?.Value ?? "0";
                    
                    movimientos.Add(new Movimiento
                    {
                        Cuenta = cuentaCodigo,
                        NroMov = int.Parse(numero),
                        Fecha = DateTime.Parse(fecha),
                        Tipo = tipoCodigo,
                        Accion = ObtenerAccionPorTipo(tipoCodigo), // Mapear tipo a acción
                        Importe = double.Parse(importe, System.Globalization.CultureInfo.InvariantCulture)
                    });
                }
                
                System.Diagnostics.Debug.WriteLine($"TraerMovimientos encontró {movimientos.Count} movimientos");
                return movimientos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al traer movimientos: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw new Exception("Error al obtener movimientos", ex);
            }
        }
        
        private string ObtenerAccionPorTipo(string tipoCodigo)
        {
            // Mapeo de códigos de tipo a acciones (según tu base de datos)
            return tipoCodigo switch
            {
                "001" => "APERTURA",
                "003" => "DEPOSITO",
                "004" => "RETIRO",
                "008" => "TRANSFERENCIA",
                "009" => "TRANSFERENCIA",
                _ => tipoCodigo
            };
        }

        public async Task<OperacionResponse> RegistrarDepositoAsync(string cuenta, double importe)
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "cuenta", cuenta },
                    { "importe", importe.ToString(System.Globalization.CultureInfo.InvariantCulture) }
                };
                
                var response = await CallSoapServiceAsync("RegDeposito", parameters);
                System.Diagnostics.Debug.WriteLine($"RegDeposito Response: {response}");
                
                var xdoc = XDocument.Parse(response);
                
                // Buscar el resultado en la respuesta (campos: Estado y Saldo con mayúsculas)
                var resultElement = xdoc.Descendants()
                    .FirstOrDefault(x => x.Name.LocalName == "RegDepositoResult");
                
                if (resultElement != null)
                {
                    // Parsear la respuesta del servidor: <a:Estado> y <a:Saldo>
                    var estadoElement = resultElement.Elements().FirstOrDefault(x => x.Name.LocalName == "Estado");
                    var saldoElement = resultElement.Elements().FirstOrDefault(x => x.Name.LocalName == "Saldo");
                    
                    var estado = int.TryParse(estadoElement?.Value, out var est) ? est : 0;
                    var saldo = double.TryParse(saldoElement?.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var sal) ? sal : 0;
                    
                    System.Diagnostics.Debug.WriteLine($"RegDeposito - Estado: {estado}, Saldo: {saldo}");
                    
                    return new OperacionResponse
                    {
                        Exito = estado == 1,
                        Mensaje = estado == 1 ? "Depósito exitoso" : "Error en el depósito",
                        SaldoActual = saldo,
                        Cuenta = cuenta
                    };
                }
                
                return new OperacionResponse
                {
                    Exito = false,
                    Mensaje = "Error: No se recibió respuesta del servidor",
                    SaldoActual = 0,
                    Cuenta = cuenta
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al registrar depósito: {ex.Message}");
                return new OperacionResponse
                {
                    Exito = false,
                    Mensaje = $"Error: {ex.Message}",
                    SaldoActual = 0,
                    Cuenta = cuenta
                };
            }
        }

        public async Task<OperacionResponse> RegistrarRetiroAsync(string cuenta, double importe)
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "cuenta", cuenta },
                    { "importe", importe.ToString(System.Globalization.CultureInfo.InvariantCulture) }
                };
                
                var response = await CallSoapServiceAsync("RegRetiro", parameters);
                System.Diagnostics.Debug.WriteLine($"RegRetiro Response: {response}");
                
                var xdoc = XDocument.Parse(response);
                
                // Buscar el resultado en la respuesta (campos: Estado y Saldo con mayúsculas)
                var resultElement = xdoc.Descendants()
                    .FirstOrDefault(x => x.Name.LocalName == "RegRetiroResult");
                
                if (resultElement != null)
                {
                    // Parsear la respuesta del servidor: <a:Estado> y <a:Saldo>
                    var estadoElement = resultElement.Elements().FirstOrDefault(x => x.Name.LocalName == "Estado");
                    var saldoElement = resultElement.Elements().FirstOrDefault(x => x.Name.LocalName == "Saldo");
                    
                    var estado = int.TryParse(estadoElement?.Value, out var est) ? est : 0;
                    var saldo = double.TryParse(saldoElement?.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var sal) ? sal : 0;
                    
                    System.Diagnostics.Debug.WriteLine($"RegRetiro - Estado: {estado}, Saldo: {saldo}");
                    
                    return new OperacionResponse
                    {
                        Exito = estado == 1,
                        Mensaje = estado == 1 ? "Retiro exitoso" : "Error en el retiro",
                        SaldoActual = saldo,
                        Cuenta = cuenta
                    };
                }
                
                return new OperacionResponse
                {
                    Exito = false,
                    Mensaje = "Error: No se recibió respuesta del servidor",
                    SaldoActual = 0,
                    Cuenta = cuenta
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al registrar retiro: {ex.Message}");
                return new OperacionResponse
                {
                    Exito = false,
                    Mensaje = $"Error: {ex.Message}",
                    SaldoActual = 0,
                    Cuenta = cuenta
                };
            }
        }

        public async Task<OperacionResponse> RegistrarTransferenciaAsync(string origen, string destino, double importe)
        {
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    { "cuentaOrigen", origen },
                    { "cuentaDestino", destino },
                    { "importe", importe.ToString(System.Globalization.CultureInfo.InvariantCulture) }
                };
                
                var response = await CallSoapServiceAsync("RegTransferencia", parameters);
                System.Diagnostics.Debug.WriteLine($"RegTransferencia Response: {response}");
                
                var xdoc = XDocument.Parse(response);
                
                // Buscar el resultado en la respuesta (campos: Estado y Saldo con mayúsculas)
                var resultElement = xdoc.Descendants()
                    .FirstOrDefault(x => x.Name.LocalName == "RegTransferenciaResult");
                
                if (resultElement != null)
                {
                    // Parsear la respuesta del servidor: <a:Estado> y <a:Saldo>
                    var estadoElement = resultElement.Elements().FirstOrDefault(x => x.Name.LocalName == "Estado");
                    var saldoElement = resultElement.Elements().FirstOrDefault(x => x.Name.LocalName == "Saldo");
                    
                    var estado = int.TryParse(estadoElement?.Value, out var est) ? est : 0;
                    var saldo = double.TryParse(saldoElement?.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var sal) ? sal : 0;
                    
                    System.Diagnostics.Debug.WriteLine($"RegTransferencia - Estado: {estado}, Saldo: {saldo}");
                    
                    return new OperacionResponse
                    {
                        Exito = estado == 1,
                        Mensaje = estado == 1 ? "Transferencia exitosa" : "Error en la transferencia",
                        SaldoActual = saldo,
                        Cuenta = origen
                    };
                }
                
                return new OperacionResponse
                {
                    Exito = false,
                    Mensaje = "Error: No se recibió respuesta del servidor",
                    SaldoActual = 0,
                    Cuenta = origen
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al registrar transferencia: {ex.Message}");
                return new OperacionResponse
                {
                    Exito = false,
                    Mensaje = $"Error: {ex.Message}",
                    SaldoActual = 0,
                    Cuenta = origen
                };
            }
        }
    }
}
