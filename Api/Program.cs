using Capture.Service.Database;
using Capture.Service.Database.Calls;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false);

builder.Services
    .AddSingleton<IAvailableHeaderRepository, AvailableHeaderRepository>()
    .AddSingleton<IHeaderRepository, CallsRepository>()
    .AddSingleton<IContextFactory, PostgreSqlContextFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();