using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DowiezPlBackend.Enums;
using DowiezPlBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace DowiezPlBackend.Data
{
    public partial class SqlDowiezPlDatabase
    {
        public void CreateGroup(Group group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            _context.Groups.Add(group);
        }

        public async Task<Group> GetGroupAsync(Guid groupId)
        {
            return await _context.Groups
                .Include(g => g.LimitedBy)
                .Include(g => g.Members)
                .Include(g => g.GroupConversation)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);
        }

        public async Task<Group> GetGroupNotTrackedAsync(Guid groupId)
        {
            return await _context.Groups.AsNoTracking()
                .Include(g => g.LimitedBy)
                .Include(g => g.Members)
                .Include(g => g.GroupConversation)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            return await _context.Groups.AsNoTracking()
                .Include(g => g.LimitedBy)
                .Include(g => g.Members)
                .Include(g => g.GroupConversation)
                .ToListAsync();
        }

        public void DeleteGroup(Group group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));
            
            _context.Groups.Remove(group);
        }
    }
}