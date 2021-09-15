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
        public void CreateTransport(Transport transport)
        {
            if (transport == null)
                throw new ArgumentNullException(nameof(transport));
            
            _context.Transports.Add(transport);
        }

        public async Task<List<Transport>> SearchTransportsAsync(
            ICollection<TransportCategory> categories,
            Guid? startsInCityId,
            Guid endsInCityId
        )
        {
            if (categories == null)
                throw new ArgumentNullException(nameof(categories));
            
            return await _context.Transports.AsNoTracking()
                .Include(c => c.StartsIn)
                .Include(c => c.EndsIn)
                .Include(u => u.Creator)
                .Where(t => t.Status == TransportStatus.Declared
                    && categories.Contains(t.Category)
                    && (startsInCityId == null ? true : startsInCityId == t.StartsIn.CityId)
                    && endsInCityId == t.EndsIn.CityId
                ).ToListAsync();
        }

        public async Task<List<Transport>> GetUserTransportsAsync(Guid userId)
        {
            return await _context.Transports.AsNoTracking()
                .Include(c => c.StartsIn)
                .Include(c => c.EndsIn)
                .Include(u => u.Creator)
                .Where(t => t.Creator.Id == userId)
                .ToListAsync();
        }

        public async Task<Transport> GetTransportAsync(Guid transportId)
        {
            return await _context.Transports
                .Include(c => c.StartsIn)
                .Include(c => c.EndsIn)
                .Include(u => u.Creator)
                .FirstOrDefaultAsync(t => t.TransportId == transportId);
        }

        public async Task<Transport> GetTransportNotTrackedAsync(Guid transportId)
        {
            return await _context.Transports
                .Include(c => c.StartsIn)
                .Include(c => c.EndsIn)
                .Include(u => u.Creator)
                .AsNoTracking().FirstOrDefaultAsync(t => t.TransportId == transportId);
        }

        public void DeleteTransport(Transport transport)
        {
            if (transport == null)
                throw new ArgumentNullException(nameof(transport));
            
            _context.Transports.Remove(transport);
        }
    }
}