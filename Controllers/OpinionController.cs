using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos;
using DowiezPlBackend.Dtos.Opinion;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    public class OpinionController : DowiezPlControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;

        public OpinionController(IDowiezPlRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(userManager)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns opinion
        /// </summary>
        /// <response code="200">Returns an opinion</response>
        /// <response code="403">User is not allowed to see this opinion</response>
        /// <response code="404">Opinion not found</response>
        [HttpGet("{opinionId}", Name = "GetOpinion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<OpinionReadDto>> GetOpinion(Guid opinionId)
        {
            var opinionFromRepo = await _repository.GetOpinionNotTrackedAsync(opinionId);
            if (opinionFromRepo == null)
                return NotFound();
            
            var userDb = await GetMyUserAsync();

            if (!await IsModerator(userDb)
                && opinionFromRepo.Issuer.Id != userDb.Id
                && opinionFromRepo.Rated.Id != userDb.Id)
            {
                return Forbid();
            }

            return Ok(_mapper.Map<OpinionReadDto>(opinionFromRepo));
        }

        /// <summary>
        /// Returns array of opinions created by currently logged in user
        /// </summary>
        /// <response code="200">Returns array of opinions about a specified user</response>
        [HttpGet("my")]
        [Authorize(Roles = "Standard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OpinionReadDto>>> GetMyOpinions()
        {
            var userDb = await GetMyUserAsync();

            var result = await _repository.GetOpinionsOfUserAsync(userDb.Id.ToString());
            return Ok(_mapper.Map<IEnumerable<OpinionReadDto>>(result));
        }

        /// <summary>
        /// Returns array of opinions about a specified user
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <response code="200">Returns array of opinions about a specified user</response>
        [HttpGet("about/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OpinionReadDto>>> GetOpinionsAboutUser(string userId)
        {
            var result = await _repository.GetOpinionsAboutUserAsync(userId);
            return Ok(_mapper.Map<IEnumerable<OpinionReadDto>>(result));
        }

        /// <summary>
        /// Returns array of opinions created by specified user
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <response code="200">Returns array of opinions about a specified user</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User not authorized</response>
        /// <response code="423">User is banned</response>
        [HttpGet("of/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Moderator,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OpinionReadDto>>> GetOpinionsOfUser(string userId)
        {
            var result = await _repository.GetOpinionsOfUserAsync(userId);
            return Ok(_mapper.Map<IEnumerable<OpinionReadDto>>(result));
        }

        /// <summary>
        /// Creates an opinion
        /// </summary>
        /// <param name="opinionCreateDto">New opinion's data</param>
        /// <response code="201">Opinion was created successfully</response>
        /// <response code="400">Creation of an opinion failed</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User not authorized</response>
        /// <response code="423">User is banned</response>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standard")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        public async Task<ActionResult<OpinionReadDto>> CreateOpinion(OpinionCreateDto opinionCreateDto)
        {
            var issuer = await GetMyUserAsync();
            var rated = await GetUserAsync(opinionCreateDto.RatedId.ToString());
            if (rated == null)
                return BadRequest(new ErrorMessage("Creation of an opinion failed"));
            
            if (await IsModerator(rated))
                return BadRequest(new ErrorMessage("Creation of an opinion failed"));
            
            var opinion = _mapper.Map<Opinion>(opinionCreateDto);
            opinion.CreationDate = System.DateTime.UtcNow;
            opinion.Issuer = issuer;
            opinion.Rated = rated;

            _repository.CreateOpinion(opinion);
            if (!await _repository.SaveChangesAsync())
            {
                return BadRequest("Creation of an opinion failed");
            }

            var opinionReadDto = _mapper.Map<OpinionReadDto>(opinion);

            return CreatedAtRoute(nameof(GetOpinion), new { opinionId = opinionReadDto.OpinionId }, opinionReadDto);
        }
    }
}