using System.Runtime.Serialization;

namespace WS_EB_DOTNET_SOAP.Models
{
    [DataContract]
    public class OperacionCuentaResponse
    {
        [DataMember(Order = 1)] public int Estado { get; set; }     // 1 = OK, -1 = Error
        [DataMember(Order = 2)] public decimal Saldo { get; set; }  // saldo actualizado
    }
}
