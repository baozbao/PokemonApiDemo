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

// 自动让 Docker 在启动时创建数据库 (Apply Migrations)
// 不然每次还得手动创建库
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PokemonDbContext>();
        context.Database.Migrate(); // 没有库就创建，有库就更新
        Console.WriteLine("Database migrated successfully. 🟢");

        // 自动生成测试数据 (Seeding)
        // 代码挪到了 DemoApi.Infrastructure/Data/PokemonSeeder.cs 里面，为了让 Program.cs 干净点
        PokemonSeeder.Seed(context);
        // -----------------------------------------------------
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration failed: {ex.Message} ");
    }
}

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

// app.UseHttpsRedirection(); // Docker 里没有配证书，得把这个关了，不然一直报黄字警告
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { } // Intergration test 用
