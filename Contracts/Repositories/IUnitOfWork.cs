using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
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

        Task BeginTransactionAsync();
        Task<int> CommitAsync();
        void Rollback();
    }
}
