using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos;
using DowiezPlBackend.Dtos.Demand;
using DowiezPlBackend.Enums;
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
            var results = await _repository.GetUserDemandsAsync(userDb.Id);
            return Ok(_mapper.Map<IEnumerable<DemandSimpleReadDto>>(results));
        }

        /// <summary>
        /// Returns demands created by a user
        /// </summary>
        /// <param name="userId">User's Id</param>
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

        /// <summary>
        /// Returns data of a demand
        /// </summary>
        /// <param name="demandId"></param>
        /// <response code="200">Returns data of demands</response>
        /// <response code="403">Demand is limited to a group, that you don't have access to</response>
        /// <response code="404">Demand not found</response>
        [HttpGet("{demandId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DemandReadDto>> GetDemand(Guid demandId)
        {
            var demandFromRepo = await _repository.GetDemandNotTrackedAsync(demandId);
            if (demandFromRepo == null)
                return NotFound();
            
            if (demandFromRepo.LimitedTo != null)
            {
                var user = await GetMyUserAsync();
                if (!(await IsModerator(user) || await _repository.IsUserAMemberOfAGroup(user.Id, (Guid)(demandFromRepo.LimitedTo.GroupId))))
                {
                    return Forbid();
                }
            }

            return Ok(_mapper.Map<DemandReadDto>(demandFromRepo));
        }

        [HttpPost]
        [Authorize(Roles = "Standard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DemandReadDto>> CreateDemand(DemandCreateDto demandCreateDto)
        {
            var me = await GetMyUserAsync();

            var demand = _mapper.Map<Demand>(demandCreateDto);
            demand.Creator = me;
            demand.CreationDate = DateTime.UtcNow;
            demand.Status = DemandStatus.Created;

            if (demandCreateDto.FromCityId != null)
            {
                var cityFrom = await _repository.GetCityAsync((Guid)(demandCreateDto.FromCityId));
                if (cityFrom == null)
                    return NotFound(new ErrorMessage("City From does not exist.", "DC_CD_1"));

                demand.From = cityFrom;
            }

            var cityDest = await _repository.GetCityAsync(demandCreateDto.DestinationCityId);
            if (cityDest == null)
                return NotFound(new ErrorMessage("City Destination does not exist.", "DC_CD_2"));
            
            demand.Destination = cityDest;

            if (demandCreateDto.RecieverUserId != null)
            {
                var reciever = await GetUserAsync(demandCreateDto.RecieverUserId.ToString());
                if (reciever == null)
                    return NotFound(new ErrorMessage("Reciever does not exist.", "DC_CD_3"));
                
                demand.Reciever = reciever;
            }

            if (demandCreateDto.LimitedToGroupId != null)
            {
                var group = await _repository.GetGroupAsync((Guid)(demandCreateDto.LimitedToGroupId));
                if (group == null)
                    return NotFound(new ErrorMessage("Group does not exist.", "DC_CD_4"));
                
                if (!await _repository.IsUserAMemberOfAGroup(me.Id, group.GroupId))
                    return Forbid();

                demand.LimitedTo = group;
            }

            _repository.CreateDemand(demand);
            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to create a demand.", "DC_CD_5"));

            return Ok(_mapper.Map<DemandReadDto>(demand));
        }
    }
}