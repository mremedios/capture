using System.IO;
using Capture.Service;
using Capture.Service.Handler;
using Capture.Service.Handler.provider;
using Capture.Service.Listener;
using Database.Database;
using Database.Database.Calls;
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
        services.AddSingleton<ICapture, UdpCapture>()
            .AddHostedService<HostedService>()
            .AddSingleton<ICallsRepository, CallsRepository>()
            .AddSingleton<IAvailableHeaderRepository, AvailableHeaderRepository>()
            .AddSingleton<IMethodsRepository, MethodsRepository>()
            .AddSingleton<IHandler, Handler>()
            .AddSingleton<IContextFactory, PostgreSqlContextFactory>()
            .AddSingleton<IOptionsProvider, OptionsProvider>();

    })
    .ConfigureLogging((_, configLogging) =>
    {
        configLogging.ClearProviders();
        configLogging.AddConsole();
    });


var host = builder.Build();

await host.RunAsync().ConfigureAwait(false);