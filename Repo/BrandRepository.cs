using Contracts.Repositories;
using Core.Model;

namespace SaleCarWebPage_Project.Repo
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {
        public BrandRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}