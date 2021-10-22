using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos;
using DowiezPlBackend.Dtos.Conversation;
using DowiezPlBackend.Enums;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    public class ConversationsController : DowiezPlControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;
        
        public ConversationsController(IDowiezPlRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(userManager)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<ConversationReadDto>>> GetMyConversations()
        {
            var me = await GetMyUserAsync();
            var result = await _repository.GetUserConversationsAsync(me.Id);

            return Ok(_mapper.Map<IEnumerable<ConversationReadDto>>(result));
        }

        [HttpGet("{conversationId}", Name = "GetConversationDetails")]
        public async Task<ActionResult<ConversationDetailedReadDto>> GetConversationDetails(Guid conversationId)
        {
            var conversation = await _repository.GetConversation(conversationId);
            if (conversation == null)
                return NotFound();
            
            return Ok(_mapper.Map<ConversationDetailedReadDto>(conversation));
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ConversationDetailedReadDto>> GetConversationWithUser(Guid userId)
        {
            var me = await GetMyUserAsync();
            var user = await GetUserAsync(userId.ToString());

            if (user == null)
                return NotFound(new ErrorMessage("User not found", "CoC_GCwU_1"));
            
            var conversation = (await _repository.GetUserConversationsAsync(me.Id))
                .Where(c => c.Category == ConversationCategory.Normal)
                .FirstOrDefault(c => c.Participants.Count(p =>
                    p.User.Id == me.Id || p.User.Id == user.Id) == 2);
            
            if (conversation == null)
                return NotFound(new ErrorMessage("Conversation not found", "CoC_GCwU_2"));
            
            return Ok(_mapper.Map<ConversationDetailedReadDto>(conversation));
        }

        [HttpPost("user/{userId}/create")]
        public async Task<ActionResult> CreateConversationWithUser(Guid userId)
        {
            var me = await GetMyUserAsync();
            var user = await GetUserAsync(userId.ToString());

            if (user == null)
                return NotFound();
            
            if ((await _repository.GetUserConversationsAsync(me.Id))
                .Where(c => c.Category == ConversationCategory.Normal)
                .Where(c => c.Participants.Count(p => p.User.Id == me.Id || p.User.Id == user.Id) == 2).Count() > 0)
            {
                return BadRequest(new ErrorMessage("Users are already in a private conversation."));
            }
            
            var conversation = new Conversation() {
                CreationDate = DateTime.UtcNow,
                Category = ConversationCategory.Normal
            };
            _repository.CreateConversation(conversation);

            await _repository.AddUserToConversation(me, conversation);
            await _repository.AddUserToConversation(user, conversation);

            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to delete a city.", "CoC_CCwU_1"));
            
            var conversationDetailedReadDto = _mapper.Map<ConversationDetailedReadDto>(conversation);
            return CreatedAtRoute(nameof(GetConversationDetails), new { conversationId = conversationDetailedReadDto.ConversationId }, conversationDetailedReadDto);
        }
    }
}