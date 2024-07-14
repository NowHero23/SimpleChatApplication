using BusinessLogicLayer.Common;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Data.Domain.Repositories.Abstract;
using DataAccessLayer.Etities;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services
{
    public class UserConnectionService : IUserConnectionService
    {
        private readonly IUserConnectionRepository _connectionRepository;
        private readonly IChatRepository _chatRepository;
        public UserConnectionService(IUserConnectionRepository connectionRepository, IChatRepository chatRepository)
        {
            _connectionRepository = connectionRepository;
            _chatRepository = chatRepository;
        }

        public Result<UserConnection> Create(UserConnection connection)
        {
            if (string.IsNullOrEmpty(connection.UserName))
                return Result<UserConnection>.ErrorResult("UserName cannot be empty.");

            if (string.IsNullOrEmpty(connection.Chat.Name))
                return Result<UserConnection>.ErrorResult("Chat name cannot be empty.");

            var chatExist = _chatRepository.FindByCondition(c => c.Name == connection.Chat.Name).Any();
            if (!chatExist)
                return Result<UserConnection>.ErrorResult("Chat is not exist.");

            var existingConnection = _connectionRepository.FindByCondition(c => c.UserName == connection.UserName).FirstOrDefault();
            /* if (existing)
                 _connectionRepository.Remove(_connectionRepository.FindByCondition(c => c.UserName == connection.UserName).First());*/

            if (existingConnection != null && existingConnection.Chat.Name == connection.Chat.Name && existingConnection.UserName == connection.UserName)
            {
                return Result<UserConnection>.ErrorResult("The connection is already created.");
            }

            try
            {
                if (existingConnection != null && existingConnection?.Id != 0)
                    _connectionRepository.Update(connection);
                else
                    _connectionRepository.Create(connection);

                _connectionRepository.SaveChanges();
                return Result<UserConnection>.SuccessResult(connection);
            }
            catch (Exception ex)
            {
                return Result<UserConnection>.ErrorResult($"An error occurred while creating the connection: {ex.Message}");
            }
        }

        public IQueryable<UserConnection> FindAll() => _connectionRepository.FindAll();

        public IQueryable<UserConnection> FindByCondition(Expression<Func<UserConnection, bool>> expression) => _connectionRepository.FindByCondition(expression);

        public Result<bool> Remove(UserConnection connection)
        {
            if (string.IsNullOrEmpty(connection.UserName))
                return Result<bool>.ErrorResult("UserName cannot be empty.");

            if (string.IsNullOrEmpty(connection.Chat.Name))
                return Result<bool>.ErrorResult("Chat name cannot be empty.");

            var existing = _connectionRepository.FindByCondition(c => c.UserName == connection.UserName && connection.Chat.Name == connection.Chat.Name).Any();
            if (!existing)
                return Result<bool>.ErrorResult("The connection does not exist.");

            try
            {
                var entity = _connectionRepository.FindByCondition((c) => connection.UserName == c.UserName).First();
                _connectionRepository.Remove(entity);
                _connectionRepository.SaveChanges();
                return Result<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.ErrorResult($"An error occurred while deleing the connection: {ex.Message}");
            }

        }

        public Result<UserConnection> Update(UserConnection newConnection)
        {
            if (string.IsNullOrEmpty(newConnection.UserName))
                return Result<UserConnection>.ErrorResult("UserName cannot be empty.");

            if (string.IsNullOrEmpty(newConnection.Chat.Name))
                return Result<UserConnection>.ErrorResult("Chat name cannot be empty.");


            var existingConnection = _connectionRepository.FindByCondition(c => c.UserName == newConnection.UserName).FirstOrDefault();
            if (existingConnection != null && existingConnection.Chat.Name == newConnection.Chat.Name && existingConnection.UserName == newConnection.UserName)
            {
                return Result<UserConnection>.ErrorResult("The connection is already created.");
            }

            try
            {
                var connection = _connectionRepository.FindByCondition((c) => newConnection.UserName == c.UserName).First();
                connection.Chat.Name = newConnection.Chat.Name;
                connection.ConnectingId = newConnection.ConnectingId;

                _connectionRepository.Update(connection);
                _connectionRepository.SaveChanges();
                return Result<UserConnection>.SuccessResult(connection);
            }
            catch (Exception ex)
            {
                return Result<UserConnection>.ErrorResult($"An error occurred while updating the connection: {ex.Message}");
            }
        }
    }
}
