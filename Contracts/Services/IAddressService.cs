using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Services
{
    public interface IAddressService
    {
        Task<Result<Address>> GeAddressByIdAsync(int addressId);
        Task<Result<IEnumerable<Address>>> GetAllAddressesAsync();

        Task<Result<IEnumerable<Address>>> SearchAddressAsync(string searchAddress);
        Task<Result<IEnumerable<Address>>> GetByCityAsync(string city);
        Task<Result<IEnumerable<Address>>> GetByPostalCodeAsync(string postalCode);

        Task<Result<Address>> CreateAddressAsync(Address newAddress);
        Task<Result> UpdateAddressAsync(Address addressToUpdate);
        Task<Result> DeleteAddressAsync(int addressId);

        Task<bool> ExistsAsync(int addressId);
        Task<bool> IsAddressInUseAsync(int addressId);
    }
}
