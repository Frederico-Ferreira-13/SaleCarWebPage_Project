using Contracts.Repositories;
using Contracts.Services;
using Core.Common;
using Core.Model;
using SaleCarWebPage_Project.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ClientService : IClientService
    {
        public readonly IUnitOfWork _unitOfWork;
        public ClientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<Client>> GetByIdAsync(int clientId)
        {
            if (clientId <= 0)
            {
                return Result<Client>.Failure(
                    Error.Validation("ID de cliente inválido."));
            }

            var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
            if (client == null)
            {
                return Result<Client>.Failure(
                    Error.NotFound(ErrorCodes.NotFound, $"Cliente com ID {clientId} não encontrado.")
                );
            }            

            return Result<Client>.Success(client);
        }

        public async Task<Result<IEnumerable<Client>>> GetClientAsync()
        {
            var clients = await _unitOfWork.Clients.GetAllAsync();
            return Result<IEnumerable<Client>>.Success(clients);
        }

        public async Task<Result<Client>> CreateClientAsync(Client createClient)
        {
            var existingNif = await _unitOfWork.Clients.GetByNifAsync(createClient.Nif);
            if (existingNif != null)
            {
                return Result<Client>.Failure(Error.Conflict(
                    ErrorCodes.AlreadyExists, "Já existe um cliente registado com este NIF."));
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var clientToSave = new Client(
                     createClient.ClientName,
                     createClient.Nif,
                     createClient.ContactId,
                     createClient.UserId,
                     createClient.AddressId
                 );

                await _unitOfWork.Clients.AddAsync(clientToSave);
                await _unitOfWork.CommitAsync();

                return Result<Client>.Success(clientToSave);
            }
            catch (ArgumentException ex)
            {
                _unitOfWork.Rollback();
                return Result<Client>.Failure(Error.Validation(ex.Message));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<Client>.Failure(Error.InternalServer($"Erro ao criar cliente: {ex.Message}"));
            }
        }

        public async Task<Result<Client>> UpdateClientAsync(Client updatedClient)
        {
            var existingClient = await _unitOfWork.Clients.GetByIdAsync(updatedClient.ClientId);
            if (existingClient == null)
                return Result<Client>.Failure(Error.NotFound(ErrorCodes.NotFound, "Cliente não encontrado para atualização."));

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Usar o método de atualização interno da Entidade Client
                existingClient.UpdateClient(
                    updatedClient.ClientName,
                    updatedClient.Nif,
                    updatedClient.UserId,
                    updatedClient.ContactId,
                    updatedClient.AddressId
                );

                await _unitOfWork.Clients.UpdateAsync(existingClient);
                await _unitOfWork.CommitAsync();

                return Result<Client>.Success(existingClient);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<Client>.Failure(Error.InternalServer($"Erro ao atualizar cliente: {ex.Message}"));
            }
        }

        public async Task<Result> DeleteAsync(int clientId)
        {
            var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
            if (client == null)
                return Result.Failure(Error.NotFound(ErrorCodes.NotFound, "Cliente não encontrado."));

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                client.Deactivate();

                await _unitOfWork.Clients.UpdateAsync(client);
                await _unitOfWork.CommitAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result.Failure(Error.InternalServer($"Erro ao remover cliente: {ex.Message}"));
            }
        }
    }
}
