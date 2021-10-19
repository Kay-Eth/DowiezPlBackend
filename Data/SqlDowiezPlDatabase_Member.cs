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
        public void CreateMember(Member member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            _context.Members.Add(member);
        }

        public async Task<Member> GetMemberAsync(Guid memberId)
        {
            return await _context.Members
                .Include(m => m.Group)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MemberId == memberId);
        }

        public async Task<Member> GetMemberNotTrackedAsync(Guid memberId)
        {
            return await _context.Members.AsNoTracking()
                .Include(m => m.Group)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.MemberId == memberId);
        }

        public async Task<List<Member>> GetGroupMembersAsync(Guid groupId)
        {
            return await _context.Members
                .Include(m => m.Group)
                .Include(m => m.User)
                .Where(m => m.Group.GroupId == groupId)
                .ToListAsync();
        }

        public void DeleteMember(Member member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            _context.Members.Remove(member);
        }

        public async Task<bool> IsUserAMemberOfAGroup(Guid userId, Guid groupId)
        {
            return await _context.Members.AsNoTracking()
                .Include(m => m.Group)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Group.GroupId == groupId && m.User.Id == userId) != null;
        }

        public async Task<List<Member>> GetUserMembershipsAsync(Guid userId)
        {
            return await _context.Members
                .Include(m => m.Group)
                .Include(m => m.User)
                .Where(m => m.User.Id == userId)
                .ToListAsync();
        }
    }
}