using System;
using DowiezPlBackend.Models;

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
    }
}