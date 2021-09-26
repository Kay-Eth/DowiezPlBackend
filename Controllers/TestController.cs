using System;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos.Demand;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    /// <summary>
    /// Controller for testing new features
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "NotBanned")]
    public class TestController : DowiezPlControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;

        public TestController(IDowiezPlRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(userManager)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("Ping")]
        public async Task<ActionResult> Ping()
        {
            return await Task.Run(() => Ok("PONG"));
        }

        [HttpGet("PingStandard")]
        [Authorize(Roles = "Standard")]
        public async Task<ActionResult> PingStandard()
        {
            return await Task.Run(() => Ok("PONG"));
        }

        [HttpGet("PingModerator")]
        [Authorize(Roles = "Moderator,Admin")]
        public async Task<ActionResult> PingModerator()
        {
            return await Task.Run(() => Ok("PONG"));
        }

        [HttpGet("DemandTest")]
        public async Task<ActionResult<DemandReadDto>> DemandTest()
        {
            var demandFromRepo = await _repository.GetDemandAsync(Guid.Parse("690ebdd1-fd9a-49c2-aa4a-5c5285b3c671"));
            return Ok(_mapper.Map<Demand, DemandReadDto>(demandFromRepo));
        }

        [HttpPost("DemandTest")]
        public async Task<ActionResult> DemandCreateTest(DemandCreateDto demandCreateDto)
        {
            var demand = _mapper.Map<DemandCreateDto, Demand>(demandCreateDto);

            _repository.CreateDemand(demand);
            if (!await _repository.SaveChangesAsync())
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}