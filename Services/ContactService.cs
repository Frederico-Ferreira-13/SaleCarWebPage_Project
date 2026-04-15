using Contracts.Services;
using Contracts.Repositories;
using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<Contact>> GetByIdAsync(int contactId)
        {
            if (contactId <= 0)
            {
                return Result<Contact>.Failure(
                    Error.Validation("ID do Contacto é inválido."));
            }

            var contact = await _unitOfWork.Contacts.GetByIdAsync(contactId);
            if (contact == null)
            {
                return Result<Contact>.Failure(
                    Error.NotFound(ErrorCodes.NotFound, $"Contacto com ID {contactId} não encontrado.")
                );
            }

            return Result<Contact>.Success(contact);
        }

        public async Task<Result<IEnumerable<Contact>>> GetAllContactAsync()
        {
            var contacts = await _unitOfWork.Contacts.GetAllAsync();
            return Result<IEnumerable<Contact>>.Success(contacts);
        }

        public async Task<Result<Contact>> CreateAsync(Contact newContact)
        {
            var existingContact = await _unitOfWork.Contacts.GetByIdAsync(newContact.ContactId);
            if (existingContact != null)
            {
                return Result<Contact>.Failure(Error.Conflict(
                    ErrorCodes.AlreadyExists, "Já existe um contacto registado com este ID."));
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var contactToSave = new Contact(
                     newContact.FirstName,
                     newContact.LastName,
                     newContact.Email,
                     newContact.PhoneNumber,
                     newContact.JobTitle
                 );

                await _unitOfWork.Contacts.AddAsync(contactToSave);
                await _unitOfWork.CommitAsync();

                return Result<Contact>.Success(contactToSave);
            }
            catch (ArgumentException ex)
            {
                _unitOfWork.Rollback();
                return Result<Contact>.Failure(Error.Validation(ex.Message));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<Contact>.Failure(Error.InternalServer($"Erro ao criar Contacto: {ex.Message}"));
            }
        }

        public async Task<Result<Contact>> UpdateAsync(Contact updateContact)
        {
            var existingContact = await _unitOfWork.Contacts.GetByIdAsync(updateContact.ContactId);
            if (existingContact == null)
                return Result<Contact>.Failure(Error.NotFound(ErrorCodes.NotFound, "Cliente não encontrado para atualização."));

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Usar o método de atualização interno da Entidade Contact
                existingContact.UpdateContact(
                    updateContact.FirstName,
                    updateContact.LastName,
                    updateContact.Email,
                    updateContact.PhoneNumber,
                    updateContact.JobTitle
                );

                await _unitOfWork.Contacts.UpdateAsync(existingContact);
                await _unitOfWork.CommitAsync();

                return Result<Contact>.Success(existingContact);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<Contact>.Failure(Error.InternalServer($"Erro ao atualizar cliente: {ex.Message}"));
            }
        }

        public async Task<Result> DeleteAsync(int contactId)
        {
            var contact = await _unitOfWork.Contacts.GetByIdAsync(contactId);
            if (contact == null)
                return Result.Failure(Error.NotFound(ErrorCodes.NotFound, "Cliente não encontrado."));

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.Contacts.DeleteAsync(contactId);
                await _unitOfWork.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result.Failure(Error.InternalServer($"Erro ao remover cliente: {ex.Message}"));
            }
        }

        public async Task<Result<List<Contact>>> GetInteractionsAsync(Guid contactId)
        {
            return Result<List<Contact>>.Success(new List<Contact>());
        }

        public async Task<Result<Contact>> GetByEmailOrPhoneAsync(string identifier)
        {
            var all = await _unitOfWork.Contacts.GetAllAsync();
            var contact = all.FirstOrDefault(c => c.Email == identifier || c.PhoneNumber == identifier);

            if (contact == null)
                return Result<Contact>.Failure(Error.NotFound(ErrorCodes.NotFound, "Nenhum contacto encontrado com esses dados."));

            return Result<Contact>.Success(contact);
        }

        public async Task<Result<Contact>> CreateInteractionAsync(Contact dto) => Result<Contact>.Success(dto);
        public async Task<Result> UpdateInteractionAsync(Contact dto) => Result.Success();
    }
}
