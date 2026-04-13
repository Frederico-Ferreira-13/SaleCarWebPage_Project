using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repositories
{
    public interface IUnitOfWork
    {
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

        Task BeginTransactionAsync();
        Task<int> CommitAsync();
        void Rollback();
    }
}
