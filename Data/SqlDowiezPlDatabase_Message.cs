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
                .ToListAsync();
        }
    }
}