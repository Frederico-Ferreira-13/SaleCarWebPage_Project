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
    public class CarModelService : ICarModelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICarModelRepository _carModelRepository;

        public CarModelService(IUnitOfWork unitOfWork, IAuthenticationService authenticationService, ICarModelRepository carModelRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _carModelRepository = carModelRepository ?? throw new ArgumentNullException(nameof(carModelRepository));
        }

        public async Task<Result<CarModel>> GetCarModelIdAsync(int modelIdd)
        {
            var modelIdResult = await GetAllBrandsAsync();
            if (!modelIdResult.IsSuccessful)
            {
                return Result<Brand>.Failure(modelIdResult.Error);
            }

            var carModel = await _carModelRepository.GetByIdAsync(modelIdd, modelIdResult.Value);
            if (carModel == null)
            {
                return Result<CarModel>.Failure(
                    Error.NotFound(ErrorCodes.NotFound, $"CarModel com ID {modelIdd} não encontrada ou não pertence à sua conta.")
                );
            }

            return Result<Brand>.Success(carModel);
        }

        public async Task<Result<CarModel>> GetByNameAsync(string modelName)
        {
            if (string.IsNullOrWhiteSpace(modelName))
            {
                return Result<CarModel>.Failure(
                    Error.Validation(
                        "Nome do modelo é obrigatório.",
                        new Dictionary<string, string[]> { { nameof(modelName), new[] { "Campo obrigatório" } } }
                    )
                );
            }

            var carModel = await _unitOfWork.CarModels.GetByNameAsync(modelName);
            if (carModel == null)
            {
                return Result<CarModel>.Failure(
                    Error.NotFound(
                    ErrorCodes.NotFound,
                    $"Modelo com nome '{modelName}' não encontrado.")
                );
            }

            return Result<Brand>.Success(carModel);
        }

        public async Task<Result<IEnumerable<CarModel>>> GetAllCarModelAsync()
        {
            var carModels = await _unitOfWork.CarModels.GetAllAsync();
            return Result<IEnumerable<CarModel>>.Success(carModels);
        }

        public async Task<Result<CarModel>> CreateCarModelAsync(CarModel carModel)
        {
            if (string.IsNullOrWhiteSpace(carModel.ModelName))
            {
                return Result<CarModel>.Failure(
                     Error.Validation(
                         "O nome do modelo é obrigatório.",
                         new Dictionary<string, string[]> { { nameof(carModel.ModelName), new[] { "Campo obrigatório" } } }
                     )
                 );
            }

            if (carModel.ModelName.Length > 50)
            {
                return Result<CarModel>.Failure(
                    Error.Validation(
                        "O nome do modelo não pode exceder 50 caracteres.",
                        new Dictionary<string, string[]> { { nameof(carModel.ModelName), new[] { "Máximo 50 caracteres" } } }
                    )
                );
            }

            if (await _unitOfWork.CarModels.GetByNameAsync(carModel.ModelName) != null)
            {
                return Result<CarModel>.Failure(
                    Error.Validation(
                    $"O modelo '{carModel.ModelName}' já existe.",
                    new Dictionary<string, string[]> { { nameof(carModel.ModelName), new[] { "Nome já em uso." } } })
                );
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var newCarModel = new CarModel(carModel.ModelName);
                await _unitOfWork.CarModels.CreateAddAsync(newCarModel);
                await _unitOfWork.CommitAsync();

                return Result<CarModel>.Success(newCarModel);
            }
            catch (ArgumentException ex)
            {
                _unitOfWork.Rollback();

                string fieldName = ex.ParamName ?? "Geral";
                return Result<CarModel>.Failure(
                    Error.Validation(
                        "Dados de entrada inválidos para o modelo.",
                        new Dictionary<string, string[]> { { fieldName, new[] { ex.Message } } }));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();

                return Result<CarModel>.Failure(
                    Error.InternalServer($"Erro inesperado ao criar modelo: {ex.Message}"));
            }
        }

        public async Task<Result<CarModel>> UpdateCarModelAsync(CarModel updateModel)
        {
            var existingCarModel = await _unitOfWork.CarModels.GetByIdAsync(updateModel.ModelId);
            if (existingCarModel == null)
            {
                return Result<CarModel>.Failure(
                    Error.NotFound(ErrorCodes.NotFound, $"Modelo com ID {updateModel.ModelId} não encontrado.")
                );
            }

            if (!string.Equals(existingCarModel.ModelName, updateModel.ModelName, StringComparison.OrdinalIgnoreCase))
            {
                if (await _unitOfWork.CarModels.GetByNameAsync(updateModel.ModelName) != null)
                {
                    return Result<CarModel>.Failure(
                        Error.Conflict(
                            ErrorCodes.AlreadyExists,
                            $"O nome '{updateModel.ModelName}' já está em uso.",
                            new Dictionary<string, string[]> { { nameof(updateModel.ModelName), new[] { "Nome já em uso" } } }
                        )
                    );
                }
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                existingCarModel.UpdateModelName(updateModel.ModelName);
                await _unitOfWork.CarModels.UpdateAsync(existingCarModel);
                await _unitOfWork.CommitAsync();

                return Result<CarModel>.Success(existingCarModel);
            }
            catch (ArgumentException ex)
            {
                _unitOfWork.Rollback();
                return Result<CarModel>.Failure(
                    Error.Validation("Dados inválidos para atualizar modelo.",
                    new Dictionary<string, string[]> { { ex.ParamName ?? "Geral", new[] { ex.Message } } })
                );
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<CarModel>.Failure(
                    Error.InternalServer($"Erro inesperado ao atualizar modelo: {ex.Message}"));
            }
        }

        public async Task<Result> DeleteCarModelAsync(int modelId)
        {
            var existingCarModel = await _unitOfWork.CarModels.GetByIdAsync(modelId);
            if (existingCarModel == null)
            {
                return Result.Success();
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.CarModels.DeleteAsync(existingCarModel);
                await _unitOfWork.CommitAsync();
                return Result.Success("Modelo excluído com sucesso.");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer($"Erro ao eliminar modelo: {ex.Message}"));
            }
        }
    }
}
