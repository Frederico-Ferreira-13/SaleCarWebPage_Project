using Core.Model;
using SaleCarWebPage_Project.Contracts;

namespace SaleCarWebPage_Project.Repo
{
    public class ProviderRepository : GenericRepository<Provider>, IProviderRepository
    {
        public ProviderRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}