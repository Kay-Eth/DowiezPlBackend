using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos;
using DowiezPlBackend.Dtos.Group;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    public class GroupController : DowiezPlControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;

        public GroupController(IDowiezPlRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(userManager)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // TODO: Dołączanie do groupy, wychodzenie z grupy, wyrzucanie z grupy

        /// <summary>
        /// Returns group
        /// </summary>
        /// <param name="groupId">Group's Id</param>
        /// <response code="200">Returns an opinion</response>
        /// <response code="404">Opinion not found</response>
        [HttpGet("{groupId}", Name = "GetGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GroupReadDto>> GetGroup(Guid groupId)
        {
            var groupFromRepo = await _repository.GetGroupNotTrackedAsync(groupId);
            if (groupFromRepo == null)
                return NotFound();
            
            // var userDb = await GetMyUserAsync();

            // if (!await IsModerator(userDb))
            // {
            //     if (!await _repository.IsUserAMemberOfAGroup(userDb.Id, groupFromRepo.GroupId))
            //         return Forbid();
            // }
            
            return Ok(_mapper.Map<GroupReadDto>(groupFromRepo));
        }

        /// <summary>
        /// Returns all groups
        /// </summary>
        /// <response code="200">Returns an array of groups</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GroupReadDto>>> GetGroups()
        {
            var groups = await _repository.GetGroupsAsync();
            return Ok(_mapper.Map<IEnumerable<GroupReadDto>>(groups));
        }

        /// <summary>
        /// Creates a group
        /// </summary>
        /// <param name="groupCreateDto">New group's data</param>
        /// <response code="201">Group was created successfully</response>
        /// <response code="400">Creation of a group failed</response>
        [HttpPost]
        [Authorize(Roles = "Standard")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GroupReadDto>> CreateGroup(GroupCreateDto groupCreateDto)
        {
            var user = await GetMyUserAsync();

            var group = _mapper.Map<Group>(groupCreateDto);
            group.CreationDate = DateTime.UtcNow;
            group.Creator = user;

            var conversation = new Conversation()
            {
                CreationDate = DateTime.UtcNow
            };

            group.GroupConversation = conversation;

            _repository.CreateConversation(conversation);
            _repository.CreateGroup(group);

            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to create a group.", "GC_CG_1"));

            var groupReadDto = _mapper.Map<GroupReadDto>(group);
            return CreatedAtRoute(nameof(GetGroup), new { groupId = groupReadDto.GroupId }, groupReadDto);
        }

        /// <summary>
        /// Updates a group
        /// </summary>
        /// <param name="groupUpdateDto">Group's new data</param>
        /// <response code="200">Group was updated successfully</response>
        /// <response code="400">Update of a group failed</response>
        /// <response code="403">Only author of group can alter it</response>
        /// <response code="404">Group not found</response>
        [HttpPut]
        [Authorize(Roles = "Standard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GroupReadDto>> UpdateGroup(GroupUpdateDto groupUpdateDto)
        {
            var groupFromRepo = await _repository.GetGroupAsync(groupUpdateDto.GroupId);
            if (groupFromRepo == null)
                return NotFound();
            
            var userDb = await GetMyUserAsync();

            if (groupFromRepo.Creator.Id != userDb.Id)
                return Forbid();
            
            _mapper.Map(groupUpdateDto, groupFromRepo);

            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to update a group.", "GC_UG_1"));

            return Ok(_mapper.Map<GroupReadDto>(groupFromRepo));
        }

        /// <summary>
        /// Deletes a group
        /// </summary>
        /// <param name="groupId">Group's Id</param>
        /// <response code="204">Group was deleted successfully</response>
        /// <response code="400">Update of a group failed</response>
        /// <response code="403">Only author of group or moderator can delete it</response>
        /// <response code="404">Group not found</response>
        [HttpDelete("{groupId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteGroup(Guid groupId)
        {
            var groupFromRepo = await _repository.GetGroupAsync(groupId);
            if (groupFromRepo == null)
                return NotFound();
            
            var userDb = await GetMyUserAsync();

            if (!await IsModerator(userDb)
                && groupFromRepo.Creator.Id != userDb.Id)
            {
                return Forbid();
            }

            _repository.DeleteGroup(groupFromRepo);

            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to delete a group.", "GC_DG_1"));

            return NoContent();
        }
    }
}