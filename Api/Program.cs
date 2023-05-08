using Database.Database;
using Database.Database.Calls;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*builder.Services.AddCors(options =>
{
   // options.
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("*")
            .WithMethods("GET,PUT,POST,DELETE,OPTIONS")
            .WithHeaders("*")
            .WithExposedHeaders("*");
    });
});*/


builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false);

builder.Services
    .AddSingleton<IAvailableHeaderRepository, AvailableHeaderRepository>()
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