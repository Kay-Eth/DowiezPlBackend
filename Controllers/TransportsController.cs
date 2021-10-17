using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos;
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

        // /// <summary>
        // /// Searches transports (with status "Declared") based on search data
        // /// </summary>
        // /// <param name="transportSearchDto">Search data</param>
        // /// <response code="200">Returns array of transports</response>
        // [HttpGet("search")]
        // public async Task<ActionResult<IEnumerable<TransportSimpleReadDto>>> GetSearchTransports(TransportSearchDto transportSearchDto)
        // {
        //     var results = await _repository.SearchTransportsAsync(
        //         transportSearchDto.Categories,
        //         transportSearchDto.StartsInCityId,
        //         transportSearchDto.EndsInCityId
        //     );
        //     return Ok(_mapper.Map<IEnumerable<TransportSimpleReadDto>>(results));
        // }

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