using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DowiezPlBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace DowiezPlBackend.Data
{
    public partial class SqlDowiezPlDatabase
    {
        public void CreateCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));
            
            _context.Cities.Add(city);
        }

        public async Task<List<City>> GetCitiesAsync()
        {
            return await _context.Cities.AsNoTracking().ToListAsync();
        }

        public async Task<City> GetCityAsync(Guid cityId)
        {
            return await _context.Cities.FirstOrDefaultAsync(c => c.CityId == cityId);
        }

        public async Task<City> GetCityNotTrackedAsync(Guid cityId)
        {
            return await _context.Cities.AsNoTracking().FirstOrDefaultAsync(c => c.CityId == cityId);
        }

        public void DeleteCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));
            
            _context.Cities.Remove(city);
        }

        public async Task<List<City>> GetCityDistrictsAsync(City city)
        {
            if (city.CityDistrict != null)
                return new List<City>() { city };
            
            return await _context.Cities.AsNoTracking()
                .Where(c => c.CityName == city.CityName)
                .ToListAsync();
        }
    }
}