using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos.Transport;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    public class TransportController : DowiezPlControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;
        
        public TransportController(IDowiezPlRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(userManager)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Searches transports (with status "Declared") based on search data
        /// </summary>
        /// <param name="transportSearchDto">Search data</param>
        /// <response code="200">Returns array of transports</response>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TransportSimpleReadDto>>> GetSearchTransports(TransportSearchDto transportSearchDto)
        {
            var results = await _repository.SearchTransportsAsync(
                transportSearchDto.Categories,
                transportSearchDto.StartsInCityId,
                transportSearchDto.EndsInCityId
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
            var results = _repository.GetUserTransportsAsync(userDb.Id);
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
        [HttpGet("{transportId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TransportReadDto>> GetTransport(Guid transportId)
        {
            var transportFromRepo = await _repository.GetTransportNotTrackedAsync(transportId);
            if (transportFromRepo == null)
                return NotFound();

            return Ok(_mapper.Map<TransportReadDto>(transportFromRepo));
        }
    }
}