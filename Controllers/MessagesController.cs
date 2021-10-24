using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos.Message;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    public class MessagesController : DowiezPlControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;

        public MessagesController(IDowiezPlRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(userManager)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("conversation/{conversationId}")]
        public async Task<ActionResult<IEnumerable<MessageSimpleReadDto>>> GetAllFromConversation(Guid conversationId)
        {
            var result = await _repository.GetMessagesFromConversation(conversationId);
            return Ok(_mapper.Map<IEnumerable<MessageSimpleReadDto>>(result));
        }

        [HttpGet("conversation/{conversationId}/last")]
        public async Task<ActionResult<IEnumerable<MessageSimpleReadDto>>> GetLastFromConversation(Guid conversationId)
        {
            var result = await _repository.GetMessagesFromConversation(conversationId);
            return Ok(_mapper.Map<IEnumerable<MessageSimpleReadDto>>(result));
        }
    }
}