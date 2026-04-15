using Core.Common;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Core.Model
{
    public class Car : IEntity, ISoftDeletable
    {
        public int CarId { get; private set; }
        public int ModelId { get; private set; }
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

        public int FavoriteCount { get; set; } = 0;
        public bool IsFavorite { get; set; }
        public double AverageRating { get; set; } = 0.0;

        public virtual CarModel? Model { get; private set; }
        public virtual Provider? Provider { get; private set; }

        public virtual Users? User { get; private set; }

        private const int MaxPlateNumberLength = 20;
        private const int MaxColorLength = 50;
        private const int MaxFuelTypeLength = 50;
        private const int MinYear = 1886; // O ano do primeiro carro
        private static readonly int MaxYear = DateTime.Now.Year + 1; // Permitir carros do próximo ano
        private const int MaxEngineCapacity = 100; // Capacidade máxima do motor em litros
        private const decimal MaxCarTare = 100000.0m; // Peso máximo do carro em kg
        private const decimal MaxCarPrice = 10000000.0m; // Preço máximo do carro em moeda local
        private const int MaxKilometers = 1000000; // Quilometragem máxima
        private const int MinKilometers = 0; // Quilometragem mínima
        private const int MaxFavoriteCount = 1000000; // Contagem máxima de favoritos
        private const int MinFavoriteCount = 0; // Contagem mínima de favoritos
        private const int MaxRating = 5; // Avaliação máxima
        private const int MinRating = 1; // Avaliação mínima
        private const int MinTitleLength = 5; // Comprimento mínimo do título
        private const int MaxTitleLength = 100; // Comprimento máximo do título

        public Car() { }

        public Car(int modelId, int providerId, string typeOfFuel, string carColor, int engineCapacity,
                   decimal carTare, decimal carPrice, string plateNumber, int year, int kilometers)
        {
            Validate(modelId, providerId, typeOfFuel, carColor, engineCapacity, carTare, carPrice, plateNumber, year, kilometers);

            ModelId = modelId;
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

        public Car(int carId, int modelId, int providerId, string typeOfFuel, string carColor, int engineCapacity,
                    decimal carTare, decimal carPrice, bool isAvailable, string plateNumber, int year, int kilometers,
                    string? imageUrl, DateTime createdAt, DateTime? lastUpdatedAt, bool isActive, bool isApproved)
        {
            CarId = carId;
            ModelId = modelId;
            ProviderId = providerId;
            TypeOfFuel = typeOfFuel;
            CarColor = carColor;
            EngineCapacity = engineCapacity;
            CarTare = carTare;
            CarPrice = carPrice;
            IsAvailable = isAvailable;
            PlateNumber = plateNumber;
            Year = year;
            Kilometers = kilometers;
            ImageUrl = imageUrl;
            CreatedAt = createdAt;
            LastUpdatedAt = lastUpdatedAt;
            IsActive = isActive;
            IsApproved = isApproved;
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0) 
            {
                throw new ArgumentException("O preço deve ser maior que zero.");
            }

            CarPrice = newPrice;
            SetLastUpdatedAt();
        }

        public void MarkAsSold()
        {
            IsAvailable = false;
            SetLastUpdatedAt();
        }

        public void SetImageUrl(string url)
        {
            this.ImageUrl = url;
            SetLastUpdatedAt();
        }

        public void Approve()
        {
            if (!IsApproved)
            {
                IsApproved = true;
                SetLastUpdatedAt();
            }
        }

        public void Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                SetLastUpdatedAt();
            }
        }

        private void Validate(int modelId, int providerId, string fuel, string color, decimal engine,
                              decimal tare, decimal price, string plate, int year, int km)
        {
            if (modelId <= 0) 
            {
                throw new ArgumentException("ModelId inválido.");
            }
            if (providerId <= 0) 
            {
                throw new ArgumentException("ProviderId inválido.");
            } 
            if (string.IsNullOrWhiteSpace(fuel)) 
            {
                throw new ArgumentException("Tipo de combustível é obrigatório.");
            } 
            if (string.IsNullOrWhiteSpace(plate)) 
            {
                throw new ArgumentException("Matrícula é obrigatória.");
            } 
            if (price <= 0) 
            {
                throw new ArgumentException("O preço deve ser positivo.");
            }
            if (km < 0) 
            {
                throw new ArgumentException("Quilometragem não pode ser negativa.");
            } 

            if (year < MinYear || year > DateTime.Now.Year + MaxYear) 
            {
                throw new ArgumentException($"O ano deve estar entre {MinYear} e {DateTime.Now.Year + MaxYear}.");
            }               
        }

        private void SetLastUpdatedAt()
        {
            LastUpdatedAt = DateTime.UtcNow;
        }

        public bool IsDeleted => !IsActive;

        public int GetId() => CarId;

        public void SetId(int id)
        {
            if (CarId != 0) 
            {
                throw new InvalidOperationException("ID já definido.");
            } 
            CarId = id;
        }

        public bool GetIsActive() => IsActive;        
    }
}
