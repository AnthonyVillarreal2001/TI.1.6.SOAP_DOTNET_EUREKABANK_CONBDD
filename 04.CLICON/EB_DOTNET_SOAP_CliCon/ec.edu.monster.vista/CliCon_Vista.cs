using System;
using System.Collections.Generic;
using EB_DOTNET_SOAP_CliCon.ec.edu.monster.controlador;
using EB_DOTNET_SOAP_CliCon.ec.edu.monster.modelo;

namespace EB_DOTNET_SOAP_CliCon.ec.edu.monster.vista
{
    public static class CliCon_Vista
    {
        private static CliCon_Controlador controlador;
        private static string usuarioActual;

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "EurekaBank - Cliente Consola";
            controlador = new CliCon_Controlador();

            // Pantalla de login (estilo)
            MostrarLogin();

            // Menú principal con acordeón
            bool salir = false;
            while (!salir)
            {
                MostrarMenuPrincipal();
                string opcion = Console.ReadLine();
                switch (opcion)
                {
                    case "1": EjecutarConsultaMovimientos(); break;
                    case "2": EjecutarDeposito(); break;
                    case "3": EjecutarRetiro(); break;
                    case "4": EjecutarTransferencia(); break;
                    case "5": salir = true; break;
                    default: ConsoleHelper.EscribirError("Opción no válida."); break;
                }
            }

            ConsoleHelper.EscribirExito("\nGracias por usar EurekaBank. ¡Hasta pronto!");
            Console.ResetColor();
        }

