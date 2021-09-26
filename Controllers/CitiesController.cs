using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos;
using DowiezPlBackend.Dtos.City;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    public class CitiesController : DowiezPlControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;

        public CitiesController(IDowiezPlRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(userManager)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns data of all cities
        /// </summary>
        /// <response code="200">Returns an array of all cities</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CityReadDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CityReadDto>>> GetCities()
        {
            var cities = await _repository.GetCitiesAsync();
            return Ok(_mapper.Map<IEnumerable<CityReadDto>>(cities));
        }

        /// <summary>
        /// Returns data of a city
        /// </summary>
        /// <param name="cityId">City's id</param>
        /// <response code="200">Returns data of a city</response>
        /// <response code="404">City not exits</response>
        [HttpGet("{cityId}", Name = "GetCity")]
        [ProducesResponseType(typeof(CityReadDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CityReadDto>> GetCity(Guid cityId)
        {
            var city = await _repository.GetCityAsync(cityId);
            if (city == null)
                return NotFound();
            
            return Ok(_mapper.Map<CityReadDto>(city));
        }

        /// <summary>
        /// Creates new City
        /// </summary>
        /// <param name="cityCreateDto">New city's data</param>
        /// <response code="201">City was created successfully</response>
        /// <response code="400">Creation of city failed</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<CityReadDto>> CreateCity(CityCreateDto cityCreateDto)
        {
            var city = _mapper.Map<City>(cityCreateDto);
            _repository.CreateCity(city);
            if (!await _repository.SaveChangesAsync())
            {
                return BadRequest();
            }

            var cityReadDto = _mapper.Map<CityReadDto>(city);
            return CreatedAtRoute(nameof(GetCity), new { cityId = cityReadDto.CityId }, cityReadDto);
        }

        /// <summary>
        /// Updates a City
        /// </summary>
        /// <param name="cityUpdateDto">City's new data</param>
        /// <response code="204">City was updated successfully</response>
        /// <response code="400">Update of city failed</response>
        /// <response code="404">City not found</response>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<CityReadDto>), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateCity(CityUpdateDto cityUpdateDto)
        {
            var cityFromRepo = await _repository.GetCityAsync(cityUpdateDto.CityId);
            if (cityFromRepo == null)
                return NotFound();
            
            _mapper.Map(cityUpdateDto, cityFromRepo);

            if (!await _repository.SaveChangesAsync())
                return BadRequest();

            return NoContent();
        }

        /// <summary>
        /// Deletes a City
        /// </summary>
        /// <param name="cityId">City's id</param>
        /// <response code="204">City was deleted successfully</response>
        /// <response code="400">Update of city failed</response>
        /// <response code="404">City not found</response>
        [HttpDelete("{cityId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteCity(Guid cityId)
        {
            var cityFromRepo = await _repository.GetCityAsync(cityId);
            if (cityFromRepo == null)
                return NotFound();
            
            _repository.DeleteCity(cityFromRepo);
            if (!await _repository.SaveChangesAsync())
                return BadRequest();

            return NoContent();
        }
    }
}