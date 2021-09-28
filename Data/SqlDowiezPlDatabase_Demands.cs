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
            ICollection<DemandCategory> categories,
            Guid? fromCityId,
            Guid destinationCityId,
            Guid? limitedToGroupId)
        {
            if (categories == null)
                throw new ArgumentNullException(nameof(categories));
            
            return await _context.Demands.AsNoTracking()
                .Include(c => c.From)
                .Include(c => c.Destination)
                .Include(u => u.Creator)
                .Include(u => u.Reciever)
                .Include(t => t.Transport)
                .Include(g => g.LimitedTo)
                .Where(d => d.Status == DemandStatus.Created
                    && categories.Contains(d.Category)
                    && (fromCityId == null ? true : (d.From == null ? true : d.From.CityId == fromCityId))
                    && destinationCityId == d.Destination.CityId
                    && (limitedToGroupId == null ? true : d.LimitedTo.GroupId == limitedToGroupId)
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