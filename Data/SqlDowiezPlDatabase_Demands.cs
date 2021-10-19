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
        public void CreateDemand(Demand demand)
        {
            if (demand == null)
                throw new ArgumentNullException(nameof(demand));
            
            _context.Demands.Add(demand);
        }

        public async Task<List<Demand>> SearchDemandsAsync(
            AppUser excludeUser,
            ICollection<DemandCategory> categories,
            Guid? fromCityId,
            Guid? destinationCityId,
            Guid? limitedToGroupId)
        {
            if (categories == null)
                throw new ArgumentNullException(nameof(categories));

            IEnumerable<Guid> fromCities = null;

            if (fromCityId != null)
            {
                var fromCity = await GetCityNotTrackedAsync((Guid)fromCityId);
                fromCities = (await GetCityDistrictsAsync(fromCity)).Select(c => c.CityId);
            }

            IEnumerable<Guid> destinationCities = null;
            if (destinationCityId != null)
            {
                var destCity = await GetCityNotTrackedAsync((Guid)destinationCityId);
                destinationCities = (await GetCityDistrictsAsync(destCity)).Select(c => c.CityId);
            }
            
            return await _context.Demands.AsNoTracking()
                .Include(c => c.From)
                .Include(c => c.Destination)
                .Include(u => u.Creator)
                .Include(u => u.Reciever)
                .Include(t => t.Transport)
                .Include(g => g.LimitedTo)
                .Where(d => d.Status == DemandStatus.Created
                    && categories.Contains(d.Category)
                    && (fromCityId == null ? true : (d.From == null ? true : fromCities.Contains(d.From.CityId)))
                    && (destinationCityId == null ? true : (destinationCities.Contains(d.Destination.CityId)))
                    && (limitedToGroupId == null ? true : d.LimitedTo.GroupId == limitedToGroupId)
                    && d.Creator.Id != excludeUser.Id
                    && d.Reciever.Id != excludeUser.Id
                ).ToListAsync();
        }

        public async Task<List<Demand>> GetUserDemandsAsync(Guid userId)
        {
            return await _context.Demands.AsNoTracking()
                .Include(c => c.From)
                .Include(c => c.Destination)
                .Include(u => u.Creator)
                .Include(u => u.Reciever)
                .Include(t => t.Transport)
                .Include(g => g.LimitedTo)
                .Where(d => d.Creator.Id == userId)
                .ToListAsync();
        }

        public async Task<Demand> GetDemandAsync(Guid demandId)
        {
            return await _context.Demands
                .Include(c => c.From)
                .Include(c => c.Destination)
                .Include(u => u.Creator)
                .Include(u => u.Reciever)
                .Include(t => t.Transport)
                .Include(g => g.LimitedTo)
                .FirstOrDefaultAsync(d => d.DemandId == demandId);
        }

        public async Task<Demand> GetDemandNotTrackedAsync(Guid demandId)
        {
            return await _context.Demands.AsNoTracking()
                .Include(c => c.From)
                .Include(c => c.Destination)
                .Include(u => u.Creator)
                .Include(u => u.Reciever)
                .Include(t => t.Transport)
                .ThenInclude(t => t.StartsIn)
                .Include(t => t.Transport)
                .ThenInclude(t => t.EndsIn)
                .Include(t => t.Transport)
                .ThenInclude(t => t.Creator)
                .Include(g => g.LimitedTo)
                .FirstOrDefaultAsync(d => d.DemandId == demandId);
        }

        public void DeleteDemand(Demand demand)
        {
            if (demand == null)
                throw new ArgumentNullException(nameof(demand));
            
            _context.Demands.Remove(demand);
        }
    }
}