using System.IO;
using Capture.Service;
using Capture.Service.Database;
using Capture.Service.Database.Calls;
using Capture.Service.Handler;
using Capture.Service.Listener;
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
            .AddSingleton<IHeaderRepository, CallsRepository>()
            .AddSingleton<IHandler, Handler>()
            .AddSingleton<IContextFactory, PostgreSqlContextFactory>()
            .AddSingleton<IHeadersProvider, HeadersProvider>();
    })
    .ConfigureLogging((_, configLogging) =>
    {
        configLogging.ClearProviders();
        configLogging.AddConsole();
    });


var host = builder.Build();

await host.RunAsync().ConfigureAwait(false);