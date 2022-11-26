using System.IO;
using Capture.Service;
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
        services.AddHostedService<HostedService>();
    })
    .ConfigureLogging((_, configLogging) =>
    {
        configLogging.ClearProviders();
        configLogging.AddConsole();
    });


var host = builder.Build();

await host.RunAsync().ConfigureAwait(false);