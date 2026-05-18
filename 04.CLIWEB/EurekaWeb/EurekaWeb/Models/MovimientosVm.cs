using System;
using System.Collections.Generic;
using ServicioEB2;

namespace EurekaWeb.Models
{
    public class MovimientosVm
    {
        public string Cuenta { get; set; } = "";
        public List<Item> Movs { get; set; } = new();

        public class Item
        {
            public int Numero { get; set; }
            public string Fecha { get; set; } = "";
            public string Tipo { get; set; } = "";
            public string Accion { get; set; } = "";
            public string Importe { get; set; } = "";
            public string RowClass { get; set; } = "";
            public string BadgeClass { get; set; } = "";
            public string ImporteClass { get; set; } = "";

            public static Item FromDto(Movimiento m)
            {
                var tipo = TipoDescripcion(m.TipoCodigo);
                var accion = TipoAccion(m.TipoCodigo);

                var row = accion == "INGRESO" ? "tr-ingreso" :
                          accion == "SALIDA" ? "tr-salida" : "";

                var badge = "badge";
                var tl = tipo.ToLowerInvariant();
                if (tl.Contains("depós") || tl.Contains("deposit")) badge += " badge-deposito";
                else if (tl.Contains("retiro")) badge += " badge-retiro";
                else if (tl.Contains("transfer")) badge += (accion == "INGRESO" ? " badge-deposito" : " badge-retiro");
                else badge += " badge-transferencia";

                var impClass = accion == "INGRESO" ? "importe-ingreso" : "importe-salida";

                return new Item
                {
                    Numero = m.Numero,
                    Fecha = m.Fecha.ToString("yyyy-MM-dd HH:mm"),
                    Tipo = tipo,
                    Accion = accion,
                    Importe = m.Importe.ToString("N2"),
                    RowClass = row,
                    BadgeClass = badge,
                    ImporteClass = impClass
                };
            }

            static string TipoDescripcion(string? codigo) => codigo switch
            {
                "001" => "Apertura de Cuenta",
                "002" => "Cancelar Cuenta",
                "003" => "Depósito",
                "004" => "Retiro",
                "005" => "Interés",
                "006" => "Mantenimiento",
                "007" => "ITF",
                "008" => "Transferencia",
                "009" => "Transferencia",
                "010" => "Cargo por Movimiento",
                _ => string.IsNullOrEmpty(codigo) ? "" : codigo
            };

            static string TipoAccion(string? codigo) => codigo switch
            {
                "001" => "INGRESO",
                "002" => "SALIDA",
                "003" => "INGRESO",
                "004" => "SALIDA",
                "005" => "INGRESO",
                "006" => "SALIDA",
                "007" => "SALIDA",
                "008" => "INGRESO",
                "009" => "SALIDA",
                "010" => "SALIDA",
                _ => ""
            };
        }
    }
}
