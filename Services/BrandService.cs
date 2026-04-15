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
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly IBrandRepository _brandRepository;

        public BrandService(IUnitOfWork unitOfWork, IAuthenticationService authenticationService, IBrandRepository brandRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        }

        public async Task<Result<Brand>> GetBrandIdAsync(int brandId)
        {
            var brandIdResult = await GetAllBrandsAsync();
            if (!brandIdResult.IsSuccessful)
            {
                return Result<Brand>.Failure(brandIdResult.Error);
            }

            var brand = await _brandRepository.ReadByIdAndBrandAsync(brandId, brandIdResult.Value);
            if (brand == null)
            {
                return Result<Brand>.Failure(
                    Error.NotFound(ErrorCodes.NotFound, $"Branding com ID {brandId} não encontrada ou não pertence à sua conta.")
                );
            }

            return Result<Brand>.Success(brand);
        }

        public async Task<Result<Brand>> GetByNameAsync(string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
            {
                return Result<Brand>.Failure(
                    Error.Validation(
                        "Nome da marca é obrigatório.",
                        new Dictionary<string, string[]> { { nameof(brandName), new[] { "Campo obrigatório" } } }
                    )
                );
            }

            var brand = await _unitOfWork.Brands.GetByNameAsync(brandName);
            if (brand == null)
            {
                return Result<Brand>.Failure(
                    Error.NotFound(
                    ErrorCodes.NotFound,
                    $"Marca com nome '{brandName}' não encontrada.")
                );
            }

            return Result<Brand>.Success(brand);
        }

        public async Task<Result<IEnumerable<Brand>>> GetAllBrandsAsync()
        {
            var brands = await _unitOfWork.Brands.GetAllAsync();
            return Result<IEnumerable<Brand>>.Success(brands);
        }

        public async Task<Result<Brand>> CreateBrandAsync(Brand brand)
        {
            if (string.IsNullOrWhiteSpace(brand.BrandName))
            {
                return Result<Brand>.Failure(
                     Error.Validation(
                         "O nome da marca é obrigatório.",
                         new Dictionary<string, string[]> { { nameof(brand.BrandName), new[] { "Campo obrigatório" } } }
                     )
                 );
            }

            if (brand.BrandName.Length > 50)
            {
                return Result<Brand>.Failure(
                    Error.Validation(
                        "O nome da marca não pode exceder 50 caracteres.",
                        new Dictionary<string, string[]> { { nameof(brand.BrandName ), new[] { "Máximo 50 caracteres" } } }
                    )
                );
            }

            if (await _unitOfWork.Brands.GetByNameAsync(brand.BrandName) != null)
            {
                return Result<Brand>.Failure(
                    Error.Validation(
                    $"A marca '{brand.BrandName}' já existe.",
                    new Dictionary<string, string[]> { { nameof(brand.BrandName), new[] { "Nome já em uso." } } })
                );
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var newBrand = new Brand(brand.BrandName);
                await _unitOfWork.Brands.CreateAddAsync(newBrand);
                await _unitOfWork.CommitAsync();

                return Result<Brand>.Success(newBrand);
            }
            catch (ArgumentException ex)
            {
                _unitOfWork.Rollback();

                string fieldName = ex.ParamName ?? "Geral";
                return Result<Brand>.Failure(
                    Error.Validation(
                        "Dados de entrada inválidos para a marca.",
                        new Dictionary<string, string[]> { { fieldName, new[] { ex.Message } } }));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();

                return Result<Brand>.Failure(
                    Error.InternalServer($"Erro inesperado ao criar marca: {ex.Message}"));
            }
        }

        public async Task<Result<Brand>> UpdateBrandAsync(Brand updateBrand)
        {
            var exisntingBrand = await _unitOfWork.Brands.GetByIdAsync(updateBrand.BrandId);
            if (exisntingBrand == null)
            {
                return Result<Brand>.Failure(
                    Error.NotFound(ErrorCodes.NotFound, $"Brand com ID {updateBrand.BrandId} não encontrado.")
                );
            }

            if (!string.Equals(exisntingBrand.BrandName, updateBrand.BrandName, StringComparison.OrdinalIgnoreCase))
            {
                if (await _unitOfWork.Brands.GetByNameAsync(updateBrand.BrandName) != null)
                {
                    return Result<Brand>.Failure(
                        Error.Conflict(
                            ErrorCodes.AlreadyExists,
                            $"O nome '{updateBrand.BrandName}' já está em uso.",
                            new Dictionary<string, string[]> { { nameof(updateBrand.BrandName), new[] { "Nome já em uso" } } }
                        )
                    );
                }
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                exisntingBrand.UpdateBrandName(updateBrand.BrandName);
                await _unitOfWork.Brands.UpdateAsync(exisntingBrand);
                await _unitOfWork.CommitAsync();

                return Result<Brand>.Success(exisntingBrand);
            }
            catch (ArgumentException ex)
            {
                _unitOfWork.Rollback();
                return Result<Brand>.Failure(
                    Error.Validation("Dados inválidos para atualizar Brand.",
                    new Dictionary<string, string[]> { { ex.ParamName ?? "Geral", new[] { ex.Message } } })
                );
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<Brand>.Failure(
                    Error.InternalServer($"Erro inesperado ao atualizar Brand: {ex.Message}"));
            }
        }

        public async Task<Result> DeleteBrandAsync(int brandId)
        {
            var existingBrand = await _unitOfWork.Brands.GetByIdAsync(brandId);
            if(existingBrand == null)
            {
                return Result.Success();
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Brands.DeleteAsync(existingBrand);
                await _unitOfWork.CommitAsync();
                return Result.Success("Brand excluída com sucesso.");
            }
            catch(Exception ex)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer($"Erro ao eliminar Brand: {ex.Message}"));
            }
        }
    }
}
