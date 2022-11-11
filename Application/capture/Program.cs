using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args)
	.ConfigureAppConfiguration((hostingContext, config) =>
	{
		config
		.SetBasePath(Directory.GetCurrentDirectory());

		//Setup configuration

	})
	.ConfigureServices(services =>
	{
		//Setup DI
	});


var host = builder.Build();

await host.RunAsync().ConfigureAwait(false);