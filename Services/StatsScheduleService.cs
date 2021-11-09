using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using DowiezPlBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace DowiezPlBackend.Services
{
    public class StatsScheduleService : IHostedService, IDisposable
    {
        public const string STATS_FILE_PATH = "./stats.data";
        private System.Timers.Timer _timer;
        private readonly CronExpression _expression;
        private readonly TimeZoneInfo _timeZoneInfo;

        private readonly IServiceScopeFactory _scopeFactory;

        public StatsScheduleService(IServiceScopeFactory scopeFactory)
        {
            // _expression = CronExpression.Parse("* * * * *");
            _expression = CronExpression.Parse("0 3 * * *");
            _timeZoneInfo = TimeZoneInfo.Utc;
            
            _scopeFactory = scopeFactory; 
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await ScheduleJob(cancellationToken);
        }

        protected async Task ScheduleJob(CancellationToken cancellationToken)
        {
            var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                if (delay.TotalMilliseconds <= 0)
                {
                    await ScheduleJob(cancellationToken);
                }
                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();
                    _timer = null;

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await DoWork(cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await ScheduleJob(cancellationToken);
                    }
                };
                _timer.Start();
            }
            await Task.CompletedTask;
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            Console.WriteLine("Creating stats file...");
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DowiezPlDbContext>();
                
                var resultUsers = new SortedDictionary<DateTime, int>(await dbContext.Users.AsQueryable()
                    .GroupBy(u => u.CreationDate.Date)
                    .Select(g => new { g.Key, Count = g.Count()})
                    .AsAsyncEnumerable()
                    .ToDictionaryAsync(v => v.Key, v => v.Count));
                
                var resultDemands = new SortedDictionary<DateTime, int>(await dbContext.Demands.AsQueryable()
                    .GroupBy(d => d.CreationDate.Date)
                    .Select(g => new { g.Key, Count = g.Count()})
                    .AsAsyncEnumerable()
                    .ToDictionaryAsync(v => v.Key, v => v.Count));

                var resultTransports = new SortedDictionary<DateTime, int>(await dbContext.Transports.AsQueryable()
                    .GroupBy(t => t.CreationDate.Date)
                    .Select(g => new { g.Key, Count = g.Count()})
                    .AsAsyncEnumerable()
                    .ToDictionaryAsync(v => v.Key, v => v.Count));
                
                await File.WriteAllTextAsync(STATS_FILE_PATH, JsonConvert.SerializeObject(new Dictionary<string, object>()
                {
                    { "CreationDate", DateTime.UtcNow.ToString("o") },
                    { "Users", resultUsers },
                    { "Demands", resultDemands },
                    { "Transports", resultTransports }
                }, new JsonSerializerSettings() {
                    DateFormatString = "yyyy-MM-dd"
                }), System.Text.Encoding.UTF8);
            }

            Console.WriteLine("Stats file creation finished.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Stop();
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}