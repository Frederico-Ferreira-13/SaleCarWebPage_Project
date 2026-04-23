using Contracts.Repositories;
using Contracts.Services;
using Core.Common;
using Core.Model.ValueObjects;
using Microsoft.AspNetCore.Authentication.Cookies;
using SaleCarWebPage_Project.Repo;
using Services;

namespace SaleCarWebPage_Project.IDContainer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Criar e registar a instância de JwtSettings
            var jwtSettings = new JwtSettings();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);
            services.AddSingleton(jwtSettings);

            services.AddHttpContextAccessor();
            // Registar os serviços de aplicação
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<ICarService, CarService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICarModelService, CarModelService>();
            services.AddScoped<IProviderService, ProviderService>();
            services.AddScoped<ISaleService, SaleService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IFavoritesService, FavoritesService>();
            services.AddScoped<IMessageBoxService, MessageBoxService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICloudService, CloudService>();
            services.AddScoped<IAuthenticationService, Services.AuthenticationService>();

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<ICarRepository, CarRepository>();
            services.AddScoped<ICarModelRepository, CarModelRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IFavoritesRepository, FavoritesRepository>();
            services.AddScoped<IMessageBoxRepository, MessageBoxRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();
            services.AddScoped<IUsersRepository, UserRepository>();
            services.AddScoped<IUserSettingRepository, UserSettingRepository>();
            services.AddScoped<IUsersRoleRepository, UsersRoleRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddAuthenticationConfig(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/AccessDenied";
                    options.Cookie.Name = "LuxCarAuth";
                    options.ExpireTimeSpan = TimeSpan.FromHours(2);
                });

            return services;
        }

        public static IServiceCollection AddAppSettingsConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            return services;
        }
    }
}
