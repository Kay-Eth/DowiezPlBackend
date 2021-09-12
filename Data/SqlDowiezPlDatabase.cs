using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DowiezPlBackend.Models;

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

        public async Task<City> GetCity(int cityId)
        {
            return await Task.Run(() => _context.Cities.FirstOrDefault((c) => c.IdCi == cityId));
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
    }
}