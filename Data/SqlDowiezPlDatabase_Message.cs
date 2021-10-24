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
        public async Task<List<Message>> GetMessagesFromConversation(Guid conversationId)
        {
            return await _context.Messages.AsNoTracking()
                .Include(m => m.Conversation)
                .Include(m => m.Sender)
                .Where(m => m.Conversation.ConversationId == conversationId)
                .OrderBy(m => m.SentDate)
                .ToListAsync();
        }

        public async Task<List<Message>> GetLastMessagesFromConversation(Guid conversationId, int count)
        {
            return await _context.Messages.AsNoTracking()
                .Include(m => m.Conversation)
                .Include(m => m.Sender)
                .Where(m => m.Conversation.ConversationId == conversationId)
                .OrderBy(m => m.SentDate)
                .TakeLast(count)
                .ToListAsync();
        }

        public async Task<List<Message>> GetMessagesAfterFromConversation(Guid conversationId, Guid messageId)
        {
            var message = await _context.Messages.AsNoTracking()
                .Include(m => m.Conversation)
                .Where(m => m.Conversation.ConversationId == conversationId)
                .FirstOrDefaultAsync(m => m.MessageId == messageId);
            
            if (message == null)
                return null;

            return await _context.Messages.AsNoTracking()
                .Include(m => m.Conversation)
                .Include(m => m.Sender)
                .Where(m => m.Conversation.ConversationId == conversationId)
                .OrderBy(m => m.SentDate)
                .Where(m => m.SentDate > message.SentDate)
                .ToListAsync();
        }
    }
}