using Contracts.Repositories;
using Contracts.Services;
using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAddressRepository _addressRepository;
        private readonly IUsersService _usersService;

        public AddressService(IUnitOfWork unitOfWork, IAddressRepository addressRepository, IUsersService usersService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        public async Task<Result<Address>> GeAddressByIdAsync(int addressId)
        {
            var address = await _addressRepository.GetByIdAsync(addressId);
            if (address == null)
            {
                return Result<Address>.Failure(
                    Error.NotFound(
                    ErrorCodes.NotFound,
                    $"Morada não encontrado.")
                );
            }

            return Result<Address>.Success(address);
        }

        public async Task<Result<IEnumerable<Address>>> GetAllAddressesAsync()
        {
            var adresses = await _unitOfWork.Address.GetAllAsync();
            return Result<IEnumerable<Address>>.Success(adresses);
        }

        public async Task<Result<IEnumerable<Address>>> SearchAddressAsync(string searchAddress)
        {
            var adresses = await _unitOfWork.Address.GetAllAsync();
            var filtered = adresses.Where(a =>
                a.Street.Contains(searchAddress, StringComparison.OrdinalIgnoreCase) ||
                a.City.Contains(searchAddress, StringComparison.OrdinalIgnoreCase));

            return Result<IEnumerable<Address>>.Success(filtered);
        }

        public async Task<Result<IEnumerable<Address>>> GetByCityAsync(string city)
        {
            var all = await _unitOfWork.Address.GetAllAsync();
            return Result<IEnumerable<Address>>.Success(all.Where(
                a => a.City.Equals(city, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<Result<IEnumerable<Address>>> GetByPostalCodeAsync(string postalCode)
        {
            var all = await _unitOfWork.Address.GetAllAsync();
            return Result<IEnumerable<Address>>.Success(all.Where(
                a => a.PostalCode.Equals(postalCode, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<Result<Address>> CreateAddressAsync(Address newAddress)
        {
            if (string.IsNullOrWhiteSpace(newAddress.Street) || string.IsNullOrWhiteSpace(newAddress.City))
            {
                return Result<Address>.Failure(Error.Validation("Rua e Cidade são obrigatórias."));
            }           

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Address.AddAsync(newAddress);
                await _unitOfWork.CommitAsync();
                return Result<Address>.Success(newAddress);                
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<Address>.Failure(Error.InternalServer($"Erro ao criar morada: {ex.Message}"));
            }            
        }

        public async Task<Result> UpdateAddressAsync(Address addressToUpdate)
        {
            var existing = await _unitOfWork.Address.GetByIdAsync(addressToUpdate.AddressId);
            if (existing != null)
            {
                return Result.Failure(Error.NotFound(
                    ErrorCodes.NotFound,
                    $"Morada com ID {addressToUpdate.AddressId} não encontrado."));
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                existing.UpdateDetails(
                    addressToUpdate.Street,
                    addressToUpdate.Street2,
                    addressToUpdate.DoorNumber,
                    addressToUpdate.Floor,
                    addressToUpdate.PostalCode,
                    addressToUpdate.City,
                    addressToUpdate.Country,
                    addressToUpdate.Locate);
                
                _unitOfWork.Address.UpdateAsync(existing);
                await _unitOfWork.CommitAsync();
                return Result.Success("Morada atualizada com sucesso.");

            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result.Failure(Error.InternalServer($"Erro ao atualizar morada: {ex.Message}"));
            }
        }

        public async Task<Result> DeleteAddressAsync(int addressId)
        {
            if (await IsAddressInUseAsync(addressId))
            {
                return Result.Failure(Error.BusinessRuleViolation(ErrorCodes.BizInvalidOperation, "Esta morada está em uso por um cliente ou fornecedor."));
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Address.DeleteAsync(addressId);
                await _unitOfWork.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result.Failure(Error.InternalServer(ex.Message));
            }
        }

        public async Task<bool> ExistsAsync(int addressId)
        {
            return await _unitOfWork.Address.GetByIdAsync(addressId) != null;
        }

        public async Task<bool> IsAddressInUseAsync(int addressId)
        {
            var clientInUse = (await _unitOfWork.Clients.GetAllAsync()).Any(c => c.AddressId == addressId);
            var providerInUse = (await _unitOfWork.Providers.GetAllAsync()).Any(p => p.AddressId == addressId);

            return clientInUse || providerInUse;
        }
    }
}
