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
    public class ProviderService : IProviderService
    {
        public readonly IUnitOfWork _unitOfWork;

        public ProviderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<Provider>> GetByIdAsync(int providerId)
        {
            if (providerId <= 0)
            {
                return Result<Provider>.Failure(
                    Error.Validation("ID de fornecedor inválido."));
            }

            var provider = await _unitOfWork.Providers.GetByIdAsync(providerId);
            if (provider == null)
            {
                return Result<Provider>.Failure(
                    Error.NotFound(ErrorCodes.NotFound, $"Fornecedor com ID {providerId} não encontrado.")
                );
            }

            return Result<Provider>.Success(provider);
        }

        public async Task<Result<IEnumerable<Provider>>> GetProvidersAsync()
        {
            var providers = await _unitOfWork.Providers.GetAllAsync();
            return Result<IEnumerable<Provider>>.Success(providers);
        }

        public async Task<Result<Provider>> CreateProviderAsync(Provider createProvider)
        {
            var allProviders = await _unitOfWork.Providers.GetAllAsync();
            var existingNif = allProviders.FirstOrDefault(p => p.NIF == createProvider.NIF);

            if (existingNif != null)
            {
                return Result<Provider>.Failure(Error.Conflict(
                    ErrorCodes.AlreadyExists, "Já existe um fornecedor registado com este NIF."));
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var providerToSave = new Provider(
                     createProvider.NameProvider,
                     createProvider.NIF,
                     createProvider.UserId,
                     createProvider.AddressId
                );

                await _unitOfWork.Providers.AddAsync(providerToSave);
                await _unitOfWork.CommitAsync();

                return Result<Provider>.Success(providerToSave);
            }
            catch (ArgumentException ex)
            {
                _unitOfWork.Rollback();
                return Result<Provider>.Failure(Error.Validation(ex.Message));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<Provider>.Failure(Error.InternalServer($"Erro ao criar Vendedor: {ex.Message}"));
            }
        }

        public async Task<Result<Provider>> UpdateProviderAsync(Provider updatedProvider)
        {
            var existingProvider = await _unitOfWork.Providers.GetByIdAsync(updatedProvider.ProviderId);
            if (existingProvider == null)
                return Result<Provider>.Failure(Error.NotFound(ErrorCodes.NotFound, "Fornecedor não encontrado para atualização."));

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Usar o método de atualização interno da Entidade Client
                existingProvider.UpdateProvider(
                    updatedProvider.NameProvider,
                    updatedProvider.NIF,
                    updatedProvider.UserId,
                    updatedProvider.AddressId
                );

                await _unitOfWork.Providers.UpdateAsync(existingProvider);
                await _unitOfWork.CommitAsync();

                return Result<Provider>.Success(existingProvider);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<Provider>.Failure(Error.InternalServer($"Erro ao atualizar fornecedor: {ex.Message}"));
            }
        }

        public async Task<Result> DeleteAsync(int providerId)
        {
            var provider = await _unitOfWork.Providers.GetByIdAsync(providerId);
            if (provider == null)
                return Result.Failure(Error.NotFound(ErrorCodes.NotFound, "Fornecedor não encontrado."));

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                provider.Deactivate();

                await _unitOfWork.Providers.UpdateAsync(provider);
                await _unitOfWork.CommitAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result.Failure(Error.InternalServer($"Erro ao remover fornecedor: {ex.Message}"));
            }
        }
    }
}
