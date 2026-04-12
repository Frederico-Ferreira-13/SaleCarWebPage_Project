using Core.Model;
using SaleCarWebPage_Project.Contracts;

namespace SaleCarWebPage_Project.Repo
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {
        public BrandRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}