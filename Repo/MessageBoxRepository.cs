using Core.Model;
using Contracts.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SaleCarWebPage_Project.Repo
{
    public class MessageBoxRepository : GenericRepository<MessageBox>, IMessageBoxRepository
    {
        public MessageBoxRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MessageBox>> GetUnreadMessagesAsync()
        {
            // Assume que existe uma propriedade IsRead ou similar na Model
            // Se der erro no 'IsRead', podes comentar a linha e devolver a lista toda
            return await _context.Set<MessageBox>()
                                 .Where(x => x.IsRead == false)
                                 .ToListAsync();
        }
    }
}