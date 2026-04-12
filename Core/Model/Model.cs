using Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class Model : IEntity
    {
        public int ModelId { get; private set; }
        public int BrandId { get; private set; }
        public string ModelName { get; private set; } = string.Empty;

        public Model() { }

        public Model(string modelName)
        {
            ValidateModelName(modelName);
            ModelName = modelName;
        }

        public Model(int id, string modelName)
        {
            BrandId = id;
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
            {
                throw new ArgumentException("O nome do Modelo é obrigatório.", nameof(modelName));
            }
            if (modelName.Length > 100)
            {
                throw new ArgumentException("O nome do Modelo não pode exceder 100 caracteres.", nameof(modelName));
            }
        }

        public int GetId() => ModelId;

        public void SetId(int id)
        {
            if (ModelId != 0)
            {
                throw new InvalidOperationException("Não é permitido alterar o ID de uma Entidade já persistida.");
            }
            if (id <= 0)
            {
                throw new ArgumentException("O ID deve ser positivo.", nameof(id));
            }
            ModelId = id;
        }

        public bool GetIsActive() => true;
    }
}
