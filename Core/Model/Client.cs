using Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class Client : IEntity, ISoftDeletable
    {
        public int ClientId { get; private set; }
        public int UserId { get; private set; }
        public int ContactId { get; private set; }
        public int AddressId { get; private set; }
        public string ClientName { get; private set; } = string.Empty;
        public string NIF { get; private set; } = string.Empty;

        public bool IsActive { get; private set; } = true;

        public virtual Address? Address { get; private set; }
        public virtual Contact? Contact { get; private set; }
        public virtual Users? User { get; private set; }

        public Client() { }

        public Client(string clientName, string nif, int contactId, int userId, int addressId)
        {
            ValidateClientData(clientName, nif, userId, contactId, addressId);
            ClientName = clientName;
            NIF = nif;
            UserId = userId;
            ContactId = contactId;
            AddressId = addressId;
            IsActive = true;
        }

        public Client(int clientId, int userId, string clientName, string nif, int contactId, int addressId)
        : this(clientName, nif, userId, contactId, addressId)
        {
            if (clientId <= 0) 
            {
                throw new ArgumentException("ID inválido.");
            }
            if (userId <= 0)
            {
                throw new ArgumentException("O UserId é obrigatório.");
            }

            ClientId = clientId;
        }

        public void UpdateClient(string newClientName, string newNif, int newUserId, int newContactId, int newAddressId)
        {
            ValidateClientData(newClientName, newNif, newUserId, newContactId, newAddressId);
            ClientName = newClientName;
            NIF = newNif;
            UserId = newUserId;
            ContactId = newContactId;
            AddressId = newAddressId;
        }

        public void Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
            }
        }

        private static void ValidateClientData(string clientName, string nif, int userId, int contactId, int addressId)
        {
            if (string.IsNullOrWhiteSpace(clientName))
            {
                throw new ArgumentException("O nome do cliente é obrigatório.", nameof(clientName));
            }
            if (string.IsNullOrWhiteSpace(nif))
            {
                throw new ArgumentException("O NIF é obrigatório.", nameof(nif));
            }
            if (userId <= 0)
            {
                throw new ArgumentException("O UserId é obrigatório.", nameof(userId));
            }
            if (contactId <= 0)
            {
                throw new ArgumentException("O ContactId deve ser positivo.", nameof(contactId));
            }
            if (addressId <= 0)
            {
                throw new ArgumentException("O AddressId deve ser positivo.", nameof(addressId));
            }
        }
        

        public int GetId() => ClientId;

        public void SetId(int id)
        {
            if (ClientId != 0)
            {
                throw new InvalidOperationException("Não é permitido alterar o ID de uma Entidade já persistida.");
            }
            if (id <= 0)
            {
                throw new ArgumentException("O ID deve ser positivo.", nameof(id));
            }
            ClientId = id;
        }

        public bool GetIsActive() => true;
    }
}
