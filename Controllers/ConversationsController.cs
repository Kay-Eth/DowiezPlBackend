using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos;
using DowiezPlBackend.Dtos.Conversation;
using DowiezPlBackend.Enums;
using DowiezPlBackend.Hubs;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DowiezPlBackend.Controllers
{
    public class ConversationsController : DowiezPlControllerBaseWithChat
    {
        IMapper _mapper;
        
        public ConversationsController(UserManager<AppUser> userManager, IHubContext<ChatHub> chatHub, IDowiezPlRepository repository, IMapper mapper) : base(userManager, chatHub, repository)
        {
            _mapper = mapper;
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<ConversationReadDto>>> GetMyConversations()
        {
            var me = await GetMyUserAsync();
            var result = await _repository.GetUserConversationsAsync(me.Id);

            return Ok(_mapper.Map<IEnumerable<ConversationReadDto>>(result));
        }

        [HttpGet("{conversationId}/small")]
        public async Task<ActionResult<ConversationSmallDetailReadDto>> GetConversationSmallDetails(Guid conversationId)
        {
            var me = await GetMyUserAsync();

            var conversation = await _repository.GetConversation(conversationId);
            if (conversation == null)
                return NotFound();
            
            var result = _mapper.Map<ConversationSmallDetailReadDto>(conversation);
            if (conversation.Category == ConversationCategory.Normal)
            {
                foreach (var part in conversation.Participants)
                {
                    if (part.User.Id != me.Id)
                    {
                        result.Name = part.User.FirstName + " " + part.User.LastName;
                        break;
                    }
                }
            }
            else if (conversation.Category == ConversationCategory.Group)
            {
                result.Name = conversation.OwnerGroup.Name;
            }
            else if (conversation.Category == ConversationCategory.Transport)
            {
                result.Name = conversation.OwnerTransport.Creator.LastName + ", " + conversation.OwnerTransport.Creator.FirstName;
            }
            
            return Ok(result);
        }

        [HttpGet("{conversationId}", Name = "GetConversationDetails")]
        public async Task<ActionResult<ConversationDetailedReadDto>> GetConversationDetails(Guid conversationId)
        {
            var me = await GetMyUserAsync();

            var conversation = await _repository.GetConversation(conversationId);
            if (conversation == null)
                return NotFound();
            
            var result = _mapper.Map<ConversationDetailedReadDto>(conversation);
            if (conversation.Category == ConversationCategory.Normal)
            {
                foreach (var part in conversation.Participants)
                {
                    if (part.User.Id != me.Id)
                    {
                        result.Name = part.User.FirstName + " " + part.User.LastName;
                        break;
                    }
                }
            }
            else if (conversation.Category == ConversationCategory.Group)
            {
                result.Name = conversation.OwnerGroup.Name;
            }
            else if (conversation.Category == ConversationCategory.Transport)
            {
                result.Name = conversation.OwnerTransport.Creator.LastName + ", " + conversation.OwnerTransport.Creator.FirstName;
            }
            
            return Ok(result);
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
                .Where(c => c.Participants.Count(
                    p => p.User.Id == me.Id
                    || p.User.Id == user.Id) == 2)
                .Count() > 0)
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

            await NotifyUserJoinConversation(me.Id, conversation.ConversationId);
            await NotifyUserJoinConversation(user.Id, conversation.ConversationId);
            
            var conversationDetailedReadDto = _mapper.Map<ConversationDetailedReadDto>(conversation);
            return CreatedAtRoute(nameof(GetConversationDetails), new { conversationId = conversationDetailedReadDto.ConversationId }, conversationDetailedReadDto);
        }
    }
}