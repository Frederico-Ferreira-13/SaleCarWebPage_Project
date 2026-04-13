using Contracts.Repositories;
using Core.Model;

namespace SaleCarWebPage_Project.Repo
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        public ClientRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}