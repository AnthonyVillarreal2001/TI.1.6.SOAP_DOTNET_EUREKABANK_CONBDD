namespace EurekaWeb.Models
{
    public class ResultadoVm
    {
        public string Mensaje { get; set; } = "";
        public bool EsExitoso { get; set; }
        public decimal? Saldo { get; set; }   // null si no aplica
        public string? Cuenta { get; set; }
    }
}
