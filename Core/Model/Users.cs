using Core.Common;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Core.Model
{
    public class Users : IEntity, ISoftDeletable
    {
        public int UserId { get; private set; }
        public int ContactId { get; protected set; }
        public int UsersRoleId { get; private set; }
        public bool IsActive { get; private set; } = true;

        private string _name = string.Empty;
        private string _userName = string.Empty;
        private string _email = string.Empty;
        private string? _profilePicture;
        private string _passwordHash = string.Empty;
        private string _salt = string.Empty;

        public bool IsApproved { get; private set; } = false;
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastUpdatedAt { get; private set; }

        public Contact? Contact { get; protected set; }
        public UsersRole? Role { get; private set; }

       
        public string Name { get => _name; private set => _name = value; }
        public string UserName { get => _userName; private set => _userName = value; }
        public string Email { get => _email; private set => _email = value; }
        public string PasswordHash { get => _passwordHash; private set => _passwordHash = value; }
        public string Salt { get => _salt; private set => _salt = value; }
        public string? ProfilePicture { get => _profilePicture; private set => _profilePicture = value; }

        private Users()
        {
            // Futuramente para EF core ou para Reconstituição via reflection
        }

        public Users(string name, string userName, string email, int usersRoleId, bool isApproved, int contactId)
        {
            ValidateName(name, nameof(name));
            ValidateUserName(userName, nameof(userName));
            ValidateEmail(email);

            if (contactId <= 0)
            {
                throw new ArgumentException("O ContactId deve ser positivo", nameof(contactId));
            }
            if (usersRoleId <= 0)
            {
                throw new ArgumentException("O UsersRoleId deve ser positivo", nameof(usersRoleId));
            }

            _name = name;
            _userName = userName;
            _email = email;
            UsersRoleId = usersRoleId;
            IsApproved = isApproved;
            ContactId = contactId;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public Users(int id, string name, string userName, string email, string? profilePicture, string passwordHash, string salt, bool isApproved, int usersRoleId, int contactId,
                    DateTime createdAt, DateTime? lastUpdatedAt, bool isActive)
        {
            UserId = id;
            IsActive = isActive;
            _name = name;
            _userName = userName;
            _email = email;
            _profilePicture = profilePicture;
            _passwordHash = passwordHash;
            _salt = salt;
            IsApproved = isApproved;
            UsersRoleId = usersRoleId;
            ContactId = contactId;
            CreatedAt = createdAt;
            LastUpdatedAt = lastUpdatedAt;
        }

        public void UpdateName(string newName)
        {
            CheckIfActive();
            ValidateName(newName, nameof(newName));

            if (_name != newName)
            {
                _name = newName;
                SetLastUpdatedAt();
            }
        }

        public void UpdateUserName(string newUserName)
        {
            CheckIfActive();
            ValidateUserName(newUserName, nameof(newUserName));

            if (!string.Equals(UserName, newUserName, StringComparison.OrdinalIgnoreCase))
            {
                UserName = newUserName;
                SetLastUpdatedAt();
            }
        }

        public void UpdateEmail(string newEmail)
        {
            CheckIfActive();
            ValidateEmail(newEmail);

            if (!string.Equals(Email, newEmail, StringComparison.OrdinalIgnoreCase))
            {
                Email = newEmail;
                SetLastUpdatedAt();
            }
        }

        public void SetPassword(string newPasswordHash, string newSalt)
        {
            CheckIfActive();
            if (string.IsNullOrWhiteSpace(newPasswordHash) || string.IsNullOrWhiteSpace(newSalt))
            {
                throw new ArgumentException("O hash da password e o salt são obrigatórios.");
            }

            _passwordHash = newPasswordHash;
            _salt = newSalt;
            SetLastUpdatedAt();
        }

        public void UpdateProfilePicture(string? fileName)
        {
            CheckIfActive();
            if (_profilePicture != fileName)
            {
                _profilePicture = fileName;
                SetLastUpdatedAt();
            }
        }

        public void ChangeRole(int newUsersRoleId)
        {
            CheckIfActive();
            if (newUsersRoleId <= 0)
            {
                throw new ArgumentException("O novo UsersRoleId deve ser positivo.", nameof(newUsersRoleId));
            }

            if (UsersRoleId != newUsersRoleId)
            {
                UsersRoleId = newUsersRoleId;
                SetLastUpdatedAt();
            }
        }

        public void ChangeContact(int newContactId)
        {
            CheckIfActive();
            if (newContactId <= 0)
            {
                throw new ArgumentException("O novo ContactId deve ser positivo.", nameof(newContactId));
            }

            if (ContactId != newContactId)
            {
                ContactId = newContactId;
                Contact = null;
                SetLastUpdatedAt();
            }
        }

        public void SetContact(Contact contact)
        {
            if (ContactId != contact.ContactId)
                throw new InvalidOperationException("Erro de mapeamento: IDs não coincidem.");

            Contact = contact;
        }

        public void Deactivate()
        {
            if (this.IsActive)
            {
                this.IsActive = false;
                SetLastUpdatedAt();
            }
        }

        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                SetLastUpdatedAt();
            }
        }

        public void Approve()
        {
            CheckIfActive();
            if (!IsApproved)
            {
                IsApproved = true;
                SetLastUpdatedAt();
            }           
        }

        private void CheckIfActive()
        {
            if (!IsActive) 
            {
                throw new InvalidOperationException("Operação inválida para utilizador inativo.");
            }                
        }       

        private void ValidateName(string name, string paramName)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"O nome de utilizador não pode ser vazio ou nulo ({paramName}).");
            }
            if (name.Length > 100)
            {
                throw new ArgumentException("O nome de utilizador não pode exceder 100 caracteres.", paramName);
            }
        }

        private void ValidateUserName(string userName, string paramName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException($"O nome de utilizador não pode ser vazio ou nulo ({paramName}).");

            if (userName.Length > 100)
                throw new ArgumentException("O nome de utilizador não pode exceder 100 caracteres.", paramName);
        }

        private void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("O email é obrigatório.", nameof(email));
            }
            if (email.Length > 255)
            {
                throw new ArgumentException("O email não pode exceder 255 caracteres.", nameof(email));
            }
            if (!IsValidEmailFormat(email))
            {
                throw new ArgumentException("Formato de email inválido.", nameof(email));
            }
        }

        private bool IsValidEmailFormat(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        private void SetLastUpdatedAt()
        {
            LastUpdatedAt = DateTime.UtcNow;
        }

        public int GetId() => UserId;

        public void SetId(int id)
        {
            if (UserId != 0)
            {
                throw new InvalidOperationException("Não é permitido alterar o ID de uma Entidade que já possui um ID.");
            }
            UserId = id;
        }

        public bool GetIsActive() => IsActive;
    }
}
