using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Model
{
    public class Brand : IEntity
    {
        [Key]
        public int BrandId { get; private set; }
        public string BrandName { get; private set; } = string.Empty;

        public Brand() { }

        public Brand(string brandName)
        {
            ValidateBrandName(brandName);
            BrandName = brandName;
        }

        public Brand(int id, string brandName)
        {            
            BrandId = id;
            BrandName = brandName;
        }

        public void UpdateBrandName(string newBrandName)
        {
            ValidateBrandName(newBrandName);
            if (!BrandName.Equals(newBrandName, StringComparison.Ordinal))
            {
                BrandName = newBrandName;
            }
        }

        public void ValidateBrandName(string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
            {
                throw new ArgumentException("O nome da Marca é obrigatório.", nameof(brandName));
            }
            if (brandName.Length > 100)
            {
                throw new ArgumentException("O nome da Marca não pode exceder 100 caracteres.", nameof(brandName));
            }
        }

        public int GetId() => BrandId;

        public void SetId(int id)
        {
            if (BrandId != 0)
            {
                throw new InvalidOperationException("Não é permitido alterar o ID de uma Entidade já persistida.");
            }
            if (id <= 0)
            {
                throw new ArgumentException("O ID deve ser positivo.", nameof(id));
            }
            BrandId = id;
        }

        public bool GetIsActive() => true;
    }
}
