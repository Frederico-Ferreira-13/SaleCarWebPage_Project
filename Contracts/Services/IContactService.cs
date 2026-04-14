using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Services
{
    public interface IContactService
    {
        Task<Result<Contact>> GetByIdAsync(int contactId);

        Task<Result<IEnumerable<Contact>>> GetAllContactAsync();

        Task<Result<Contact>> CreateAsync(Contact dto);
        Task<Result<Contact>> UpdateAsync(Contact dto);
        Task<Result> DeleteAsync(int id);

        Task<Result<List<Contact>>> GetInteractionsAsync(Guid contactId);

        Task<Result<Contact>> CreateInteractionAsync(Contact dto);
        Task<Result> UpdateInteractionAsync(Contact dto);

        Task<Result<Contact>> GetByEmailOrPhoneAsync(string identifier);
    }

}