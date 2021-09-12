using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos.City;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;

        public CitiesController(IDowiezPlRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET api/Cities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityReadDto>>> GetCities()
        {
            var cities = await _repository.GetCities();
            return Ok(_mapper.Map<IEnumerable<CityReadDto>>(cities));
        }

        // GET api/Cities/{cityId}
        [HttpGet("{cityId}")]
        public async Task<ActionResult<CityReadDto>> GetCity(int cityId)
        {
            var city = await _repository.GetCity(cityId);
            if (city == null)
                return NotFound();
            
            return Ok(_mapper.Map<CityReadDto>(city));
        }

        // POST api/Cities
        [HttpPost]
        public async Task<ActionResult> CreateCity(CityCreateDto cityCreateDto)
        {
            var city = _mapper.Map<City>(cityCreateDto);
            await _repository.CreateCity(city);
            if (!await _repository.SaveChanges())
            {
                return BadRequest();
            }
            
            return NoContent();
        }

        // PUT api/Cities
        [HttpPut]
        public async Task<ActionResult> UpdateCity(CityUpdateDto cityUpdateDto)
        {
            var cityFromRepo = await _repository.GetCity(cityUpdateDto.IdCi);
            if (cityFromRepo == null)
                return NotFound();
            
            _mapper.Map(cityUpdateDto, cityFromRepo);
            await _repository.UpdateCity(cityFromRepo);

            if (!await _repository.SaveChanges())
                return BadRequest();

            return NoContent();
        }

        // DELETE api/Cities/{cityId}
        [HttpDelete("{cityId}")]
        public async Task<ActionResult> DeleteCity(int cityId)
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