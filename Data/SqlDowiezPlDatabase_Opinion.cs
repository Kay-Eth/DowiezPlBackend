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
        public void CreateOpinion(Opinion opinion)
        {
            if (opinion == null)
                throw new ArgumentNullException(nameof(opinion));
            
            _context.Opinions.Add(opinion);
        }

        public async Task<List<Opinion>> GetOpinionsAsync()
        {
            return await _context.Opinions.AsNoTracking()
                .Include(o => o.Issuer)
                .Include(o => o.Rated)
                .ToListAsync();
        }

        public async Task<Opinion> GetOpinionPairAsync(Guid issuerId, Guid ratedId)
        {
            return await _context.Opinions.AsNoTracking()
                .Include(o => o.Issuer)
                .Include(o => o.Rated)
                .FirstOrDefaultAsync(o => o.Issuer.Id == issuerId && o.Rated.Id == ratedId);
        }

        public async Task<List<Opinion>> GetOpinionsAboutUserAsync(string userId)
        {
            return await _context.Opinions.AsNoTracking()
                .Include(o => o.Issuer)
                .Include(o => o.Rated)
                .Where(o => o.Rated.Id.ToString() == userId)
                .ToListAsync();
        }

        public async Task<List<Opinion>> GetOpinionsOfUserAsync(string userId)
        {
            return await _context.Opinions.AsNoTracking()
                .Include(o => o.Issuer)
                .Include(o => o.Rated)
                .Where(o => o.Issuer.Id.ToString() == userId)
                .ToListAsync();
        }

        public async Task<Opinion> GetOpinionAsync(Guid opinionId)
        {
            return await _context.Opinions
                .Include(o => o.Issuer)
                .Include(o => o.Rated)
                .FirstOrDefaultAsync(o => o.OpinionId == opinionId);
        }

        public async Task<Opinion> GetOpinionNotTrackedAsync(Guid opinionId)
        {
            return await _context.Opinions.AsNoTracking()
                .Include(o => o.Issuer)
                .Include(o => o.Rated)
                .FirstOrDefaultAsync(o => o.OpinionId == opinionId);
        }

        public void DeleteOpinion(Opinion opinion)
        {
            if (opinion == null)
                throw new ArgumentNullException(nameof(opinion));
            
            _context.Opinions.Remove(opinion);
        }
    }
}