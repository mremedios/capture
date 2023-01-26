using System.IO;
using Capture.Service;
using Capture.Service.Database;
using Capture.Service.Database.JsonHeaders;
using Capture.Service.Listener;
using Capture.Service.NameLater;
using Capture.Service.Parser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) =>
    {
        config
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<ICapture, UdpCapture>();
        // services.AddSingleton<ICapture, TcpCapture>();
        services.AddHostedService<HostedService>();
        services.AddSingleton<IHeaderRepository, JsonRepository>();
        services.AddSingleton<IHandler, Handler>();
    })
    .ConfigureLogging((_, configLogging) =>
    {
        configLogging.ClearProviders();
        configLogging.AddConsole();
    });


var host = builder.Build();

await host.RunAsync().ConfigureAwait(false);