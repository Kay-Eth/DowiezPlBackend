using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos;
using DowiezPlBackend.Dtos.Stats;
using DowiezPlBackend.Enums;
using DowiezPlBackend.Models;
using DowiezPlBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{   
    /// <summary>
    /// Only for moderators and admins
    /// </summary>
    [Authorize(Roles = "Moderator,Admin")]
    public class StatsController : DowiezPlControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;
        
        public StatsController(IDowiezPlRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(userManager)
        {
            _repository = repository;
            _mapper = mapper;
        }

        protected async Task<bool> IsAdmin(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            for (int i = 0; i < roles.Count; i++)
            {
                if (roles[i] == "Admin")
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns count of issued reports divided to categories
        /// </summary>
        /// <response code="200">Returns object with count of issued reports divided to categories</response>
        [HttpGet("countOfReports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ReportsCountsStatsDto>> ReportsCount()
        {
            var reports = await _repository.GetIssuedReportsAsync();

            var result = new ReportsCountsStatsDto();
            result.UserCount = reports.Count(r => r.Category == ReportCategory.User);
            result.TransportCount = reports.Count(r => r.Category == ReportCategory.Transport);
            result.DemandCount = reports.Count(r => r.Category == ReportCategory.Demand);
            result.GroupCount = reports.Count(r => r.Category == ReportCategory.Group);
            result.TechnicalCount = reports.Count(r => r.Category == ReportCategory.Technical);
            result.OpinionCount = reports.Count(r => r.Category == ReportCategory.Opinion);

            return Ok(result);
        }

        /// <summary>
        /// Returns count of new objects created in the past {days} days
        /// </summary>
        /// <param name="days">Days before</param>
        /// <returns></returns>
        [HttpGet("newCounts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<NewStatsDto>> NewCountStats([Required] int days)
        {
            var before = DateTime.UtcNow.AddDays(-days);
            
            var result = new NewStatsDto();
            var users = new List<AppUser>();
            foreach (var user in _userManager.Users.ToList())
            {
                if (!await IsModerator(user))
                    users.Add(user);
            }

            result.Users = users.Count(u => u.CreationDate > before);
            result.Demands = await _repository.CountOfNewDemands(before);
            result.Transports = await _repository.CountOfNewTransports(before);
            result.Opinions = await _repository.CountOfNewOpinions(before);
            result.Groups = await _repository.CountOfNewGroups(before);

            return Ok(result);
        }

        [HttpGet("creationStats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CreationStatsDto>> GetCreationStats()
        {
            if (!await IsAdmin(await GetMyUserAsync()))
                return Forbid();

            if (System.IO.File.Exists(StatsScheduleService.STATS_FILE_PATH))
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<CreationStatsDto>(
                    await System.IO.File.ReadAllTextAsync(StatsScheduleService.STATS_FILE_PATH));
                return Ok(result);
            }
            else
            {
                return NotFound(new ErrorMessage("Stats not found. There are no stats available currently.", "SC_GCS_1"));
            }
        }
    }
}