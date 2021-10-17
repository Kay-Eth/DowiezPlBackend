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
    public class OpinionsController : DowiezPlControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;

        public OpinionsController(IDowiezPlRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(userManager)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns opinion
        /// </summary>
        /// <param name="opinionId">Opinion's Id</param>
        /// <response code="200">Returns an opinion</response>
        /// <response code="404">Opinion not found</response>
        [HttpGet("{opinionId}", Name = "GetOpinion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<OpinionReadDto>> GetOpinion(Guid opinionId)
        {
            var opinionFromRepo = await _repository.GetOpinionNotTrackedAsync(opinionId);
            if (opinionFromRepo == null)
                return NotFound();
            
            // var userDb = await GetMyUserAsync();

            // if (!await IsModerator(userDb)
            //     && opinionFromRepo.Issuer.Id != userDb.Id
            //     && opinionFromRepo.Rated.Id != userDb.Id)
            // {
            //     return Forbid();
            // }

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
        [HttpGet("of/{userId}")]
        [Authorize(Roles = "Moderator,Admin")]
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
        [HttpPost]
        [Authorize(Roles = "Standard")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OpinionReadDto>> CreateOpinion(OpinionCreateDto opinionCreateDto)
        {
            var issuer = await GetMyUserAsync();
            var rated = await GetUserAsync(opinionCreateDto.RatedId.ToString());
            if (rated == null)
                return BadRequest(new ErrorMessage("Failed to create an opinion.", "OC_CO_1"));
            
            if (await IsModerator(rated))
                return BadRequest(new ErrorMessage("Failed to create an opinion.", "OC_CO_2"));
            
            var opinion = _mapper.Map<Opinion>(opinionCreateDto);
            opinion.CreationDate = System.DateTime.UtcNow;
            opinion.Issuer = issuer;
            opinion.Rated = rated;

            _repository.CreateOpinion(opinion);
            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to create an opinion.", "OC_CO_3"));

            var opinionReadDto = _mapper.Map<OpinionReadDto>(opinion);
            return CreatedAtRoute(nameof(GetOpinion), new { opinionId = opinionReadDto.OpinionId }, opinionReadDto);
        }

        /// <summary>
        /// Updates an opinion
        /// </summary>
        /// <param name="opinionUpdateDto">Opinion's new data</param>
        /// <response code="200">Opinion was updated successfully</response>
        /// <response code="400">Update of an opinion failed</response>
        /// <response code="403">Only author of opinion can alter it</response>
        /// <response code="404">Opinion not found</response>
        [HttpPut]
        [Authorize(Roles = "Standard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OpinionReadDto>> UpdateOpinion(OpinionUpdateDto opinionUpdateDto)
        {
            var opinionFromRepo = await _repository.GetOpinionAsync(opinionUpdateDto.OpinionId);
            if (opinionFromRepo == null)
                return NotFound();
            
            var userDb = await GetMyUserAsync();

            if (opinionFromRepo.Issuer.Id != userDb.Id)
                return Forbid();
            
            _mapper.Map(opinionUpdateDto, opinionFromRepo);

            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to update an opinion.", "OC_UO_1"));

            return Ok(_mapper.Map<OpinionReadDto>(opinionFromRepo));
        }

        /// <summary>
        /// Deletes an opinion
        /// </summary>
        /// <param name="opinionId">Opinion's Id</param>
        /// <response code="204">Opinion was deleted successfully</response>
        /// <response code="400">Update of an opinion failed</response>
        /// <response code="403">Only author of opinion or moderator can delete it</response>
        /// <response code="404">Opinion not found</response>
        [HttpDelete("{opinionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteOpinion(Guid opinionId)
        {
            var opinionFromRepo = await _repository.GetOpinionAsync(opinionId);
            if (opinionFromRepo == null)
                return NotFound();
            
            var userDb = await GetMyUserAsync();

            if (!await IsModerator(userDb)
                && opinionFromRepo.Issuer.Id != userDb.Id
                && opinionFromRepo.Rated.Id != userDb.Id)
            {
                return Forbid();
            }

            _repository.DeleteOpinion(opinionFromRepo);

            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to delete an opinion.", "OC_DO_1"));

            return NoContent();
        }
    }
}