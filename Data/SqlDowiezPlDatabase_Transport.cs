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
            AppUser excludeUser,
            ICollection<TransportCategory> categories,
            Guid? startsInCityId,
            Guid? endsInCityId)
        {
            if (categories == null)
                throw new ArgumentNullException(nameof(categories));
            
            City startCity = null;
            City endCity = null;

            if (startsInCityId != null)
                startCity = await GetCityNotTrackedAsync((Guid)startsInCityId);

            if (endsInCityId != null)
                endCity = await GetCityNotTrackedAsync((Guid)endsInCityId);
            
            return await _context.Transports.AsNoTracking()
                .Include(c => c.StartsIn)
                .Include(c => c.EndsIn)
                .Include(u => u.Creator)
                .Where(t => t.Status == TransportStatus.Declared
                    && categories.Contains(t.Category)
                    && (startsInCityId == null ? true : startCity.CityId == t.StartsIn.CityId)
                    && (endsInCityId == null ? true : endCity.CityId == t.EndsIn.CityId)
                    && t.Creator.Id != excludeUser.Id
                ).ToListAsync();
        }

        public async Task<List<Transport>> GetUserTransportsAsync(Guid userId)
        {
            return await _context.Transports.AsNoTracking()
                .Include(c => c.StartsIn)
                .Include(c => c.EndsIn)
                .Include(u => u.Creator)
                .Include(t => t.TransportConversation)
                .Where(t => t.Creator.Id == userId)
                .ToListAsync();
        }

        public async Task<Transport> GetTransportAsync(Guid transportId)
        {
            return await _context.Transports
                .Include(c => c.StartsIn)
                .Include(c => c.EndsIn)
                .Include(u => u.Creator)
                .Include(d => d.Demands)
                .Include(t => t.TransportConversation)
                .FirstOrDefaultAsync(t => t.TransportId == transportId);
        }

        public async Task<Transport> GetTransportNotTrackedAsync(Guid transportId)
        {
            return await _context.Transports.AsNoTracking()
                .Include(c => c.StartsIn)
                .Include(c => c.EndsIn)
                .Include(u => u.Creator)
                .Include(t => t.TransportConversation)
                .Include(d => d.Demands)
                .ThenInclude(c => c.From)
                .Include(d => d.Demands)
                .ThenInclude(c => c.Destination)
                .FirstOrDefaultAsync(t => t.TransportId == transportId);
        }

        public void DeleteTransport(Transport transport)
        {
            if (transport == null)
                throw new ArgumentNullException(nameof(transport));
            
            _context.Transports.Remove(transport);
        }
    }
}