using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos.Demand;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    public class DemandController : DowiezPlControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;
        
        public DemandController(IDowiezPlRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(userManager)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Searches demands (with status "Created") based on search data
        /// </summary>
        /// <param name="demandSearchDto">Search data</param>
        /// <response code="200">Returns array of demands</response>
        /// <response code="403">Cannot read demands from a group that user is not a member</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DemandSimpleReadDto>>> GetSearchDemands(DemandSearchDto demandSearchDto)
        {
            if (demandSearchDto.LimitedToGroupId != null)
            {
                var user = await GetMyUserAsync();
                if (!(await IsModerator(user) || await _repository.IsUserAMemberOfAGroup(user.Id, (Guid)(demandSearchDto.LimitedToGroupId))))
                {
                    return Forbid();
                }
            }

            var results = await _repository.SearchDemandsAsync(
                demandSearchDto.Categories,
                demandSearchDto.FromCityId,
                demandSearchDto.DestinationCityId,
                demandSearchDto.LimitedToGroupId
            );

            return Ok(_mapper.Map<IEnumerable<DemandSimpleReadDto>>(results));
        }

        /// <summary>
        /// Returns demands created by currently logged in user
        /// </summary>
        /// <response code="200">Returns array of demands</response>
        [HttpGet("my")]
        [Authorize(Roles = "Standard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DemandSimpleReadDto>>> GetMyDemands()
        {
            var userDb = await GetMyUserAsync();
            var results = _repository.GetUserDemandsAsync(userDb.Id);
            return Ok(_mapper.Map<IEnumerable<DemandSimpleReadDto>>(results));
        }

        /// <summary>
        /// Returns demands created by a user
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Returns array of demands</response>
        /// <response code="404">User not found</response>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Moderator,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DemandSimpleReadDto>>> GetUserDemands(Guid userId)
        {
            var user = await GetUserAsync(userId.ToString());
            if (user == null)
                return NotFound();
            var results = _repository.GetUserDemandsAsync(userId);
            return Ok(_mapper.Map<IEnumerable<DemandSimpleReadDto>>(results));
        }
    }
}