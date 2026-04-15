using Contracts.Repositories;
using Core.Model;


namespace SaleCarWebPage_Project.Repo
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
