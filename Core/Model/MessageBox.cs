using Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class MessageBox : IEntity
    {
        public int MessageBoxId { get; private set; }
        public int SenderId { get; private set; }
        public int ReceiverId { get; private set; }
        public int CarId { get; private set; }
        public string Subject { get; private set; } = string.Empty;
        public string MessageText { get; private set; } = string.Empty;
        public DateTime SentDate { get; private set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt { get; protected set; }
        public bool IsEdited { get; protected set; } = false;
        public bool IsDeleted { get; private set; } = false;
        public bool IsRead { get; private set; } = true;

        private MessageBox() { }

        public MessageBox(int senderId, int receiverId, int carId, string subject, string messageText)
        {
            if (senderId <= 0)
            {
                throw new ArgumentException("O SenderId deve ser um ID válido (maior que 0).", nameof(senderId));
            }
            if (receiverId <= 0)
            {
                throw new ArgumentException("O ReceiverId deve ser um ID válido (maior que 0).", nameof(receiverId));
            }
            if (carId <= 0)
            {
                throw new ArgumentException("O CarId deve ser um ID válido (maior que 0).", nameof(carId));
            }
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("O Subject é obrigatório.", nameof(subject));
            }
            if (string.IsNullOrWhiteSpace(messageText))
            {
                throw new ArgumentException("O MessageText é obrigatório.", nameof(messageText));
            }
            SenderId = senderId;
            ReceiverId = receiverId;
            CarId = carId;
            Subject = subject;
            MessageText = messageText;
            SentDate = DateTime.UtcNow;
        }

        public MessageBox(int messageBoxId, int senderId, int receiverId, int carId, string subject, string messageText, DateTime sentDate, bool isRead)
            : this(senderId, receiverId, carId, subject, messageText)
        {
            if (messageBoxId < 0)
            {
                throw new ArgumentException("O ID não pode ser negativo.", nameof(messageBoxId));
            }
            MessageBoxId = messageBoxId;
            SentDate = sentDate;
            IsRead = isRead;
        }

        public void UpdateMessage(string newSubject, string newMessageText)
        {
            if (string.IsNullOrWhiteSpace(newSubject))
            {
                throw new ArgumentException("O Subject é obrigatório.", nameof(newSubject));
            }
            if (string.IsNullOrWhiteSpace(newMessageText))
            {
                throw new ArgumentException("O MessageText é obrigatório.", nameof(newMessageText));
            }

            if(newMessageText.Length > 500)
            {
                throw new ArgumentException("O MessageText não pode exceder 500 caracteres.", nameof(newMessageText));
            }

            if((DateTime.UtcNow - SentDate).TotalDays > 30)
            {
                throw new InvalidOperationException("Não é possível editar mensagens com mais de 30 dias.");
            }

            Subject = newSubject;
            MessageText = newMessageText;
        }

        public void Delete()
        {
            if (!IsDeleted)
            {
                IsDeleted = true;
                LastUpdatedAt = DateTime.UtcNow;
                MessageText = "[Mensagem Excluída]";
            }
        }

        public int GetId() => MessageBoxId;

        public void SetId(int id)
        {
            if (MessageBoxId != 0)
            {
                throw new InvalidOperationException("Não é permitido alterar o ID de uma Entidade que já possui um ID.");
            }
            MessageBoxId = id;
        }

        public bool GetIsActive() => !IsDeleted;
    }
}
