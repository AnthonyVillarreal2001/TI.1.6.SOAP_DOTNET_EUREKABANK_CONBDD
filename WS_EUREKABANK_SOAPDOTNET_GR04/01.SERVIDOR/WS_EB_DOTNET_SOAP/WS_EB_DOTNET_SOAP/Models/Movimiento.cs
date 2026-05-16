using System.Runtime.Serialization;

namespace WS_EB_DOTNET_SOAP.Models
{
    [DataContract]
    public class Movimiento
    {
        [DataMember(Order = 1)] public string CuentaCodigo { get; set; } = string.Empty; // chr_cuencodigo
        [DataMember(Order = 2)] public int Numero { get; set; }                           // int_movinumero
        [DataMember(Order = 3)] public DateTime Fecha { get; set; }                       // dtt_movifecha
        [DataMember(Order = 4)] public string EmpleadoCodigo { get; set; } = string.Empty;// chr_emplcodigo
        [DataMember(Order = 5)] public string TipoCodigo { get; set; } = string.Empty;    // chr_tipocodigo
        [DataMember(Order = 6)] public decimal Importe { get; set; }                      // dec_moviimporte
        [DataMember(Order = 7)] public string? ReferenciaCuenta { get; set; }             // chr_cuenreferencia
    }
}
