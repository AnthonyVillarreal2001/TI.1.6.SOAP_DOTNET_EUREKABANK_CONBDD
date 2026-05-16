using CLIESC_EurekaBank_SOAP_GR04;
using ec.edu.monster.controllers;
using ec.edu.monster.services;
using ec.edu.monster.views;
using System.Windows;

namespace EurekaBank.WpfClient
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Instancia del API SOAP (.NET CoreWCF en 01.SERVIDOR)
            var api = new SoapEurekaApi("https://localhost:7299/CoreBancario.svc");

            var login = new LoginWindow();
            var ctrl = new LoginController(login, api);
            MainWindow = login;
            login.Show();
        }
    }
}
