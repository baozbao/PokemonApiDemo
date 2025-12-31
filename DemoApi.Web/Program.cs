using DemoApi.Infrastructure.Data;
using DemoApi.Infrastructure.Repositories;
using DemoApi.Service.Services;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Interfaces;

var builder = WebApplication.CreateBuilder(args);



//DB connection
var connStr = builder.Configuration.GetConnectionString("PokeMonCenterDB");

builder.Services.AddDbContext<PokemonDbContext>(options =>
    options.UseSqlServer(connStr)
);

//DI

builder.Services.AddScoped<IPokemonPCRepository, PokemonPCRepository>();
builder.Services.AddScoped<IPokemonPCService, PokemonPCService>();
// зЂВс Controllers
builder.Services.AddControllers();

// зЂВс Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(DemoApi.Service.Mapping.MappingProfiles).Assembly);

var app = builder.Build();

// Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
