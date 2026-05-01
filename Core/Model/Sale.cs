using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    public class Sale : IEntity
    {
        [Key]
        public int SaleId { get; private set; }
        public int CarId { get; private set; }
        public int ClientId { get; private set; }        
        public DateTime SaleDate { get; private set; }
        public decimal FinalPrice { get; private set; }
        public DateTime PurchaseDate { get; private set; }
        public string PaymentMethod { get; private set; } = string.Empty;

        [ForeignKey("ClientId")]
        public virtual Client? Client { get; private set; }

        public Sale() { }

        public Sale(int carId, int clientId, DateTime saleDate, decimal finalPrice, DateTime purchaseDate, string paymentMethod)
        {
            ValidateSaleData(carId, clientId, saleDate, finalPrice, purchaseDate, paymentMethod);
            CarId = carId;
            ClientId = clientId;
            SaleDate = saleDate;
            FinalPrice = finalPrice;
            PurchaseDate = purchaseDate;
            PaymentMethod = paymentMethod;
        }

        public Sale(int saleId, int carId, int clientId, DateTime saleDate, decimal finalPrice, DateTime purchaseDate, string paymentMethod)
            : this(carId, clientId, saleDate, finalPrice, purchaseDate, paymentMethod)
        {
            if (saleId <= 0)
            {
                throw new ArgumentException("ID inválido.");
            }

            SaleId = saleId;
        }       

        public void UpdateSale(int newCarId, int newClientId, DateTime newSaleDate, decimal newFinalPrice, DateTime newPurchaseDate, string newPaymentMethod)
        {
            ValidateSaleData(newCarId, newClientId, newSaleDate, newFinalPrice, newPurchaseDate, newPaymentMethod);

            CarId = newCarId;
            ClientId = newClientId;
            SaleDate = newSaleDate;
            FinalPrice = newFinalPrice;
            PurchaseDate = newPurchaseDate;
            PaymentMethod = newPaymentMethod;
        }

        private static void ValidateSaleData(int carId, int clientId, DateTime saleDate, decimal finalPrice, DateTime purchaseDate, string paymentMethod)
        {
            if (carId <= 0)
            {
                throw new ArgumentException("O CarId deve ser positivo.", nameof(carId));
            }
            if (clientId <= 0)
            {
                throw new ArgumentException("O ClientId deve ser positivo.", nameof(clientId));
            }
            if (saleDate == default)
            {
                throw new ArgumentException("A SaleDate é obrigatória.", nameof(saleDate));
            }
            if (finalPrice <= 0)
            {
                throw new ArgumentException("O FinalPrice deve ser positivo.", nameof(finalPrice));
            }
            if (purchaseDate == default)
            {
                throw new ArgumentException("A PurchaseDate é obrigatória.", nameof(purchaseDate));
            }
            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                throw new ArgumentException("O PaymentMethod é obrigatório.", nameof(paymentMethod));
            }
        }

        public int GetId() => SaleId;

        public void SetId(int id)
        {
            if (SaleId != 0)
            {
                throw new InvalidOperationException("Não é permitido alterar o ID de uma Entidade já persistida.");
            }
            if (id <= 0)
            {
                throw new ArgumentException("O ID deve ser positivo.", nameof(id));
            }
            SaleId = id;
        }

        public bool GetIsActive() => true;
    }
}
