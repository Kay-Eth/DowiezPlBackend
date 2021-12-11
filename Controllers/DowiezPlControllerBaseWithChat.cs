using System;
using System.Threading.Tasks;
using DowiezPlBackend.Data;
using DowiezPlBackend.Hubs;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace DowiezPlBackend.Controllers
{
    public class DowiezPlControllerBaseWithChat : DowiezPlControllerBase
    {
        protected IDowiezPlRepository _repository;
        IHubContext<ChatHub> _chatHub;

        public DowiezPlControllerBaseWithChat(UserManager<AppUser> userManager, IHubContext<ChatHub> chatHub, IDowiezPlRepository repository) : base(userManager)
        {
            _chatHub = chatHub;
            _repository = repository;
        }

        protected async Task NotifyUserJoinConversation(Guid accountId, Guid conversationId)
        {
            var accountsConnections = await _repository.GetAccountsActiveConnections(accountId);
            await ChatHub.NotifyJoinConv(_chatHub, conversationId.ToString(), accountId, accountsConnections);
        }

        protected async Task NotifyUserLeaveConversation(Guid accountId, Guid conversationId)
        {
            var accountsConnections = await _repository.GetAccountsActiveConnections(accountId);
            await ChatHub.NotifyLeaveConv(_chatHub, conversationId.ToString(), accountId, accountsConnections);
        }
    }
}