        private static void MostrarLogin()
        {
            Console.Clear();
            // Simular fondo con color sólido (como overlay oscuro)
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            // Título principal
            ConsoleHelper.EscribirTitulo(@"
    ███████╗██╗   ██╗██████╗ ███████╗██╗  ██╗ █████╗ ██████╗  █████╗ ███╗   ██╗██╗  ██╗
    ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██║ ██╔╝██╔══██╗██╔══██╗██╔══██╗████╗  ██║██║ ██╔╝
    █████╗   ╚████╔╝ ██████╔╝█████╗  █████╔╝ ███████║██████╔╝███████║██╔██╗ ██║█████╔╝ 
    ██╔══╝    ╚██╔╝  ██╔══██╗██╔══╝  ██╔═██╗ ██╔══██║██╔══██╗██╔══██║██║╚██╗██║██╔═██╗ 
    ███████╗   ██║   ██║  ██║███████╗██║  ██╗██║  ██║██████╔╝██║  ██║██║ ╚████║██║  ██╗
    ╚══════╝   ╚═╝   ╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝
            ");
            ConsoleHelper.EscribirSubtitulo("=== Sistema Bancario Seguro ===\n");

            Console.Write("  Usuario: ");
            string usuario = Console.ReadLine();
            Console.Write("  Contraseña: ");
            string password = LeerPassword();

            string resultado = controlador.ValidarIngreso(usuario, password);
            if (resultado == "Exitoso")
            {
                usuarioActual = usuario;
                ConsoleHelper.EscribirExito("\n  ¡Acceso concedido! Redirigiendo...");
                System.Threading.Thread.Sleep(1000);
            }
            else
            {
                ConsoleHelper.EscribirError("\n  Acceso denegado. Presione cualquier tecla para salir.");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private static string LeerPassword()
        {
            string pass = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass.Substring(0, pass.Length - 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return pass;
        }

        private static void MostrarMenuPrincipal()
        {
            Console.Clear();
            ConsoleHelper.DibujarHeader(usuarioActual);
            ConsoleHelper.EscribirTituloMenú();

            ConsoleHelper.EscribirOpcion("1", "Consulta de Movimientos");
            ConsoleHelper.EscribirOpcion("2", "Depósito");
            ConsoleHelper.EscribirOpcion("3", "Retiro");
            ConsoleHelper.EscribirOpcion("4", "Transferencia");
            ConsoleHelper.EscribirOpcion("5", "Salir");
            ConsoleHelper.DibujarFooter();

            Console.Write("\n  Seleccione una opción: ");
        }

        private static void EjecutarConsultaMovimientos()
        {
            Console.Clear();
            ConsoleHelper.DibujarHeader(usuarioActual);
            ConsoleHelper.EscribirSubtitulo("  Consulta de Movimientos");
            Console.Write("\n  Número de cuenta: ");
            string cuenta = Console.ReadLine();

            var movimientos = controlador.ObtenerMovimientos(cuenta);
            MovimientoView.MostrarMovimientos(movimientos, cuenta);

            ConsoleHelper.EscribirInfo("\n  Presione cualquier tecla para volver...");
            Console.ReadKey();
        }

        private static void EjecutarDeposito()
        {
            Console.Clear();
            ConsoleHelper.DibujarHeader(usuarioActual);
            ConsoleHelper.EscribirSubtitulo("  Depósito");
            Console.Write("\n  Cuenta destino: ");
            string cuenta = Console.ReadLine();
            Console.Write("  Importe (USD): ");
            decimal importe = LeerDecimal();

            var resultado = controlador.RegistrarDeposito(cuenta, importe);
            if (resultado.estado == 1)
                ConsoleHelper.EscribirExito($"  Depósito exitoso. Nuevo saldo: {resultado.saldo:C2}");
            else
                ConsoleHelper.EscribirError("  Error al realizar el depósito.");

            ConsoleHelper.EscribirInfo("\n  Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private static void EjecutarRetiro()
        {
            Console.Clear();
            ConsoleHelper.DibujarHeader(usuarioActual);
            ConsoleHelper.EscribirSubtitulo("  Retiro");
            Console.Write("\n  Cuenta origen: ");
            string cuenta = Console.ReadLine();
            Console.Write("  Importe (USD): ");
            decimal importe = LeerDecimal();

            var resultado = controlador.RegistrarRetiro(cuenta, importe);
            if (resultado.estado == 1)
                ConsoleHelper.EscribirExito($"  Retiro exitoso. Nuevo saldo: {resultado.saldo:C2}");
            else
                ConsoleHelper.EscribirError("  Error: saldo insuficiente o cuenta inválida.");

            ConsoleHelper.EscribirInfo("\n  Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private static void EjecutarTransferencia()
        {
            Console.Clear();
            ConsoleHelper.DibujarHeader(usuarioActual);
            ConsoleHelper.EscribirSubtitulo("  Transferencia");
            Console.Write("\n  Cuenta origen: ");
            string origen = Console.ReadLine();
            Console.Write("  Cuenta destino: ");
            string destino = Console.ReadLine();
            Console.Write("  Importe (USD): ");
            decimal importe = LeerDecimal();

            var resultado = controlador.RegistrarTransferencia(origen, destino, importe);
            if (resultado.estado == 1)
                ConsoleHelper.EscribirExito($"  Transferencia exitosa. Saldo origen: {resultado.saldo:C2}");
            else
                ConsoleHelper.EscribirError("  Error en la transferencia. Verifique saldo o cuentas.");

            ConsoleHelper.EscribirInfo("\n  Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private static decimal LeerDecimal()
        {
            while (true)
            {
                if (decimal.TryParse(Console.ReadLine(), out decimal valor))
                    return valor;
                ConsoleHelper.EscribirError("  Valor inválido. Ingrese un número: ");
            }
        }
    }

    // Clase auxiliar para colores, bordes y efectos
    public static class ConsoleHelper
    {
        public static void EscribirTitulo(string texto)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(texto);
            Console.ResetColor();
        }

        public static void EscribirSubtitulo(string texto)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(texto);
            Console.ResetColor();
        }

        public static void EscribirExito(string texto)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(texto);
            Console.ResetColor();
        }

        public static void EscribirError(string texto)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(texto);
            Console.ResetColor();
        }

        public static void EscribirInfo(string texto)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(texto);
            Console.ResetColor();
        }

        public static void EscribirOpcion(string numero, string descripcion)
        {
            Console.Write("  ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{numero}]");
            Console.ResetColor();
            Console.WriteLine($" {descripcion}");
        }

        public static void DibujarHeader(string usuario)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string(' ', 80));
            Console.WriteLine("  EUREKABANK                         Usuario: " + usuario.PadRight(20));
            Console.WriteLine(new string(' ', 80));
            Console.ResetColor();
        }

        public static void DibujarFooter()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n  " + new string('─', 76));
            Console.WriteLine("  Desarrollado por Ariel R. y Anthony V. | EurekaBank © 2025");
            Console.ResetColor();
        }

        public static void EscribirTituloMenú()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n  === MENÚ PRINCIPAL ===\n");
            Console.ResetColor();
        }
    }
}