using Contracts.Repositories;
using Core.Model;

namespace SaleCarWebPage_Project.Repo
{
    public class ProviderRepository : GenericRepository<Provider>, IProviderRepository
    {
        public ProviderRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}