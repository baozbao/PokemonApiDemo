using DemoApi.Infrastructure.Data;
using DemoApi.Infrastructure.Repositories;
using DemoApi.Service.Services;
using Microsoft.EntityFrameworkCore;
using DemoApi.Domain.Interfaces;
using FluentValidation.AspNetCore;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

//DB connection
var connStr = builder.Configuration.GetConnectionString("PokeMonCenterDB");

builder.Services.AddDbContext<PokemonDbContext>(options =>
    options.UseSqlServer(connStr)
);

//DI
builder.Services.AddScoped<IPokemonPCRepository, PokemonPCRepository>();
builder.Services.AddScoped<IPokemonPCService, PokemonPCService>();

// Controllers
// Controllers
builder.Services.AddControllers();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<DemoApi.Service.Validators.SearchPokemonRequestValidator>();

// Swagger
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
