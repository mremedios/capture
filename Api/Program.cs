using Database.Database;
using Database.Database.Calls;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Configuration
	.SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
	.AddJsonFile("appsettings.json", optional: false);

builder.Services
	.AddSingleton<IAvailableHeaderRepository, AvailableHeaderRepository>()
	.AddSingleton<IMethodsRepository, MethodsRepository>()
	.AddSingleton<IReadonlyRepository, ReadonlyRepository>()
	.AddSingleton<IContextFactory, PostgreSqlContextFactory>()
	.AddSingleton<IPartmanRepository, PartmanRepository>();

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

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.Run();