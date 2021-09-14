using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DowiezPlBackend.Models;

namespace DowiezPlBackend.Data
{
    public interface IDowiezPlRepository
    {
        Task<bool> SaveChanges();

        Task CreateCity(City city);
        Task<IEnumerable<City>> GetCities();
        Task<City> GetCity(Guid cityId);
        Task UpdateCity(City city);
        Task DeleteCity(City city);
    }
}