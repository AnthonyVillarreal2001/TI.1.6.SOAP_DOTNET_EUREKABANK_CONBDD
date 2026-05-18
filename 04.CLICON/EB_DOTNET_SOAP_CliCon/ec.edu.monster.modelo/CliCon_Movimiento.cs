using System;

namespace EB_DOTNET_SOAP_CliCon.ec.edu.monster.modelo
{
    public class CliCon_Movimiento
    {
        public string Cuenta { get; set; }
        public int NroMov { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
        public string Accion { get; set; }
        public decimal Importe { get; set; }
    }
}
