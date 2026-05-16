// MovimientoView.cs
using System;
using System.Collections.Generic;
using EB_DOTNET_SOAP_CliCon.ec.edu.monster.modelo;

namespace EB_DOTNET_SOAP_CliCon.ec.edu.monster.vista
{
    public class MovimientoView
    {
        public static void MostrarMovimientos(List<CliCon_Movimiento> movimientos)
        {
            if (movimientos == null || movimientos.Count == 0)
            {
                Console.WriteLine("Sin movimientos.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine(new string('═', 95));
            Console.WriteLine("{0,-6} {1,-16} {2,-22} {3,-10} {4,16}",
                "NRO", "FECHA", "TIPO", "ACCIÓN", "IMPORTE");
            Console.WriteLine(new string('═', 95));

            foreach (var m in movimientos)
            {
                Console.WriteLine("{0,-6} {1,-16:yyyy-MM-dd HH:mm} {2,-22} {3,-10} {4,16:N2}",
                    m.NroMov, m.Fecha, m.Tipo, m.Accion, m.Importe);
            }

            Console.WriteLine(new string('─', 95));
        }
    }
}
