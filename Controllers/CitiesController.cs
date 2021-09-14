using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos.City;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : DowiezPlControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;

        public CitiesController(IDowiezPlRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(userManager)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET api/Cities
        /// <summary>
        /// Returns data of all cities
        /// </summary>
        /// <response code="200">Returns object with data of all cities</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="423">User is banned</response>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        public async Task<ActionResult<IEnumerable<CityReadDto>>> GetCities()
        {
            await CheckUser();
            var cities = await _repository.GetCities();
            return Ok(_mapper.Map<IEnumerable<CityReadDto>>(cities));
        }

        // GET api/Cities/{cityId}
        /// <summary>
        /// Returns data of all cities
        /// </summary>
        /// <param name="cityId">City's id</param>
        /// <response code="200">Returns object with data of all cities</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="404">City not exits</response>
        /// <response code="423">User is banned</response>
        [HttpGet("{cityId}", Name = "GetCity")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        public async Task<ActionResult<CityReadDto>> GetCity(Guid cityId)
        {
            var city = await _repository.GetCity(cityId);
            if (city == null)
                return NotFound();
            
            return Ok(_mapper.Map<CityReadDto>(city));
        }

        // POST api/Cities
        /// <summary>
        /// Creates new City
        /// </summary>
        /// <param name="cityCreateDto">New city's data</param>
        /// <response code="201">City was created successfully</response>
        /// <response code="400">Creation of city failed</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User not authorized</response>
        /// <response code="423">User is banned</response>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        public async Task<ActionResult<CityReadDto>> CreateCity(CityCreateDto cityCreateDto)
        {
            var city = _mapper.Map<City>(cityCreateDto);
            await _repository.CreateCity(city);
            if (!await _repository.SaveChanges())
            {
                return BadRequest();
            }

            var cityReadDto = _mapper.Map<CityReadDto>(city);
            
            return CreatedAtRoute(nameof(GetCity), new { cityId = cityReadDto.CityId }, cityReadDto);
        }

        // PUT api/Cities
        /// <summary>
        /// Updates a City
        /// </summary>
        /// <param name="cityUpdateDto">City's new data</param>
        /// <response code="204">City was updated successfully</response>
        /// <response code="400">Update of city failed</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User not authorized</response>
        /// <response code="423">User is banned</response>
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        public async Task<ActionResult> UpdateCity(CityUpdateDto cityUpdateDto)
        {
            var cityFromRepo = await _repository.GetCity(cityUpdateDto.CityId);
            if (cityFromRepo == null)
                return NotFound();
            
            _mapper.Map(cityUpdateDto, cityFromRepo);
            await _repository.UpdateCity(cityFromRepo);

            if (!await _repository.SaveChanges())
                return BadRequest();

            return NoContent();
        }

        // DELETE api/Cities/{cityId}
        /// <summary>
        /// Deletes a City
        /// </summary>
        /// <param name="cityId">City's id</param>
        /// <response code="204">City was deleted successfully</response>
        /// <response code="400">Update of city failed</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User not authorized</response>
        /// <response code="423">User is banned</response>
        [HttpDelete("{cityId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        public async Task<ActionResult> DeleteCity(Guid cityId)
        {
            var cityFromRepo = await _repository.GetCity(cityId);
            if (cityFromRepo == null)
                return NotFound();
            
            await _repository.DeleteCity(cityFromRepo);
            if (!await _repository.SaveChanges())
                return BadRequest();

            return NoContent();
        }
    }
}