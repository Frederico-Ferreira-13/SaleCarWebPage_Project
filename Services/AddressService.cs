using Contracts.Repositories;
using Contracts.Services;
using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<Address>> GeAddressByIdAsync(int addressId)
        {
            // O repositório devolve a entidade diretamente, não um Result
            var address = await _unitOfWork.Address.GetByIdAsync(addressId);

            if (address == null)
            {
                return Result<Address>.Failure(Error.NotFound(
                    ErrorCodes.NotFound,
                    "Dados de Morada não encontrados."));
            }

            return Result<Address>.Success(address);
        }

        public async Task<Result<IEnumerable<Address>>> GetAllAddressesAsync()
        {
            var addresses = await _unitOfWork.Address.GetAllAsync();
            return Result<IEnumerable<Address>>.Success(addresses);
        }

        public async Task<Result<Address>> CreateAddressAsync(Address newAddress)
        {
            await _unitOfWork.Address.AddAsync(newAddress);
            await _unitOfWork.CommitAsync();
            return Result<Address>.Success(newAddress);
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
            await _unitOfWork.Address.UpdateAsync(addressToUpdate);
            await _unitOfWork.CommitAsync();
            return Result.Success();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result.Failure(Error.InternalServer($"Erro ao atualizar morada: {ex.Message}"));
            }
        }

        public async Task<Result> DeleteAddressAsync(int addressId)
        {
            await _unitOfWork.Address.DeleteAsync(addressId);
            await _unitOfWork.CommitAsync();
            return Result.Success();
        }

        // Implementação básica dos outros métodos para não dar erro de compilação
        public async Task<Result<IEnumerable<Address>>> SearchAddressAsync(string searchAddress) =>
            Result<IEnumerable<Address>>.Success((await _unitOfWork.Address.GetAllAsync()).Where(a => a.Street.Contains(searchAddress)));

        public async Task<Result<IEnumerable<Address>>> GetByCityAsync(string city) =>
            Result<IEnumerable<Address>>.Success((await _unitOfWork.Address.GetAllAsync()).Where(a => a.City == city));

        public async Task<Result<IEnumerable<Address>>> GetByPostalCodeAsync(string postalCode) =>
            Result<IEnumerable<Address>>.Success((await _unitOfWork.Address.GetAllAsync()).Where(a => a.PostalCode == postalCode));

        public async Task<bool> ExistsAsync(int addressId) => (await _unitOfWork.Address.GetByIdAsync(addressId)) != null;

        public async Task<bool> IsAddressInUseAsync(int addressId) => false; // Lógica a implementar depois
    }
}