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
        public void CreateReport(Report report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));
            
            _context.Reports.Add(report);
        }

        public async Task<List<Report>> GetReportsAsync()
        {
            return await _context.Reports.AsNoTracking()
                .Include(r => r.Reporter)
                .Include(r => r.Reported)
                .ToListAsync();
        }

        public async Task<List<Report>> GetIssuedReportsAsync()
        {
            return await _context.Reports.AsNoTracking()
                .Where(r => r.Status == ReportStatus.Issued)
                .Include(r => r.Reporter)
                .Include(r => r.Reported)
                .ToListAsync();
        }

        public async Task<Report> GetReportAsync(Guid reportId)
        {
            return await _context.Reports
                .Include(r => r.Reporter)
                .Include(r => r.Reported)
                .FirstOrDefaultAsync(r => r.ReportId == reportId);
        }

        public async Task<Report> GetReportNotTrackedAsync(Guid reportId)
        {
            return await _context.Reports.AsNoTracking()
                .Include(r => r.Reporter)
                .Include(r => r.Reported)
                .FirstOrDefaultAsync(r => r.ReportId == reportId);
        }

        public void DeleteReport(Report report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));
            
            _context.Reports.Remove(report);
        }

        public async Task<List<Report>> GetUserReportsAsync(Guid userId)
        {
            return await _context.Reports.AsNoTracking()
                .Include(r => r.Reporter)
                .Include(r => r.Reported)
                .Where(r => r.Reporter.Id == userId)
                .ToListAsync();
        }
    }
}