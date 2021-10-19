using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos;
using DowiezPlBackend.Dtos.Demand;
using DowiezPlBackend.Dtos.Transport;
using DowiezPlBackend.Enums;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    public class TransportsController : DowiezPlControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;
        
        public TransportsController(IDowiezPlRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(userManager)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Searches transports (with status "Declared") based on search query
        /// </summary>
        /// <param name="categories">List of integers separated by comma, for example: 1,2 (categories Big and WithTrailer)</param>
        /// <param name="startCityId">Start city's id</param>
        /// <param name="endCityId">End city's id</param>
        /// <response code="200">Returns array of transports</response>
        /// <response code="400">Search failed</response>
        /// <response code="404">City not found</response>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TransportSimpleReadDto>>> GetSearchTransports(
            [Required] string categories,
            Guid? startCityId,
            [Required] Guid endCityId)
        {
            var categories_array = categories.Split(",");
            var categories_list = new List<TransportCategory>();

            for (int i = 0; i < categories_array.Length; i++)
            {
                try
                {
                    categories_list.Add((TransportCategory)(Int32.Parse(categories_array[i])));
                }
                catch (Exception e)
                {
                    return BadRequest(new ErrorMessage("Failed to execute search query: " + e.Message, "TC_GST_1"));
                }
            }

            if (startCityId != null)
            {
                var startCity = await _repository.GetCityNotTrackedAsync((Guid)startCityId);
                if (startCity == null)
                    return NotFound(new ErrorMessage("Start city not found.", "TC_GST_2"));
            }
            
            var endCity = await _repository.GetCityNotTrackedAsync(endCityId);
            if (endCity == null)
                return NotFound(new ErrorMessage("End city not found.", "TC_GST_3"));

            var results = await _repository.SearchTransportsAsync(
                categories_list,
                startCityId,
                endCityId
            );

            return Ok(_mapper.Map<IEnumerable<TransportSimpleReadDto>>(results));
        }

        /// <summary>
        /// Returns transports created by currently logged in user
        /// </summary>
        /// <response code="200">Returns array of transports</response>
        [HttpGet("my")]
        [Authorize(Roles = "Standard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TransportSimpleReadDto>>> GetMyTransports()
        {
            var userDb = await GetMyUserAsync();
            var results = await _repository.GetUserTransportsAsync(userDb.Id);
            Console.WriteLine(results);
            return Ok(_mapper.Map<IEnumerable<TransportSimpleReadDto>>(results));
        }

        /// <summary>
        /// Returns transports created by a user
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <response code="200">Returns array of transports</response>
        /// <response code="404">User not found</response>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Moderator,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TransportSimpleReadDto>>> GetUserTransports(Guid userId)
        {
            var user = await GetUserAsync(userId.ToString());
            if (user == null)
                return NotFound();
            var results = _repository.GetUserTransportsAsync(userId);
            return Ok(_mapper.Map<IEnumerable<TransportSimpleReadDto>>(results));
        }

        /// <summary>
        /// Returns data of a transport
        /// </summary>
        /// <param name="transportId"></param>
        /// <response code="200">Returns data of transport</response>
        /// <response code="404">Transport not found</response>
        [HttpGet("{transportId}", Name = "GetTransport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TransportReadDto>> GetTransport(Guid transportId)
        {
            var transportFromRepo = await _repository.GetTransportNotTrackedAsync(transportId);
            if (transportFromRepo == null)
                return NotFound();

            return Ok(_mapper.Map<TransportReadDto>(transportFromRepo));
        }
        
        /// <summary>
        /// Creates a transport
        /// </summary>
        /// <param name="transportCreateDto">Transport's data</param>
        /// <response code="201">Returns data of a created transport</response>
        /// <response code="404">City not found</response>
        [HttpPost]
        [Authorize(Roles = "Standard")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransportReadDto>> CreateTransport(TransportCreateDto transportCreateDto)
        {
            var me = await GetMyUserAsync();

            var transport = _mapper.Map<Transport>(transportCreateDto);
            transport.Creator = me;
            transport.CreationDate = DateTime.UtcNow;
            transport.Status = TransportStatus.Declared;

            var cityStart = await _repository.GetCityAsync(transportCreateDto.StartsInCityId);
            if (cityStart == null)
                return NotFound(new ErrorMessage("City Start does not exists.", "TC_CT_1"));
            
            var cityEnd = await _repository.GetCityAsync(transportCreateDto.EndsInCityId);
            if (cityEnd == null)
                return NotFound(new ErrorMessage("City End does not exists.", "TC_CT_2"));

            transport.StartsIn = cityStart;
            transport.EndsIn = cityEnd;

            _repository.CreateTransport(transport);

            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to create a transport.", "TC_CT_3"));

            var transportReadDto = _mapper.Map<TransportReadDto>(transport);
            return CreatedAtRoute(nameof(GetTransport), new { transportId = transportReadDto.TransportId }, transportReadDto);
        }

        /// <summary>
        /// Updates a transport. Only transport with status Declared can be updated.
        /// </summary>
        /// <param name="transportUpdateDto">Transport's new data</param>
        /// <response code="200">Returns data of a updated transport</response>
        /// <response code="400">Failed to update a transport</response>
        /// <response code="403">Only creator of a transport can do this</response>
        /// <response code="404">City not found</response>
        [HttpPut]
        [Authorize(Roles = "Standard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransportReadDto>> UpdateTransport(TransportUpdateDto transportUpdateDto)
        {
            var me = await GetMyUserAsync();
            var transportFromRepo = await _repository.GetTransportAsync(transportUpdateDto.TransportId);

            if (transportFromRepo == null)
                return NotFound(new ErrorMessage("Transport does not exist.", "TC_UT_1"));
            
            if (transportFromRepo.Creator.Id != me.Id)
                return Forbid();
            
            if (transportFromRepo.Status != TransportStatus.Declared)
                return BadRequest(new ErrorMessage("Cannot update a transport with status other than Declared.", "TC_UT_2"));
            
            var cityStart = await _repository.GetCityAsync(transportUpdateDto.StartsInCityId);
            if (cityStart == null)
                return NotFound(new ErrorMessage("City Start does not exists.", "TC_UT_3"));
            
            var cityEnd = await _repository.GetCityAsync(transportUpdateDto.EndsInCityId);
            if (cityEnd == null)
                return NotFound(new ErrorMessage("City End does not exists.", "TC_UT_4"));

            transportFromRepo.StartsIn = cityStart;
            transportFromRepo.EndsIn = cityEnd;

            _mapper.Map(transportUpdateDto, transportFromRepo);

            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to update a transport.", "TC_UT_5"));

            return Ok(_mapper.Map<TransportReadDto>(transportFromRepo));
        }

        /// <summary>
        /// Cancels a transport
        /// </summary>
        /// <param name="transportId">Transport's Id</param>
        /// <response code="204">Transport successfully canceled</response>
        /// <response code="400">Failed to cancel transport</response>
        /// <response code="403">Only creator of a transport can do this</response>
        /// <response code="404">Transport not found</response>
        [HttpPost("{transportId}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CancelTransport(Guid transportId)
        {
            var me = await GetMyUserAsync();
            var transportFromRepo = await _repository.GetTransportAsync(transportId);

            if (transportFromRepo == null)
                return NotFound();
            
            if (transportFromRepo.Creator.Id != me.Id)
                return Forbid();
            
            if (transportFromRepo.Status == TransportStatus.Canceled)
                return BadRequest(new ErrorMessage("Transport is already canceled", "TC_CaT_1"));
            
            if (transportFromRepo.Status != TransportStatus.Declared)
                return BadRequest(new ErrorMessage("Cannot cancel a transport with status other than Declared.", "TC_CaT_2"));
            
            transportFromRepo.Status = TransportStatus.Canceled;

            var demands = transportFromRepo.Demands;
            foreach (var demand in demands)
            {
                demand.Transport = null;
                demand.Status = DemandStatus.Created;
            }

            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to cancel a transport.", "TC_CaT_3"));
            
            return NoContent();
        }

        /// <summary>
        /// Returns data of a demands connected to the transport
        /// </summary>
        /// <param name="transportId"></param>
        /// <response code="200">Returns data of demands</response>
        /// <response code="403">Only creator of a transport can do this</response>
        /// <response code="404">Transport not found</response>
        [HttpGet("{transportId}/demands")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DemandSimpleReadDto>> GetTransportDemands(Guid transportId)
        {
            var me = await GetMyUserAsync();
            var transportFromRepo = await _repository.GetTransportNotTrackedAsync(transportId);

            if (transportFromRepo == null)
                return NotFound();
            
            if (transportFromRepo.Creator.Id != me.Id)
                return Forbid();
            
            var result = transportFromRepo.Demands;
            return Ok(_mapper.Map<IEnumerable<DemandSimpleReadDto>>(result));
        }

        /// <summary>
        /// Accept request to carry a demand within a transport
        /// </summary>
        /// <param name="transportId">Transport's id</param>
        /// <param name="demandId">Demand's id</param>
        /// <response code="204">Demand carry accepted</response>
        /// <response code="400">Failed to accept transport of demand</response>
        /// <response code="403">Only creator of a transport can do this</response>
        /// <response code="404">Demand or transport not found</response>
        [HttpPost("{transportId}/accept/{demandId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AcceptTransportOfDemand(Guid transportId, Guid demandId)
        {
            var me = await GetMyUserAsync();
            var transportFromRepo = await _repository.GetTransportNotTrackedAsync(transportId);

            if (transportFromRepo == null)
                return NotFound(new ErrorMessage("Transport not found.", "TC_AToD_1"));
            
            if (transportFromRepo.Creator.Id != me.Id)
                return Forbid();
            
            if (transportFromRepo.Status != TransportStatus.Declared)
                return BadRequest(new ErrorMessage("Only transport with status Declared can accept demands.", "TC_AToD_2"));
            
            var demandFromRepo = await _repository.GetDemandAsync(demandId);
            if (demandFromRepo == null)
                return NotFound(new ErrorMessage("Demand not found.", "TC_AToD_3"));
            
            if (demandFromRepo.Transport.TransportId != transportFromRepo.TransportId)
                return BadRequest(new ErrorMessage("Only demand connected to this transport can be accepted.", "TC_AToD_4"));

            if (demandFromRepo.Status != DemandStatus.TransportRequested)
                return BadRequest(new ErrorMessage("Only demand with status TransportRequested can be accepted.", "TC_AToD_5"));

            demandFromRepo.Status = DemandStatus.Accepted;
            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to accept a demand.", "TC_AToD_6"));

            return NoContent();
        }

        /// <summary>
        /// Denies request to carry a demand within a transport (NOT IMPLEMENTED)
        /// </summary>
        /// <param name="transportId">Transport's id</param>
        /// <param name="demandId">Demand's id</param>
        /// <response code="204">Demand carry denied</response>
        /// <response code="400">Failed to deny transport of demand</response>
        /// <response code="403">Only creator of a transport can do this</response>
        /// <response code="404">Demand or transport not found</response>
        [HttpPost("{transportId}/deny/{demandId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DenyTransportOfDemand(Guid transportId, Guid demandId)
        {
            var me = await GetMyUserAsync();
            var transportFromRepo = await _repository.GetTransportNotTrackedAsync(transportId);

            if (transportFromRepo == null)
                return NotFound(new ErrorMessage("Transport not found.", "TC_DToD_1"));
            
            if (transportFromRepo.Creator.Id != me.Id)
                return Forbid();
            
            if (transportFromRepo.Status != TransportStatus.Declared)
                return BadRequest(new ErrorMessage("Only transport with status Declared can deny demands.", "TC_DToD_2"));
            
            var demandFromRepo = await _repository.GetDemandAsync(demandId);
            if (demandFromRepo == null)
                return NotFound(new ErrorMessage("Demand not found.", "TC_DToD_3"));
            
            if (demandFromRepo.Transport.TransportId != transportFromRepo.TransportId)
                return BadRequest(new ErrorMessage("Only demand connected to this transport can be denied.", "TC_DToD_4"));

            if (demandFromRepo.Status != DemandStatus.TransportRequested)
                return BadRequest(new ErrorMessage("Only demand with status TransportRequested can be denied.", "TC_DToD_5"));

            demandFromRepo.Status = DemandStatus.Created;
            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to deny a demand.", "TC_DToD_6"));

            return NoContent();
        }

        /// <summary>
        /// Proposes to carry a demand within a transport (NOT IMPLEMENTED)
        /// </summary>
        /// <param name="transportId">Transport's id</param>
        /// <param name="demandId">Demand's id</param>
        /// <response code="204">Proposed to carry a Demand</response>
        /// <response code="400">Failed to propose carring of demand</response>
        /// <response code="403">Only creator of a transport can do this</response>
        /// <response code="404">Demand or transport not found</response>
        [HttpPost("{transportId}/propose/{demandId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ProposeTransportOfDemand(Guid transportId, Guid demandId)
        {
            var me = await GetMyUserAsync();
            var transportFromRepo = await _repository.GetTransportAsync(transportId);

            if (transportFromRepo == null)
                return NotFound(new ErrorMessage("Transport not found.", "TC_PToD_1"));
            
            if (transportFromRepo.Creator.Id != me.Id)
                return Forbid();
            
            if (transportFromRepo.Status != TransportStatus.Declared)
                return BadRequest(new ErrorMessage("Only transport with status Declared can propose carring of demands.", "TC_PToD_2"));
            
            var demandFromRepo = await _repository.GetDemandAsync(demandId);
            if (demandFromRepo == null)
                return NotFound(new ErrorMessage("Demand not found.", "TC_PToD_3"));
            
            if (demandFromRepo.Status != DemandStatus.Created)
                return BadRequest(new ErrorMessage("Only demand with status Created can be proposed to transport.", "TC_PToD_4"));

            demandFromRepo.Status = DemandStatus.TransportProposed;
            demandFromRepo.Transport = transportFromRepo;
            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to propose carring a demand.", "TC_PToD_5"));

            return NoContent();
        }

        /// <summary>
        /// Begins a transport (NOT IMPLEMENTED)
        /// </summary>
        /// <param name="transportId"></param>
        [HttpPost("{transportId}/begin")]
        public async Task<ActionResult> BeginTransport(Guid transportId)
        {
            return null;
        }

        /// <summary>
        /// Finishes a transport (NOT IMPLEMENTED)
        /// </summary>
        /// <param name="transportId"></param>
        [HttpPost("{transportId}/finish")]
        public async Task<ActionResult> FinishTransport(Guid transportId)
        {
            return null;
        }
    }
}