using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GroupsController : DowiezPlControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;

        public GroupsController(IDowiezPlRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(userManager)
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

        [HttpGet("my")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GroupReadDto>>> GetMyGroups()
        {
            var me = await GetMyUserAsync();
            var groups = (await _repository.GetUserMembershipsAsync(me.Id)).Select(m => m.Group);
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
            var me = await GetMyUserAsync();

            var group = _mapper.Map<Group>(groupCreateDto);
            group.CreationDate = DateTime.UtcNow;
            group.Creator = me;

            var conversation = new Conversation()
            {
                CreationDate = DateTime.UtcNow
            };

            group.GroupConversation = conversation;

            _repository.CreateConversation(conversation);
            _repository.CreateGroup(group);
            _repository.CreateMember(new Member() {
                User = me,
                Group = group
            });

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
            
            var me = await GetMyUserAsync();

            if (groupFromRepo.Creator.Id != me.Id)
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
            
            var me = await GetMyUserAsync();

            if (!await IsModerator(me)
                && groupFromRepo.Creator.Id != me.Id)
            {
                return Forbid();
            }

            _repository.DeleteGroup(groupFromRepo);
            _repository.DeleteConversation(groupFromRepo.GroupConversation);

            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to delete a group.", "GC_DG_1"));

            return NoContent();
        }

        /// <summary>
        /// Allows a user to join a group
        /// </summary>
        /// <param name="groupId">Group's Id</param>
        /// <response code="204">Joined group successfully</response>
        /// <response code="400">Joining a group failed</response>
        /// <response code="404">Group not found</response>
        [HttpPost("{groupId}/join")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> JoinGroup(Guid groupId)
        {
            var groupFromRepo = await _repository.GetGroupAsync(groupId);
            if (groupFromRepo == null)
                return NotFound();
            
            var me = await GetMyUserAsync();

            if (await _repository.IsUserAMemberOfAGroup(me.Id, groupFromRepo.GroupId))
                return BadRequest(new ErrorMessage("You are already member of this group.", "GC_JG_1"));
            
            _repository.CreateMember(new Member() {
                User = me,
                Group = groupFromRepo
            });

            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to join a group.", "GC_JG_2"));
            
            return NoContent();
        }

        /// <summary>
        /// Allows a user to leave a group. You cannot leave a group if you are a creator
        /// </summary>
        /// <param name="groupId">Group's Id</param>
        /// <response code="204">Leaved group successfully</response>
        /// <response code="400">Leaving a group failed</response>
        /// <response code="404">Group not found</response>
        [HttpPost("{groupId}/leave")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> LeaveGroup(Guid groupId)
        {
            var groupFromRepo = await _repository.GetGroupAsync(groupId);
            if (groupFromRepo == null)
                return NotFound();
            
            var me = await GetMyUserAsync();

            var member = await _repository.GetMemberAsync(groupId, me.Id);
            if (member == null)
                return BadRequest(new ErrorMessage("You must be a member of this group to leave it.", "GC_LG_1"));

            if (groupFromRepo.Creator.Id == me.Id)
                return BadRequest(new ErrorMessage("You cannot leave a group you created.", "GC_LG_2"));

            _repository.DeleteMember(member);

            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to leave a group.", "GC_LG_3"));
            
            return NoContent();
        }
    }
}