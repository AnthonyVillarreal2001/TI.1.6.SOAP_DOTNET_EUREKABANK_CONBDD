using CoreWCF;
using WS_EB_DOTNET_SOAP.Models;

namespace WS_EB_DOTNET_SOAP.Contracts
{
    [ServiceContract(Name = "CoreBancarioSoap", Namespace = "urn:eurekabank")]
    public interface ICoreBancarioSoap
    {
        [OperationContract]
        string ValidarIngreso(string usuario, string password);

        [OperationContract]
        Movimiento[] TraerMovimientos(string cuenta);

        [OperationContract]
        OperacionCuentaResponse RegDeposito(string cuenta, decimal importe, string? empleado);

        [OperationContract]
        OperacionCuentaResponse RegRetiro(string cuenta, decimal importe, string? empleado);

        // Retorna saldo de ORIGEN
        [OperationContract]
        OperacionCuentaResponse RegTransferencia(string cuentaOrigen, string cuentaDestino, decimal importe, string? empleado);
    }
}
