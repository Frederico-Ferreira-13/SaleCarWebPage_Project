using Core.Model;
using Contracts.Repositories;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SaleCarWebPage_Project.Repo
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        public ClientRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Método para procurar por Email
        public async Task<Client> GetByEmailAsync(string email)
        {
            return await _context.Set<Client>()
                                 .FirstOrDefaultAsync(x => x.Email == email);
        }

        // Método para procurar por NIF
        public async Task<Client> GetByNifAsync(string nif)
        {
            return await _context.Set<Client>()
                                 .FirstOrDefaultAsync(x => x.Nif == nif);
        }
    }
}