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
                var proposals = await _unitOfWork.Sales.GetByCarIdAsync(carId);

                var allSales = await _unitOfWork.Sales.GetAllAsync();
                var carProposals = allSales.Where(s => s.CarId == carId).ToList();

                return Result<IEnumerable<Sale>>.Success(carProposals);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Sale>>.Failure(Error.InternalServer($"Erro: {ex.Message}"));
            }
        }

        public async Task<int?> GetClientIdByUserIdAsync(int userId)
        {
            var allClients = await _unitOfWork.Clients.GetAllAsync();
            var client = allClients.FirstOrDefault(c => c.UserId == userId);

            return client?.ClientId;
        }
    }
}
