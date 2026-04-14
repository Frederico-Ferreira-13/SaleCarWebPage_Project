using Contracts.Repositories;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAddressRepository _addressRepository;

        public AddressService(IUnitOfWork unitOfWork, IAddressRepository addressRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
        }

        public async Task<Result<Address>> GeAddressByIdAsync(int addressId)
        {
            var addressResul = await _addressRepository.GetAllAsync();
            if(!addressResul.IsSuccessful)
            {
                return Result<int>.Failure(addressResul.Error);
            }

            var address = addressResul.Value;
            if(address == null)
            {
                return Result<int>.Failure(
                    Error.Unauthorized(
                        ErrorCodes.AuthUnauthorized,
                        "Dados de Morada não encontrados."));
            }
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
