using System;
using System.Collections.Generic;
using EB_DOTNET_SOAP_CliCon.ec.edu.monster.modelo;

namespace EB_DOTNET_SOAP_CliCon.ec.edu.monster.vista
{
    public static class MovimientoView
    {
        public static void MostrarMovimientos(List<CliCon_Movimiento> movimientos, string cuenta)
        {
            Console.Clear();
            ConsoleHelper.DibujarHeader(Environment.UserName); // o pasa el usuario desde fuera
            ConsoleHelper.EscribirSubtitulo($"  Movimientos de cuenta: {cuenta}");
            Console.WriteLine();

            if (movimientos == null || movimientos.Count == 0)
            {
                ConsoleHelper.EscribirInfo("  No hay movimientos registrados.");
                return;
            }

            // Encabezado de tabla
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  ┌{new string('─', 6)}┬{new string('─', 20)}┬{new string('─', 24)}┬{new string('─', 12)}┬{new string('─', 16)}┐");
            Console.WriteLine($"  │ {"Nro",-4} │ {"Fecha",-18} │ {"Tipo",-22} │ {"Acción",-10} │ {"Importe (USD)",-14} │");
            Console.WriteLine($"  ├{new string('─', 6)}┼{new string('─', 20)}┼{new string('─', 24)}┼{new string('─', 12)}┼{new string('─', 16)}┤");
            Console.ResetColor();

            foreach (var m in movimientos)
            {
                // Cambiar color según acción
                if (m.Accion == "INGRESO")
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (m.Accion == "SALIDA")
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ForegroundColor = ConsoleColor.Gray;

                Console.WriteLine($"  │ {m.NroMov,-4} │ {m.Fecha:yyyy-MM-dd HH:mm,-18} │ {m.Tipo,-22} │ {m.Accion,-10} │ {m.Importe,14:N2} │");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  └{new string('─', 6)}┴{new string('─', 20)}┴{new string('─', 24)}┴{new string('─', 12)}┴{new string('─', 16)}┘");
            Console.ResetColor();
        }
    }
}