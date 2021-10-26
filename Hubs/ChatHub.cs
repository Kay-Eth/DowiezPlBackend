using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DowiezPlBackend.Data;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DowiezPlBackend.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "NotBanned")]
    public class ChatHub : Hub
    {
        private readonly DowiezPlDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ChatHub(DowiezPlDbContext context, UserManager<AppUser> userManager) : base()
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<AppUser> GetMyUserAsync()
        {
            return await _context.Users
                .Include(x => x.Connections)
                .Include(x => x.Participations)
                .ThenInclude(p => p.Conversation)
                .SingleAsync(x => x.UserName == Context.User.Identity.Name);
        }

        private async Task<string> GetMyUserIdAsync()
        {
            return (await _context.Users.SingleAsync(x => x.UserName == Context.User.Identity.Name)).Id.ToString();
        }

        public async Task SendToConversation(string conversationId, string message)
        {
            var messageEntity = new Message()
            {
                Content = message,
                Sender = await GetMyUserAsync(),
                Conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.ConversationId.ToString() == conversationId),
                SentDate = DateTime.UtcNow
            };
            _context.Messages.Add(messageEntity);
            await _context.SaveChangesAsync();
            await Clients.Group(conversationId).SendAsync("Send", conversationId, messageEntity.Sender.Id, messageEntity.MessageId, messageEntity.SentDate, message);
            // await Clients.Caller.SendAsync("Send", conversationId, messageEntity.Sender.Id, messageEntity.MessageId, messageEntity.SentDate, message);
        }

        public async Task NotifyChatJoin(string conversationId, Guid accountId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == accountId);
            await Clients.Group(conversationId).SendAsync("GroupJoin", conversationId, accountId.ToString());
        }

        public override async Task OnConnectedAsync()
        {
            var user = await GetMyUserAsync();
            user.Connections.Add(new Connection
            {
                ConnectionId = Context.ConnectionId,
                UserAgent = Context.GetHttpContext().Request.Headers["User-Agent"],
                Connected = true
            });
            await _context.SaveChangesAsync();

            foreach (var participant in user.Participations)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, participant.Conversation.ConversationId.ToString());
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connection = _context.Connections.Find(Context.ConnectionId);
            connection.Connected = false;
            await _context.SaveChangesAsync();

            await base.OnDisconnectedAsync(exception);
        }
    }
}