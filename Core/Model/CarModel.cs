using Core.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Model
{
    [Table("Model")]
    public class CarModel : IEntity
    {
        [Key]
        public int Id { get; set; } 

        public int BrandId { get; private set; }
        public string ModelName { get; private set; } = string.Empty;
        public virtual Brand Brand { get; private set; }

        public CarModel() { }

        public CarModel(string modelName)
        {
            ValidateModelName(modelName);
            ModelName = modelName;
        }

        public CarModel(int brandId, string modelName)
        {
            BrandId = brandId;
            ModelName = modelName;
        }

        public void UpdateModelName(string newModelName)
        {
            ValidateModelName(newModelName);
            if (!ModelName.Equals(newModelName, StringComparison.Ordinal))
            {
                ModelName = newModelName;
            }
        }

        public void ValidateModelName(string modelName)
        {
            if (string.IsNullOrWhiteSpace(modelName))
                throw new ArgumentException("O nome do Modelo é obrigatório.", nameof(modelName));
        }

        // Métodos obrigatórios da IEntity
        public int GetId() => Id;
        public void SetId(int id) => Id = id;
        public bool GetIsActive() => true;
    }
}