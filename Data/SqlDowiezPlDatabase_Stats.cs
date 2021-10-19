using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DowiezPlBackend.Data
{
    public partial class SqlDowiezPlDatabase
    {
        public async Task<int> CountOfNewDemands(DateTime before)
        {
            return await _context.Demands.AsNoTracking()
                .CountAsync(d => d.CreationDate > before);
        }

        public async Task<int> CountOfNewTransports(DateTime before)
        {
            return await _context.Transports.AsNoTracking()
                .CountAsync(t => t.CreationDate > before);
        }

        public async Task<int> CountOfNewOpinions(DateTime before)
        {
            return await _context.Opinions.AsNoTracking()
                .CountAsync(o => o.CreationDate > before);
        }

        public async Task<int> CountOfNewGroups(DateTime before)
        {
            return await _context.Groups.AsNoTracking()
                .CountAsync(g => g.CreationDate > before);
        }
    }
}