using EB_DOTNET_SOAP_CliCon.ec.edu.monster.controlador;
using EB_DOTNET_SOAP_CliCon.ec.edu.monster.vista;
using System;

namespace EB_DOTNET_SOAP_CliCon
{
    public class CliCon_Vista
    {
        static void Main(string[] args)
        {
            var scanner = new ConsoleInputHelper();
            var controlador = new CliCon_Controlador();

            Console.WriteLine("\n=== E U R E K A B A N K ===\n");

            Console.Write("Usuario: ");
            string usuario = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            var login = controlador.ValidarIngreso(usuario, password);
            if (!string.Equals(login, "Exitoso", StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nAcceso denegado");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nAcceso exitoso");
            Console.ResetColor();

            int opcion;
            do
            {
                Console.WriteLine("\n1. Consultar movimientos");
                Console.WriteLine("2. Realizar depósito");
                Console.WriteLine("3. Realizar retiro");
                Console.WriteLine("4. Realizar transferencia");
                Console.WriteLine("5. Salir");
                Console.Write("\nSeleccione: ");
                opcion = scanner.ReadInt();

                switch (opcion)
                {
                    case 1:
                        Console.Write("\nCuenta: ");
                        var cuenta = Console.ReadLine();
                        var movimientos = controlador.ObtenerMovimientos(cuenta);
                        if (movimientos.Count == 0) Console.WriteLine("Sin movimientos.");
                        else MovimientoView.MostrarMovimientos(movimientos);
                        break;

                    case 2:
                        Console.Write("\nCuenta depósito: ");
                        var cDep = Console.ReadLine();
                        Console.Write("Importe: ");
                        var impDep = scanner.ReadDecimal();
                        var dep = controlador.RegistrarDeposito(cDep, impDep);
                        Console.WriteLine(dep.estado == 1
                            ? $"Depósito OK. Saldo actual: {dep.saldo:N2}"
                            : "Depósito falló.");
                        break;

                    case 3:
                        Console.Write("\nCuenta retiro: ");
                        var cRet = Console.ReadLine();
                        Console.Write("Importe: ");
                        var impRet = scanner.ReadDecimal();
                        var ret = controlador.RegistrarRetiro(cRet, impRet);
                        Console.WriteLine(ret.estado == 1
                            ? $"Retiro OK. Saldo actual: {ret.saldo:N2}"
                            : "Retiro falló.");
                        break;

                    case 4:
                        Console.Write("\nCuenta origen: ");
                        var cOri = Console.ReadLine();
                        Console.Write("Cuenta destino: ");
                        var cDes = Console.ReadLine();
                        Console.Write("Importe: ");
                        var impTr = scanner.ReadDecimal();
                        var tr = controlador.RegistrarTransferencia(cOri, cDes, impTr);
                        Console.WriteLine(tr.estado == 1
                            ? $"Transferencia OK. Saldo origen: {tr.saldo:N2}"
                            : "Transferencia falló.");
                        break;

                    case 5:
                        Console.WriteLine("Saliendo...");
                        break;

                    default:
                        Console.WriteLine("Opción inválida.");
                        break;
                }
            }
            while (opcion != 5);
        }
    }


    public class ConsoleInputHelper
    {
        public int ReadInt()
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out var v)) return v;
                Console.Write("Número inválido. Intente: ");
            }
        }

        public decimal ReadDecimal()
        {
            while (true)
            {
                if (decimal.TryParse(Console.ReadLine(),
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var v)) return v;
                Console.Write("Número inválido. Intente: ");
            }
        }
    }

}
