using Core.Model;
using SaleCarWebPage_Project.Contracts;

namespace SaleCarWebPage_Project.Repo
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        public ClientRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}