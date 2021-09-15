using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace DowiezPlBackend.Data
{
    public class SqlDowiezPlDatabase : IDowiezPlRepository
    {
        private readonly DowiezPlDbContext _context;

        public SqlDowiezPlDatabase(DowiezPlDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveChanges()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

#region CITIES
        public async Task CreateCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));
            
            await _context.Cities.AddAsync(city);
        }

        public async Task<IEnumerable<City>> GetCities()
        {
            return await Task.Run(() => _context.Cities);
        }

        public async Task<City> GetCity(Guid cityId)
        {
            return await Task.Run(() => _context.Cities.FirstOrDefault(c => c.CityId == cityId));
        }

        public async Task DeleteCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));
            
            await Task.Run(() => _context.Cities.Remove(city));
        }

        public async Task UpdateCity(City city)
        {
            await Task.Run(() => _context.Cities.Update(city));
        }
#endregion

#region DEMANDS

        public async Task CreateDemand(Demand demand)
        {
            if (demand == null)
                throw new ArgumentNullException(nameof(demand));
            
            await _context.Demands.AddAsync(demand);
        }

        public async Task<Demand> GetDemand(Guid demandId)
        {
            return await Task.Run(() => _context.Demands
                .Include(c => c.From)
                .Include(c => c.Destination)
                .Include(u => u.Creator)
                .Include(u => u.Reciever)
                .Include(t => t.Transport)
                .Include(g => g.LimitedTo)
                .FirstOrDefault(d => d.DemandId == demandId));
        }

#endregion
    }
}