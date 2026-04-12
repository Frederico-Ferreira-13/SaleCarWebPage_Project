using Microsoft.EntityFrameworkCore;
using SaleCarWebPage_Project.Repo;
using SaleCarWebPage_Project.Contracts;
using Core.Model;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar o Razor Pages
builder.Services.AddRazorPages();

// 2. Configurar a Base de Dados (Entity Framework)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Injeção de Dependência dos Repositórios
// IMPORTANTE: Registamos a Interface ligada à Implementação
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();

// Aqui podes já deixar preparado para o futuro:
// builder.Services.AddScoped<IUserRepository, UserRepository>();
// builder.Services.AddScoped<ISaleRepository, SaleRepository>();

var app = builder.Build();

// 4. Configurar o Pipeline de pedidos HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Adiciona o suporte para ficheiros estáticos (CSS, JS, Imagens dos carros)
app.UseStaticFiles();

app.UseRouting();

// Necessário para o sistema de Login que vamos criar
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();