using Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class Provider : IEntity
    {
        public int ProviderId { get; private set; }
        public int UserId { get; private set; }
        public int AddressId { get; private set; }
        public string NameProvider { get; private set; } = string.Empty;        
        public string NIF { get; private set; } = string.Empty;

        public virtual Users? User { get; private set; }
        public virtual Address? Address { get; private set; }

        public Provider() { }

        public Provider(string nameProvider, string nif, int userId, int addressId)
        {
            ValidateProviderData(nameProvider, nif, userId, addressId);
            NameProvider = nameProvider;
            NIF = nif;
            UserId = userId;
            AddressId = addressId;
        }

        public Provider(int providerId, string nameProvider, string nif, int userId, int addressId)
            : this(nameProvider, nif, userId, addressId)
        {
            if (providerId <= 0)
            {
                throw new ArgumentException("ID inválido.");
            }

            ProviderId = providerId;
        }

        public void UpdateProvider(string newNameProvider, string newNif, int newUserId, int newAddressId)
        {
            ValidateProviderData(newNameProvider, newNif, newUserId, newAddressId);

            NameProvider = newNameProvider; 
            NIF = newNif;
            UserId = newUserId;
            AddressId = newAddressId;
        }

        private static void ValidateProviderData(string nameProvider, string nif, int userId, int addressId)
        {
            if (string.IsNullOrWhiteSpace(nameProvider))
            {
                throw new ArgumentException("O nome do fornecedor é obrigatório.", nameof(nameProvider));
            }
            if (string.IsNullOrWhiteSpace(nif))
            {
                throw new ArgumentException("O NIF é obrigatório.", nameof(nif));
            }
            if (userId <= 0)
            {
                throw new ArgumentException("O UserId deve ser positivo.", nameof(userId));
            }
            if (addressId <= 0)
            {
                throw new ArgumentException("O AddressId deve ser positivo.", nameof(addressId));
            }
        }

        public int GetId() => ProviderId;

        public void SetId(int id)
        {
            if (ProviderId != 0)
            {
                throw new InvalidOperationException("Não é permitido alterar o ID de uma Entidade já persistida.");
            }
            if (id <= 0)
            {
                throw new ArgumentException("O ID deve ser positivo.", nameof(id));
            }
            ProviderId = id;
        }

        public bool GetIsActive() => true;
    }
}
