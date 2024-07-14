using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Etities;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PresentationLayer.DTOs;
using System.Text.Json;

namespace PresentationLayer.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        private readonly IDistributedCache _cache;
        private readonly IChatService _chatService;
        private readonly IUserConnectionService _connectionService;
        public ChatHub(IDistributedCache cache, IChatService chatService, IUserConnectionService connectionService)
        {
            _cache = cache;
            _chatService = chatService;
            _connectionService = connectionService;
        }

        public async Task Create(ConnectionDto connectionDto)
        {
            Chat chat = new Chat();
            chat.Name = connectionDto.ChatRoom;
            chat.AuthorName = connectionDto.UserName;

            var chatResult = _chatService.Create(chat);

            if (!chatResult.Success)
                await Clients.Caller.ReciveMessage("Server", chatResult.ErrorMessage);
            else
            {
                var existing = _connectionService.FindByCondition(c => c.UserName == connectionDto.UserName).Any();

                var connection = new UserConnection()
                {
                    UserName = connectionDto.UserName,
                    Chat = await _chatService.FindByCondition(c => c.Name == connectionDto.ChatRoom).FirstAsync(),
                    ConnectingId = Context.ConnectionId
                };


                if (existing)
                    _connectionService.Update(connection);
                else _connectionService.Create(connection);


                var stringConnection = JsonSerializer.Serialize(connection);
                await _cache.SetStringAsync(Context.ConnectionId, stringConnection);

                await Clients.Caller.ReciveMessage("ChatRoom", $"User {connection.UserName} created a chat room.");

                await Groups.AddToGroupAsync(Context.ConnectionId, connection.Chat.Name);
                await Clients
                    .Group(connection.Chat.Name)
                    .ReciveMessage("ChatRoom", $"{connection.UserName} connected.");
            }
        }

        public async Task JoinChat(ConnectionDto connectionDto)
        {
            var chat = await _chatService.FindByCondition(c => c.Name == connectionDto.ChatRoom).FirstAsync();

            if (chat == null)
                await Clients.Caller.ReciveMessage("Server", "The chat does not exist.");
            else
            {
                var existing = _connectionService.FindByCondition(c => c.UserName == connectionDto.UserName).Any();

                var connection = new UserConnection()
                {
                    UserName = connectionDto.UserName,
                    Chat = await _chatService.FindByCondition(c => c.Name == connectionDto.ChatRoom).FirstAsync(),
                    ConnectingId = Context.ConnectionId
                };

                var chatResult = (existing) ? _connectionService.Update(connection) : _connectionService.Create(connection);


                if (!chatResult.Success)
                {
                    await Clients.Caller.ReciveMessage("Server", chatResult.ErrorMessage);
                }
                else
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, connection.Chat.Name);

                    var stringConnection = JsonSerializer.Serialize(connection);
                    await _cache.SetStringAsync(Context.ConnectionId, stringConnection);

                    await Clients
                        .Group(connection.Chat.Name)
                        .ReciveMessage("ChatRoom", $"{connection.UserName} connected.");
                }
            }

        }

        public async Task LeaveChat()
        {
            var stringConnection = _cache.Get(Context.ConnectionId);
            var connection = JsonSerializer.Deserialize<UserConnection>(stringConnection);

            if (connection is not null)
            {
                var connectionResult = _connectionService.Remove(connection);

                if (!connectionResult.Success)
                    await Clients.Caller.ReciveMessage("Server", connectionResult.ErrorMessage);
                else
                {
                    await _cache.RemoveAsync(Context.ConnectionId);
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, connection.Chat.Name);

                    await Clients
                    .Group(connection.Chat.Name)
                    .ReciveMessage("ChatRoom", "You disconnected.");
                    await Clients
                    .Group(connection.Chat.Name)
                    .ReciveMessage("ChatRoom", $"{connection.UserName} disconnected.");
                }
            }
        }

        public async Task SearchChats(string search)
        {
            var chats = await _chatService.FindByCondition(c => c.Name.Contains(search)).Select(c => c.Name).ToListAsync();

            if (chats.Count() == 0)
                await Clients.Caller.ReciveMessage("Server", "No chats found.");
            else
                await Clients.Caller.ReciveStringList("Search", chats);
        }

        public async Task SendMessage(string message)
        {
            var stringConnection = _cache.Get(Context.ConnectionId);
            var connection = JsonSerializer.Deserialize<UserConnection>(stringConnection);

            await Clients
                .Group(connection.Chat.Name)
                .ReciveMessage(connection.UserName, message);
        }

        public async Task Remove()
        {
            var stringConnection = _cache.Get(Context.ConnectionId);
            var connection = JsonSerializer.Deserialize<UserConnection>(stringConnection);

            var chat = _chatService.FindByCondition(c => c.Name == connection.Chat.Name).FirstOrDefault();

            if (chat != null && connection != null)
            {
                if (chat.AuthorName != connection.UserName)
                    await Clients.Caller.ReciveMessage("Server", "The user is not the author.");
                else
                {
                    var chatResult = _chatService.Remove(chat);

                    if (!chatResult.Success)
                    {
                        await Clients.Caller.ReciveMessage("Server", chatResult.ErrorMessage);
                    }
                    else
                    {
                        await Clients
                            .Group(connection.Chat.Name)
                            .ReciveMessage("ChatRoom", $"The user {connection.UserName} deleted the chat.");

                        var connections = await _connectionService.FindByCondition(c => c.Chat.Name == connection.Chat.Name).ToListAsync();
                        try
                        {
                            foreach (var connect in connections)
                            {
                                await _cache.RemoveAsync(connect.ConnectingId);
                                _connectionService.Remove(connect);
                                await Groups.RemoveFromGroupAsync(connect.ConnectingId, connect.Chat.Name);
                            }

                        }
                        catch (Exception)
                        {
                            await Clients.Caller.ReciveMessage("Server", chatResult.ErrorMessage);
                        }
                    }
                }
            }
            else
                await Clients.Caller.ReciveMessage("Server", "Іomething went wrong.");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var stringConnection = _cache.Get(Context.ConnectionId);
            var connection = JsonSerializer.Deserialize<UserConnection>(stringConnection);

            if (connection is not null)
            {
                await _cache.RemoveAsync(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connection.Chat.Name);

                _connectionService.Remove(connection);

                await Clients
                .Group(connection.Chat.Name)
                .ReciveMessage("ChatRoom", $"{connection.UserName} disconnected.");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }

}
