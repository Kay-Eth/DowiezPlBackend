using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos;
using DowiezPlBackend.Dtos.Demand;
using DowiezPlBackend.Enums;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    public class DemandsController : DowiezPlControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;
        
        public DemandsController(IDowiezPlRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(userManager)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Searches demands (with status "Created") based on search query
        /// </summary>
        /// <param name="categories">List of integers separated by comma, for example: 0,3 (categories Small and Other)</param>
        /// <param name="fromCityId">From city's id</param>
        /// <param name="destinationCityId">Destination city's id</param>
        /// <param name="limitedToGroupId">Group's id</param>
        /// <response code="200">Returns array of demands</response>
        /// <response code="400">Search failed</response>
        /// <response code="403">Cannot read demands from a group that user is not a member</response>
        /// <response code="404">City not found</response>
        [HttpGet("search")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DemandSimpleReadDto>>> GetSearchDemands(
            [Required] string categories,
            Guid? fromCityId,
            [Required] Guid destinationCityId,
            Guid? limitedToGroupId)
        {
            var user = await GetMyUserAsync();

            if (limitedToGroupId != null)
            {
                if (!(await IsModerator(user) || await _repository.IsUserAMemberOfAGroup(user.Id, (Guid)(limitedToGroupId))))
                {
                    return Forbid();
                }
            }

            var categories_array = categories.Split(",");
            var categories_list = new List<DemandCategory>();

            for (int i = 0; i < categories_array.Length; i++)
            {
                try
                {
                    categories_list.Add((DemandCategory)(Int32.Parse(categories_array[i])));
                }
                catch (Exception e)
                {
                    return BadRequest(new ErrorMessage("Failed to execute search query: " + e.Message, "DC_GSD_1"));
                }
            }

            if (fromCityId != null)
            {
                var fromCity = await _repository.GetCityNotTrackedAsync((Guid)fromCityId);
                if (fromCity == null)
                    return NotFound(new ErrorMessage("From city not found.", "DC_GSD_2"));
            }

            var destCity = await _repository.GetCityNotTrackedAsync(destinationCityId);
            if (destCity == null)
                return NotFound(new ErrorMessage("Destination city not found.", "DC_GSD_3"));

            var results = await _repository.SearchDemandsAsync(
                user,
                categories_list,
                fromCityId,
                destinationCityId,
                limitedToGroupId
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
            var results = await _repository.GetUserDemandsAsync(userId);
            return Ok(_mapper.Map<IEnumerable<DemandSimpleReadDto>>(results));
        }

        /// <summary>
        /// Returns data of a demand
        /// </summary>
        /// <param name="demandId"></param>
        /// <response code="200">Returns data of demands</response>
        /// <response code="403">Demand is limited to a group, that you don't have access to</response>
        /// <response code="404">Demand not found</response>
        [HttpGet("{demandId}", Name = "GetDemand")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        /// <summary>
        /// Creates a demand
        /// </summary>
        /// <param name="demandCreateDto">Demand's data</param>
        /// <response code="201">Returns data of a created demand</response>
        /// <response code="400">Failed to create a demand</response>
        /// <response code="404">City, user or group not found</response>
        [HttpPost]
        [Authorize(Roles = "Standard")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]
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

            var demandReadDto = _mapper.Map<DemandReadDto>(demand);
            return CreatedAtRoute(nameof(GetDemand), new { demandId = demandReadDto.DemandId }, demandReadDto);
        }

        /// <summary>
        /// Updates a demand. Only demand with status Created can be updated.
        /// </summary>
        /// <param name="demandUpdateDto">Demand's new data</param>
        /// <response code="200">Returns data of a updated demand</response>
        /// <response code="400">Failed to update a demand</response>
        /// <response code="404">City, user or group not found</response>
        [HttpPut]
        [Authorize(Roles = "Standard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DemandReadDto>> UpdateDemand(DemandUpdateDto demandUpdateDto)
        {
            var me = await GetMyUserAsync();
            var demandFromRepo = await _repository.GetDemandAsync(demandUpdateDto.DemandId);

            if (demandFromRepo == null)
                return NotFound(new ErrorMessage("Demand does not exist.", "DC_UD_1"));
            
            if (demandFromRepo.Creator.Id != me.Id)
                return Forbid();
            
            if (demandFromRepo.Status != DemandStatus.Created)
                return BadRequest(new ErrorMessage("Cannot update a demand with status other than Created.", "DC_UD_2"));
            
            if (demandUpdateDto.FromCityId != null)
            {
                var cityFrom = await _repository.GetCityNotTrackedAsync((Guid)(demandUpdateDto.FromCityId));
                if (cityFrom == null)
                    return NotFound(new ErrorMessage("City From does not exist.", "DC_UD_3"));

                demandFromRepo.From = cityFrom;
            }

            var cityDest = await _repository.GetCityNotTrackedAsync(demandUpdateDto.DestinationCityId);
            if (cityDest == null)
                return NotFound(new ErrorMessage("City Destination does not exist.", "DC_UD_4"));
            
            demandFromRepo.Destination = cityDest;

            if (demandUpdateDto.RecieverUserId != null)
            {
                var reciever = await GetUserAsync(demandUpdateDto.RecieverUserId.ToString());
                if (reciever == null)
                    return NotFound(new ErrorMessage("Reciever does not exist.", "DC_UD_5"));
                
                demandFromRepo.Reciever = reciever;
            }

            if (demandUpdateDto.LimitedToGroupId != null)
            {
                var group = await _repository.GetGroupNotTrackedAsync((Guid)(demandUpdateDto.LimitedToGroupId));
                if (group == null)
                    return NotFound(new ErrorMessage("Group does not exist.", "DC_UD_6"));
                
                if (!await _repository.IsUserAMemberOfAGroup(me.Id, group.GroupId))
                    return Forbid();

                demandFromRepo.LimitedTo = group;
            }
            
            _mapper.Map(demandUpdateDto, demandFromRepo);
        
            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to update a demand.", "DC_UD_7"));

            return Ok(_mapper.Map<DemandReadDto>(demandFromRepo));
        }

        /// <summary>
        /// Cancels a demand
        /// </summary>
        /// <param name="demandId">Demand's Id</param>
        /// <response code="204">Demand successfully canceled</response>
        /// <response code="400">Failed to cancel demand</response>
        /// <response code="404">Demand not found</response>
        [HttpPost("{demandId}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CancelDemand(Guid demandId)
        {
            var me = await GetMyUserAsync();
            var demandFromRepo = await _repository.GetDemandAsync(demandId);

            if (demandFromRepo == null)
                return NotFound();
            
            if (demandFromRepo.Creator.Id != me.Id)
                return Forbid();
            
            if (demandFromRepo.Status == DemandStatus.InProgress
                || demandFromRepo.Status == DemandStatus.Finished)
            {
                return BadRequest(new ErrorMessage("Cannot cancel a demand with status InProgress or Finished.", "DC_CaD_1"));
            }

            if (demandFromRepo.Status == DemandStatus.Canceled)
            {
                return BadRequest(new ErrorMessage("Demand is already canceled.", "DC_CaD_2"));
            }

            demandFromRepo.Status = DemandStatus.Canceled;
            demandFromRepo.Transport = null;

            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to cancel a demand.", "DC_CaD_3"));
            
            return NoContent();
        }

        /// <summary>
        /// Ask to carry a demand within a transport (NOT IMPLEMENTED)
        /// </summary>
        /// <param name="demandId"></param>
        /// <param name="transportId"></param>
        [HttpPost("{demandId}/ask/{transportId}")]
        public async Task<ActionResult> AskForTransport(Guid demandId, Guid transportId)
        {
            return null;
        }

        /// <summary>
        /// Accepts transport proposition (NOT IMPLEMENTED)
        /// </summary>
        /// <param name="demandId"></param>
        /// <param name="transportId"></param>
        [HttpPost("{demandId}/accept/{transportId}")]
        public async Task<ActionResult> AcceptProposition(Guid demandId, Guid transportId)
        {
            return null;
        }

        /// <summary>
        /// Denies transport proposition (NOT IMPLEMENTED)
        /// </summary>
        /// <param name="demandId"></param>
        /// <param name="transportId"></param>
        [HttpPost("{demandId}/deny/{transportId}")]
        public async Task<ActionResult> DenyProposition(Guid demandId, Guid transportId)
        {
            return null;
        }

        /// <summary>
        /// Cancels transport of demand
        /// </summary>
        /// <param name="demandId"></param>
        /// <param name="transportId"></param>
        [HttpPost("{demandId}/cancel/{transportId}")]
        public async Task<ActionResult> CancelTransportOfDemand(Guid demandId, Guid transportId)
        {
            return null;
        }
    }
}