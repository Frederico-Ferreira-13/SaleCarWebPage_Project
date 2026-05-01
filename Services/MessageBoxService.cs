using Contracts.Repositories;
using Contracts.Services;
using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class MessageBoxService : IMessageBoxService
    {
        public readonly IUnitOfWork _unitOfWork;

        public MessageBoxService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<MessageBox>> GetMessageDetailAsync(int messageId, int currentUserId)
        {
            var message = await _unitOfWork.MessageBox.GetByIdAsync(messageId);

            if (message == null || !message.IsActive)
            {
                return Result<MessageBox>.Failure(
                    Error.NotFound(
                        ErrorCodes.NotFound, "Mensagem não encontrada."));
            }

            if (message.SenderId != currentUserId && message.ReceiverId != currentUserId)
            {
                return Result<MessageBox>.Failure(
                    Error.Unauthorized(
                        ErrorCodes.AuthUnauthorized, "Acesso negado à mensagem."));  
            }

            return Result<MessageBox>.Success(message);
        }

        public async Task<Result<IEnumerable<MessageBox>>> GetUserInboxAsync(int userId)
        {
            var messages = await _unitOfWork.MessageBox.GetByUserIdAsync(userId);
            return Result<IEnumerable<MessageBox>>.Success(messages.Where(m => m.IsActive));
        }

        public async Task<Result<IEnumerable<MessageBox>>> GetChatHistoryAsync(int carId, int buyerId, int sellerId)
        {
            var history = await _unitOfWork.MessageBox.GetChatHistoryAsync(carId, buyerId, sellerId);
            return Result<IEnumerable<MessageBox>>.Success(history);
        }

        public async Task<Result<MessageBox>> SendInquiryAsync(int carId, int senderId, string subject, string messageText)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(carId);
            if (car == null)
            {
                return Result<MessageBox>.Failure(
                    Error.NotFound(
                        ErrorCodes.NotFound, "O carro em questão já não está disponível."));
            }

            int receiverId = car.ProviderId;

            if (senderId == receiverId)
            {
                return Result<MessageBox>.Failure(
                    Error.Validation("Não pode enviar uma mensagem para si mesmo."));
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var newMessage = new MessageBox(senderId, receiverId, carId, subject, messageText);

                await _unitOfWork.MessageBox.AddAsync(newMessage);
                await _unitOfWork.CommitAsync();

                return Result<MessageBox>.Success(newMessage);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<MessageBox>.Failure(Error.InternalServer($"Erro ao enviar mensagem: {ex.Message}"));
            }
        }

        public async Task<Result> MarkAsReadAsync(int messageId, int currentUserId)
        {
            var message = await _unitOfWork.MessageBox.GetByIdAsync(messageId);
            if (message == null)
            {
                return Result.Failure(Error.NotFound(
                    ErrorCodes.NotFound, "Mensagem não encontrada."));
            }

            if (message.ReceiverId != currentUserId)
            {
                return Result.Success();
            }

            return Result.Success();
        }

        public async Task<Result> ArchiveMessageAsync(int messageId, int currentUserId)
        {
            var message = await _unitOfWork.MessageBox.GetByIdAsync(messageId);
            if (message == null || (message.SenderId != currentUserId && message.ReceiverId != currentUserId))
            {
                return Result.Failure(
                    Error.NotFound(
                        ErrorCodes.NotFound, "Mensagem não encontrada."));
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                message.Deactivate();

                await _unitOfWork.MessageBox.UpdateAsync(message);
                await _unitOfWork.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result.Failure(Error.InternalServer(ex.Message));
            }
        }

        public async Task<Result<int>> GetUnreadCountAsync(int userId)
        {
            int count = await _unitOfWork.MessageBox.GetUnreadCountAsync(userId);
            return Result<int>.Success(count);
        }

        public async Task<Result<IEnumerable<MessageBox>>> GetChatHistoryAsync(int carId, int currentUserId)
        {
            var car = await _unitOfWork.Cars.GetByIdAsync(carId);
            if (car == null) 
            {
                return Result<IEnumerable<MessageBox>>.Failure(Error.NotFound("Carro não encontrado"));
            } 

            // É o vendedor ou admin?
            bool isSeller = (car.ProviderId == currentUserId);

            if (isSeller)
            {
                // Vendedor vê TUDO (Principais + Respostas)
                var allMessages = await _unitOfWork.MessageBox.GetAllAsync();
                return Result<IEnumerable<MessageBox>>.Success(allMessages.Where(m => m.CarId == carId));
            }
            else
            {
                // Comprador só vê a sua conversa (onde ele é sender ou receiver)
                var myHistory = await _unitOfWork.MessageBox.GetChatHistoryAsync(carId, currentUserId, car.ProviderId);
                return Result<IEnumerable<MessageBox>>.Success(myHistory);
            }
        }

        public async Task<Result<MessageBox>> SendMessageAsync(int carId, int senderId, int receiverId, string subject,
            string messageText, int? parentMessageId = null)
        {
            if (senderId == receiverId)
                return Result<MessageBox>.Failure(
                    Error.Validation("Não pode enviar uma mensagem para si mesmo."));

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var newMessage = new MessageBox(
                    senderId, receiverId, carId, subject, messageText);

                // Ligar à thread pai se for uma resposta
                if (parentMessageId.HasValue)
                    newMessage.SetParent(parentMessageId.Value);

                await _unitOfWork.MessageBox.AddAsync(newMessage);
                await _unitOfWork.CommitAsync();

                return Result<MessageBox>.Success(newMessage);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<MessageBox>.Failure(
                    Error.InternalServer($"Erro ao enviar mensagem: {ex.Message}"));
            }
        }

        public async Task<Result> EditMessageAsync(int messageId, int currentUserId, string newText)
        {
            var message = await _unitOfWork.MessageBox.GetByIdAsync(messageId);

            if (message == null || !message.IsActive)
                return Result.Failure(Error.NotFound(ErrorCodes.NotFound, "Mensagem não encontrada."));

            if (message.SenderId != currentUserId)
                return Result.Failure(Error.Unauthorized(ErrorCodes.AuthUnauthorized, "Não pode editar mensagens de outro utilizador."));

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                message.UpdateMessage(message.Subject, newText); // usa o método já existente na entidade
                await _unitOfWork.MessageBox.UpdateAsync(message);
                await _unitOfWork.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result.Failure(Error.InternalServer($"Erro ao editar mensagem: {ex.Message}"));
            }
        }
    }
}
