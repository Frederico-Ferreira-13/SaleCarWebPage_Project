using Core.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Model
{
    public class Car : IEntity, ISoftDeletable
    {
        [Key]
        public int CarId { get; private set; }

        [Column("ModelId")]
        public int CarModelId { get; private set; }
        public int ProviderId { get; private set; }

        public string TypeOfFuel { get; private set; } = string.Empty;
        public string CarColor { get; private set; } = string.Empty;
        public int EngineCapacity { get; private set; }
        public decimal CarTare { get; private set; }
        public decimal CarPrice { get; private set; }
        public bool IsAvailable { get; private set; }
        public string PlateNumber { get; private set; } = string.Empty;
        public int Year { get; private set; }
        public int Kilometers { get; private set; }
        public string? ImageUrl { get; protected set; }       

        public DateTime CreatedAt { get; protected set; }
        public DateTime? LastUpdatedAt { get; protected set; }
        public bool IsActive { get; private set; }
        public bool IsApproved { get; private set; }

        // --- PROPRIEDADES APENAS DE CÓDIGO (NÃO VÃO PARA A TABELA DBO.CAR) ---
        [NotMapped]
        public int FavoriteCount { get; set; } = 0;

        [NotMapped]
        public bool IsFavorite { get; set; }
        [NotMapped]
        public string? Location { get; set; }

        // --- NAVEGAÇÃO ---
        [ForeignKey("CarModelId")]
        public virtual CarModel? Model { get; private set; }

        [ForeignKey("ProviderId")]
        public virtual Provider? Provider { get; private set; }

        // --- CONSTRUTORES ---
        public Car() { }

        public Car(int carModelId, int providerId, string typeOfFuel, string carColor, int engineCapacity,
                   decimal carTare, decimal carPrice, string plateNumber, int year, int kilometers)
        {
            CarModelId = carModelId;
            ProviderId = providerId;
            TypeOfFuel = typeOfFuel;
            CarColor = carColor;
            EngineCapacity = engineCapacity;
            CarTare = carTare;
            CarPrice = carPrice;
            PlateNumber = plateNumber;
            Year = year;
            Kilometers = kilometers;

            IsAvailable = true;
            IsActive = true;
            IsApproved = false;
            CreatedAt = DateTime.UtcNow;
        }

        // --- MÉTODOS OBRIGATÓRIOS (CORREÇÃO DO ERRO CS0535) ---

        public void Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                SetLastUpdatedAt();
            }
        }

        public bool IsDeleted => !IsActive;

        public int GetId() => CarId;

        public void SetId(int id) => CarId = id;

        public bool GetIsActive() => IsActive;

        // --- OUTROS MÉTODOS ---

        public void Approve()
        {
            IsApproved = true;
            SetLastUpdatedAt();
        }

        public void SetImageUrl(string url)
        {
            this.ImageUrl = url;
            SetLastUpdatedAt();
        }

        private void SetLastUpdatedAt() => LastUpdatedAt = DateTime.UtcNow;

        public void MarkAsSold()
        {
            throw new NotImplementedException();
        }
    }
}