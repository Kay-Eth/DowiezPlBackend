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
            Guid endsInCityId)
        {
            if (categories == null)
                throw new ArgumentNullException(nameof(categories));
            
            IEnumerable<Guid> startCities = null;

            if (startsInCityId != null)
            {
                var startCity = await GetCityNotTrackedAsync((Guid)startsInCityId);
                startCities = (await GetCityDistrictsAsync(startCity)).Select(c => c.CityId);
            }

            IEnumerable<Guid> endCities = (await GetCityDistrictsAsync(await GetCityNotTrackedAsync(endsInCityId))).Select(c => c.CityId);            
            
            return await _context.Transports.AsNoTracking()
                .Include(c => c.StartsIn)
                .Include(c => c.EndsIn)
                .Include(u => u.Creator)
                .Where(t => t.Status == TransportStatus.Declared
                    && categories.Contains(t.Category)
                    && (startsInCityId == null ? true : startCities.Contains(t.StartsIn.CityId))
                    && endCities.Contains(t.EndsIn.CityId)
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
                .Include(d => d.Demands)
                .FirstOrDefaultAsync(t => t.TransportId == transportId);
        }

        public async Task<Transport> GetTransportNotTrackedAsync(Guid transportId)
        {
            return await _context.Transports.AsNoTracking()
                .Include(c => c.StartsIn)
                .Include(c => c.EndsIn)
                .Include(u => u.Creator)
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