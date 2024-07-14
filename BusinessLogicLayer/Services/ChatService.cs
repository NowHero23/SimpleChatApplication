using BusinessLogicLayer.Common;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data.Domain.Repositories.Abstract;
using DataAccessLayer.Etities;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public Result<Chat> Create(Chat chat)
        {
            if (string.IsNullOrEmpty(chat.Name))
                return Result<Chat>.ErrorResult("Chat name cannot be empty.");

            if (string.IsNullOrEmpty(chat.AuthorName))
                return Result<Chat>.ErrorResult("Author name cannot be empty.");

            if (_chatRepository.FindByCondition(c => c.Name == chat.Name).Any())
                return Result<Chat>.ErrorResult("A chat with the same name already exists.");

            try
            {
                _chatRepository.Create(chat);
                _chatRepository.SaveChanges();
                return Result<Chat>.SuccessResult(chat);
            }
            catch (Exception ex)
            {
                return Result<Chat>.ErrorResult($"An error occurred while creating the chat: {ex.Message}");
            }
        }

        public IQueryable<Chat> FindAll() => _chatRepository.FindAll();

        public IQueryable<Chat> FindByCondition(Expression<Func<Chat, bool>> expression) => _chatRepository.FindByCondition(expression);

        public Result<bool> Remove(Chat chat)
        {
            if (chat is null)
                return Result<bool>.ErrorResult("The chat does not exist.");

            if (string.IsNullOrEmpty(chat.Name))
                return Result<bool>.ErrorResult("Chat name cannot be empty.");

            if (!_chatRepository.FindByCondition(c => c.Name == chat.Name).Any())
                return Result<bool>.ErrorResult("The chat does not exist.");

            try
            {
                _chatRepository.Remove(chat);
                _chatRepository.SaveChanges();
                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.ErrorResult($"An error occurred while deleting the chat: {ex.Message}");
            }
        }

        public Result<Chat> Update(Chat chat)
        {
            if (string.IsNullOrEmpty(chat.Name))
                return Result<Chat>.ErrorResult("Chat name cannot be empty.");

            if (string.IsNullOrEmpty(chat.AuthorName))
                return Result<Chat>.ErrorResult("Author name cannot be empty.");

            if (!_chatRepository.FindByCondition(c => c.Id == chat.Id).Any())
                return Result<Chat>.ErrorResult("The chat does not exist.");

            if (_chatRepository.FindByCondition(c => c.Name == chat.Name).Any())
                return Result<Chat>.ErrorResult("A chat with the same name already exists.");

            try
            {
                _chatRepository.Update(chat);
                _chatRepository.SaveChanges();
                return Result<Chat>.SuccessResult(chat);
            }
            catch (Exception ex)
            {
                return Result<Chat>.ErrorResult($"An error occurred while updating the chat: {ex.Message}");
            }
        }
    }
}
