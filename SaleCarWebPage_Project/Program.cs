using Contracts.Repositories;
using Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SaleCarWebPage_Project.IDContainer;
using SaleCarWebPage_Project.Repo;
using DotNetEnv;
using Microsoft.AspNetCore.Builder;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// --- 1. CONFIGURAÇÃO DE SERVIÇOS (DI Container) ---

builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configuração do DbContext (Assume-se SQL Server, ajusta se for outro)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Chamar as tuas extensões (DI Container)
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddAuthenticationConfig();
builder.Services.AddAppSettingsConfig(builder.Configuration);

// --- 2. BUILD DA APLICAÇÃO ---

var app = builder.Build();

// --- 3. CONFIGURAÇÃO DO PIPELINE DE PEDIDOS (Middleware) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Localização
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

// Ficheiros Estáticos
app.UseDefaultFiles();
var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
provider.Mappings[".avif"] = "image/avif";
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.UseRouting();

// A ordem do CORS, Auth e Authz é fundamental:
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

// Mapeamento de Endpoints
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("Index.html");

app.Run();