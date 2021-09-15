using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace DowiezPlBackend.Data
{
    public partial class SqlDowiezPlDatabase : IDowiezPlRepository
    {
        private readonly DowiezPlDbContext _context;

        public SqlDowiezPlDatabase(DowiezPlDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}