using Contracts.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace SaleCarWebPage_Project.Repo
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private TransactionScope? _scope;
        private bool _disposed = false;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context; // ADICIONADO: Necessário para salvar no DB

        public IAddressRepository Address { get; }
        public IBrandRepository Brands { get; }
        public ICarRepository Cars { get; }
        public ICarModelRepository Models { get; }
        public IClientRepository Clients { get; }
        public IContactRepository Contacts { get; }
        public IFavoritesRepository Favorites { get; }
        public IMessageBoxRepository MessageBox { get; }
        public IProviderRepository Providers { get; }
        public ISaleRepository Sales { get; }
        public IUsersRepository Users { get; }
        public IUserSettingRepository UserSettings { get; }
        public IUsersRoleRepository UsersRole { get; }

        public UnitOfWork(
            ApplicationDbContext context, // Injetar o contexto aqui
            IAddressRepository addressRepository,
            IBrandRepository brandRepository,
            ICarRepository carRepository,
            ICarModelRepository carModelRepository,
            IClientRepository clientRepository,
            IContactRepository contactRepository,
            IFavoritesRepository favoritesRepository,
            IMessageBoxRepository messageBoxRepository,
            IProviderRepository providerRepository,
            ISaleRepository saleRepository,
            IUsersRepository usersRepository,
            IUserSettingRepository userSettingRepository,
            IUsersRoleRepository userRoleRepository,
            IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            Address = addressRepository;
            Brands = brandRepository;
            Cars = carRepository;
            Models = carModelRepository;
            Clients = clientRepository;
            Contacts = contactRepository;
            Favorites = favoritesRepository;
            MessageBox = messageBoxRepository;
            Providers = providerRepository;
            Sales = saleRepository;
            Users = usersRepository;
            UserSettings = userSettingRepository;
            UsersRole = userRoleRepository;
        }

        public async Task BeginTransactionAsync()
        {
            // Mantido comentário do colega, mas deixo a estrutura preparada
            await Task.CompletedTask;
        }

        public async Task<int> CommitAsync()
        {
            // CORREÇÃO: Chama o SaveChangesAsync do contexto para gravar os dados
            return await _context.SaveChangesAsync();
        }

        public void Rollback()
        {
            // Implementação futura de rollback se necessário
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _scope?.Dispose();
                _context.Dispose(); // Garante que o contexto é libertado
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}