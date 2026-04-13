using Contracts.Services;
using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AddressService : IAddressService
    {


        public async Task<Result<Address>> GeAddressByIdAsync(int addressId)
        {

        }

        public async Task<Result<IEnumerable<Address>>> GetAllAddressesAsync()
        {

        }

        public async Task<Result<IEnumerable<Address>>> SearchAddressAsync(string searchAddress)
        {

        } 
        public async Task<Result<IEnumerable<Address>>> GetByCityAsync(string city)
        {

        }

        public async Task<Result<IEnumerable<Address>>> GetByPostalCodeAsync(string postalCode)
        {

        }

        public async Task<Result<Address>> CreateAddressAsync(Address newAddress)
        {

        }

        public async Task<Result> UpdateAddressAsync(Address addressToUpdate)
        {

        }

        public async Task<Result> DeleteAddressAsync(int addressId)
        {

        }

        public async Task<bool> ExistsAsync(int addressId)
        {

        }

        public async Task<bool> IsAddressInUseAsync(int addressId)
        {

        }
    }
}
