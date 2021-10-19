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
                .SingleAsync(x => x.UserName == Context.User.Identity.Name);
        }

        public async Task SendMessage(string message)
        {
            Console.WriteLine("Name:" + Context.User.Identity.Name);
            var me = await GetMyUserAsync();
            
            await Clients.All.SendAsync("ReceiveMessage", me.Id.ToString(), message);
        }

        public async Task SendToGroup(string group, string message)
        {
            await Clients.Group(group).SendAsync("Send", message);
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
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