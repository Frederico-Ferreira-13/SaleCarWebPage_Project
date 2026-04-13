using Contracts.Repositories;
using Microsoft.Extensions.Configuration;
using SaleCarWebPage_Project.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Repo
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private TransactionScope? _scope;
        private bool _disposed = false;
        private readonly IConfiguration _configuration;

        public IAddressRepository Address { get; }
        public IBrandRepository Brands { get; }
        public ICarRepository Cars { get; }
        public ICarModelRepository Models { get; }
        public IClientRepository Clients { get; }
        public IContactRepository Contacts { get; }
        public IFavoritsRepository Favorits { get; }
        public IMassageBoxRepository MessageBoox { get; }
        public IProviderRepository Providers { get; }
        public ISaleRepository Sales { get; }
        public IUsersRepository Users { get; }
        public IUserSettingRepository UserSettings { get; }
        public IUserRoleRepository UserRoles { get; }

        public UnitOfWork(IAddressRepository addressRepository, IBrandRepository brandRepository, ICarRepository carRepository, 
            ICarModelRepository carModelRepository, IClientRepository clientRepository, 
            IContactRepository contactRepository, IFavoritsRepository favoritsRepository, 
            IMassageBoxRepository massageBoxRepository, IProviderRepository providerRepository, 
            ISaleRepository saleRepository, IUsersRepository usersRepository, 
            IUserSettingRepository userSettingRepository, IUserRoleRepository userRoleRepository, IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            Address = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            Brands = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            Cars = carRepository ?? throw new ArgumentNullException(nameof(carRepository));
            Models = carModelRepository ?? throw new ArgumentNullException(nameof(carModelRepository));
            Clients = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
            Contacts = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
            Favorits = favoritsRepository ?? throw new ArgumentNullException(nameof(favoritsRepository));
            MessageBoox = massageBoxRepository ?? throw new ArgumentNullException(nameof(massageBoxRepository));
            Providers = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
            Sales = saleRepository ?? throw new ArgumentNullException(nameof(saleRepository));
            Users = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
            UserSettings = userSettingRepository ?? throw new ArgumentNullException(nameof(userSettingRepository));
            UserRoles = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
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
            // Como não abro o scope acima, não precisamos de dar Complete()
            /*if(_scope == null)
            {
                return 0;
            }

            _scope.Complete();*/

            await Task.CompletedTask;
            return 1;
        }

        public void Rollback()
        {
            //_scope?.Dispose();
            //_scope = null;
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
