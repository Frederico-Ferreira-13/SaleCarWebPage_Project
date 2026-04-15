using Microsoft.EntityFrameworkCore;
using SaleCarWebPage_Project.Repo;
using Core.Model;
using Contracts.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar o Razor Pages
builder.Services.AddRazorPages();

// 2. Configurar a Base de Dados (Entity Framework)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Injeção de Dependência dos Repositórios
// Interface ligada à Implementação para as 13 tabelas
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<ICarModelRepository, CarModelRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IFavoritesRepository, FavoritesRepository>();
builder.Services.AddScoped<IMessageBoxRepository, MessageBoxRepository>();
builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IUsersRepository, UserRepository>();
builder.Services.AddScoped<IUserSettingRepository, UserSettingRepository>();
builder.Services.AddScoped<IUsersRoleRepository, UsersRoleRepository>();

// Registro da Unit of Work (Centraliza todos os repositórios acima)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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