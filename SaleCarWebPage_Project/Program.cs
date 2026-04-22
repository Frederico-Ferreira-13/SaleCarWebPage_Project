using Contracts.Repositories;
using Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SaleCarWebPage_Project.IDContainer;
using SaleCarWebPage_Project.Repo;
using DotNetEnv;
using Microsoft.AspNetCore.Builder;

// 1. Carregar variáveis do ficheiro .env logo no início
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// --- 1. CONFIGURAÇÃO DE SERVIÇOS (DI Container) ---

// Força o builder a reconhecer as variáveis injetadas pelo DotNetEnv
builder.Configuration.AddEnvironmentVariables();

// Lógica de captura da Connection String:
// Primeiro tentamos ler a variável de ambiente direta do .env
// Se não existir, ele recua para o método padrão (appsettings.json)
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada no .env ou appsettings.json.");
}

// Configuração do DbContext
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

// Injeção de Dependências das tuas Extensões
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

// Configuração de Localização
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

// Configuração de Ficheiros Estáticos
app.UseDefaultFiles();
var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
provider.Mappings[".avif"] = "image/avif";
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.UseRouting();

// Ordem fundamental de Middleware: CORS -> Auth -> Authz
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

// Mapeamento de Endpoints
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("Index.html");

app.Run();