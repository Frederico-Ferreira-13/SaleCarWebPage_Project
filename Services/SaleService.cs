using Contracts.Repositories;
using Contracts.Services;
using Core.Common;
using Core.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class SaleService : ISaleService
    {
        public readonly IUnitOfWork _unitOfWork;

        public SaleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result> AddAsync(Sale sale)
        {
            try
            {
                await _unitOfWork.Sales.AddAsync(sale);
                await _unitOfWork.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(Error.InternalServer($"Erro ao guardar proposta: {ex.Message}"));
            }
        }

        public async Task<Result> MarkAsSoldAsync(int carId, int clientId, decimal finalPrice)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(carId);
            if (car == null || !car.IsActive)
            {
                return Result.Failure(Error.NotFound(ErrorCodes.NotFound, "Carro não encontrado ou já não está disponível."));
            }
            
            var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
            if (client == null)
            {
                return Result.Failure(Error.NotFound(ErrorCodes.NotFound, "Cliente não encontrado."));
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {                
                var newSale = new Sale(
                    carId,
                    clientId,
                    DateTime.UtcNow,
                    finalPrice,
                    DateTime.UtcNow,
                    "Transferência Bancária" 
                );

                await _unitOfWork.Sales.AddAsync(newSale);
                
                car.Deactivate();
                await _unitOfWork.Cars.UpdateAsync(car);

                await _unitOfWork.CommitAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result.Failure(Error.InternalServer($"Erro ao processar venda: {ex.Message}"));
            }
        }

        public async Task<Result<IEnumerable<Car>>> GetSoldInventoryAsync(int providerId)
        {
            var allCars = await _unitOfWork.Cars.GetAllAsync();

            var soldCars = allCars.Where(c => c.ProviderId == providerId && !c.IsActive);

            return Result<IEnumerable<Car>>.Success(soldCars);
        }

        public async Task<Result<IEnumerable<Sale>>> GetProposalsByCarIdAsync(int carId)
        {
            try
            {
                var proposals = await _unitOfWork.Sales.GetProposalsByCarIdAsync(carId);
                return Result<IEnumerable<Sale>>.Success(proposals);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Sale>>.Failure(Error.InternalServer($"Erro: {ex.Message}"));
            }
        }

        public async Task<Result<IEnumerable<Sale>>> GetUserNegotiationsAsync(int userId, HashSet<int> myCarIds)
        {
            try
            {
                var sales = await _unitOfWork.Sales.GetUserNegotiationsAsync(userId, myCarIds);
                return Result<IEnumerable<Sale>>.Success(sales);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Sale>>.Failure(Error.InternalServer($"Erro ao obter negociações: {ex.Message}"));
            }
        }

        public async Task<int?> GetClientIdByUserIdAsync(int userId)
        {
            var allClients = await _unitOfWork.Clients.GetAllAsync();
            var client = allClients.FirstOrDefault(c => c.UserId == userId);

            return client?.ClientId;
        }

        public async Task<int> EnsureClientProfileExistsAsync(int userId)
        {
            // 1. Verificar se já existe como Cliente
            var clients = await _unitOfWork.Clients.GetAllAsync();
            var client = clients.FirstOrDefault(c => c.UserId == userId);
            if (client != null) return client.ClientId;

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            string realName = user?.Name ?? "Utilizador Desconhecido";

            // 2. Se não existe, vamos tentar copiar os dados do perfil de Provider (Vendedor)
            var providers = await _unitOfWork.Providers.GetAllAsync();
            var provider = providers.FirstOrDefault(p => p.UserId == userId);

            Client newClient;

            if (provider != null)
            {
                // Aproveita os dados que o utilizador já preencheu como vendedor
                newClient = new Client(
                    provider.NameProvider,
                    provider.NIF,
                    1, 
                    userId,
                    provider.AddressId
                );
            }
            else
            {
                // Se nem como provider existe (user novo), cria com dados genéricos 
                // ou redireciona para completar perfil
                newClient = new Client(realName, "000000000", 1, userId, 1);
            }

            await _unitOfWork.Clients.AddAsync(newClient);
            await _unitOfWork.CommitAsync();

            return newClient.ClientId;
        }

        public async Task<Result<IEnumerable<Sale>>> GetAllAsync()
        {
            try
            {
                // Busca todas as vendas/propostas através do repositório
                var sales = await _unitOfWork.Sales.GetAllAsync();
                return Result<IEnumerable<Sale>>.Success(sales);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Sale>>.Failure(Error.InternalServer($"Erro ao obter propostas: {ex.Message}"));
            }
        }

        public async Task<Result<IEnumerable<Sale>>> GetProposalsByUserIdAsync(int userId)
        {
            try
            {                
                var allSales = await _unitOfWork.Sales.GetAllAsync();                
                var userSales = allSales.Where(s => s.ClientId == userId || s.CarId != 0);

                return Result<IEnumerable<Sale>>.Success(userSales);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Sale>>.Failure(Error.InternalServer($"Erro: {ex.Message}"));
            }
        }        
    }
}
