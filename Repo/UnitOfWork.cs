using Contracts.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace SaleCarWebPage_Project.Repo
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private TransactionScope? _scope = null!;

        private bool _disposed = false;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context; // ADICIONADO: Necessário para salvar no DB

        public IAddressRepository Address { get; }
        public IBrandRepository Brands { get; }
        public ICarRepository Cars { get; }
        public ICarModelRepository CarModels { get; }
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
            CarModels = carModelRepository;
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
            // Vou comentar para que o UnitOfWork não bloquear o ADO.NET
            /*if (_scope != null)
            {
                throw new InvalidOperationException("Já existe uma transação ativa.");
            }

            var options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted, // Padrão bom para leitura/escrita consistente
                Timeout = TimeSpan.FromSeconds(30) // Ajustar se necessitar de mais tempo
            };

            _scope = new TransactionScope(
                TransactionScopeOption.Required,
                options,
                TransactionScopeAsyncFlowOption.Enabled //Para Async
            );*/

            await Task.CompletedTask;
        }

        public async Task<int> CommitAsync()
        {
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
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}