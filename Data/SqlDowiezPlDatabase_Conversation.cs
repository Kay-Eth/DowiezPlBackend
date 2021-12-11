using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DowiezPlBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace DowiezPlBackend.Data
{
    public partial class SqlDowiezPlDatabase
    {
        public void CreateConversation(Conversation conversation)
        {
            if (conversation == null)
                throw new ArgumentNullException(nameof(conversation));
            
            _context.Conversations.Add(conversation);
        }

        public void DeleteConversation(Conversation conversation)
        {
            if (conversation == null)
                throw new ArgumentNullException(nameof(conversation));

            _context.Conversations.Remove(conversation);
        }

        public async Task<Conversation> GetConversation(Guid conversationId)
        {
            return await _context.Conversations.AsNoTracking()
                .Include(c => c.Participants)
                .ThenInclude(p => p.User)
                .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
                .Include(c => c.OwnerTransport)
                .ThenInclude(t => t.Creator)
                .Include(c => c.OwnerTransport)
                .ThenInclude(t => t.StartsIn)
                .Include(c => c.OwnerTransport)
                .ThenInclude(t => t.EndsIn)
                .Include(c => c.OwnerGroup)
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);
        }

        public async Task<List<Conversation>> GetUserConversationsAsync(Guid userId)
        {
            return await _context.Participants.AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Conversation)
                .ThenInclude(c => c.Participants)
                .ThenInclude(p => p.User)
                .Where(p => p.User.Id == userId)
                .Select(p => p.Conversation)
                .ToListAsync();
        }

        public async Task AddUserToConversation(AppUser user, Conversation conversation)
        {
            if (await _context.Participants.AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Conversation)
                .FirstOrDefaultAsync(p => p.User.Id == user.Id && p.Conversation.ConversationId == conversation.ConversationId)
            != null)
                return;
            
            _context.Participants.Add(new Participant() {
                Conversation = conversation,
                User = user
            });
        }

        public async Task RemoveUserFromConversation(AppUser user, Conversation conversation)
        {
            var participant = await _context.Participants
                .Include(p => p.User)
                .Include(p => p.Conversation)
                .FirstOrDefaultAsync(p => p.User.Id == user.Id && p.Conversation.ConversationId == conversation.ConversationId);
            
            if (participant == null)
                return;
            
            _context.Participants.Remove(participant);
        }
    }
}