using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ServicioEB2; // <-- namespace del Connected Service

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// Inyecta el cliente generado por Connected Service
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var url = cfg["Soap:EndpointHttps"] ?? "https://localhost:7299/CoreBancario.svc";

    return new CoreBancarioSoapClient(
        CoreBancarioSoapClient.EndpointConfiguration.BasicHttpBinding_CoreBancarioSoap,
        url);
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
