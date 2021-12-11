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
        public async Task<List<Connection>> GetAccountsActiveConnections(Guid accountId)
        {
            var user = await _context.Users.AsNoTracking()
                .Include(u => u.Connections)
                .FirstOrDefaultAsync(u => u.Id == accountId);
            
            if (user == null)
                throw new Exception($"User with id {accountId} does not exist.");
            
            return user.Connections.Where(c => c.Connected).ToList();
        }
    }
}