using Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class Contact : IEntity
    {
        public int ContactId { get; private set; }
        public int ClientId { get; set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public string JobTitle { get; private set; } = string.Empty;

        public virtual Client? Client { get; private set; }

        public Contact() { }

        public Contact(string firstName, string lastName, string email, string phoneNumber, string jobTitle)
        {
            ValidateContactData(firstName, lastName, email, phoneNumber, jobTitle);
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            JobTitle = jobTitle;
        }

        public Contact(int contactId, string firstName, string lastName, string email, string phoneNumber, string jobTitle)
            : this(firstName, lastName, email, phoneNumber, jobTitle)
        {
            if (contactId <= 0)
            {
                throw new ArgumentException("ID inválido.");
            }
            ContactId = contactId;
        }

        public void UpdateContact(string newFirstName, string newLastName, string newEmail, string newPhoneNumber, string newJobTitle)
        {
            ValidateContactData(newFirstName, newLastName, newEmail, newPhoneNumber, newJobTitle);
            FirstName = newFirstName;
            LastName = newLastName;
            Email = newEmail;
            PhoneNumber = newPhoneNumber;
            JobTitle = newJobTitle;
        }

        public void ValidateContactData(string firstName, string lastName, string email, string phoneNumber, string jobTitle)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("O nome é obrigatório.", nameof(firstName));
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("O sobrenome é obrigatório.", nameof(lastName));
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("O email é obrigatório.", nameof(email));
            }
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new ArgumentException("O número de telefone é obrigatório.", nameof(phoneNumber));
            }
            if (string.IsNullOrWhiteSpace(jobTitle))
            {
                throw new ArgumentException("O cargo é obrigatório.", nameof(jobTitle));
            }
        }

        public void SetClient(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client), "O cliente não pode ser nulo.");
            }
            Client = client;
        }

        public int GetId() => ContactId;

        public void SetId(int id)
        {
            if (ContactId != 0)
            {
                throw new InvalidOperationException("ID já definido.");
            }
            ContactId = id;
        }

        public bool GetIsActive() => true;
    }
}
