using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos;
using DowiezPlBackend.Dtos.Account;
using DowiezPlBackend.Models;
using DowiezPlBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DowiezPlBackend.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly DowiezPlDbContext _context;

        public AccountsController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration,
            IMapper mapper,
            IMailService mailService,
            DowiezPlDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _mailService = mailService;
            _context = context;
        }

        public async static Task<bool> CheckIfExistsAsync(string userId, UserManager<AppUser> userManager)
        {
            return (await userManager.FindByIdAsync(userId)) != null;
        }
        
        /// <summary>
        /// Returns account information
        /// </summary>
        /// <response code="200">Returns object with user data</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="404">User not found</response>
        /// <response code="423">User is banned</response>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "NotBanned")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountReadDto>> GetAccount()
        {
            var userDb = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (userDb == null)
                return NotFound();
            
            var accountReadDto = _mapper.Map<AppUser, AccountReadDto>(userDb);
            var role = (await _userManager.GetRolesAsync(userDb))[0];
            if (role == "Standard")
                accountReadDto.Role = Enums.Role.Standard;
            else if (role == "Moderator")
                accountReadDto.Role = Enums.Role.Moderator;
            else if (role == "Admin")
                accountReadDto.Role = Enums.Role.Admin;
            else
                return BadRequest(new ErrorMessage($"Role {role} doesn't exist!"));
            
            return Ok(accountReadDto);
        }

        /// <summary>
        /// Creates user with "Standard" role and sends confirmation email
        /// </summary>
        /// <param name="accountCreateDto">Object with information about new user</param>
        /// <response code="204">User created successfully</response>
        /// <response code="400">Failed to create account</response>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateStandardUser(AccountCreateDto accountCreateDto)
        {
            var user = _mapper.Map<AccountCreateDto, AppUser>(accountCreateDto);
            user.Banned = false;
            var userDb = await _userManager.FindByEmailAsync(accountCreateDto.Email);
            if (userDb != null)
                return BadRequest(new ErrorMessage("Failed to create account."));

            var result = await _userManager.CreateAsync(user, accountCreateDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Standard");
                // return Ok(await BuildToken(new AccountLoginDto() {
                //     Email = accountCreateDto.Email,
                //     Password = accountCreateDto.Password
                // }));
                var email_confirmation_token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                await _mailService.SendEmailConfirmationAsync(accountCreateDto.Email, user.Id.ToString(), email_confirmation_token);

                return NoContent();
            }
            else
            {
                // string details = "";
                // foreach (var error in result.Errors)
                // {
                //     details += $"{error.Code}|";
                // }
                // details = details.Remove(details.Length - 1);
                return BadRequest(new ErrorMessage("Failed to create account.", result.Errors));
            }
        }

        /// <summary>
        /// Creates user with "Moderator" role
        /// </summary>
        /// <param name="accountCreateDto">Object with information about new user</param>
        /// <response code="204">User created successfully</response>
        /// <response code="400">Failed to create account</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User not authorized</response>
        [HttpPost("create/moderator")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateModeratorUser(AccountCreateDto accountCreateDto)
        {
            var user = _mapper.Map<AccountCreateDto, AppUser>(accountCreateDto);
            user.Banned = false;
            user.EmailConfirmed = true;
            var userDb = await _userManager.FindByEmailAsync(accountCreateDto.Email);
            if (userDb != null)
                return BadRequest(new ErrorMessage("Failed to create account."));

            var result = await _userManager.CreateAsync(user, accountCreateDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Moderator");
                return NoContent();
            }
            else
            {
                return BadRequest(new ErrorMessage("Failed to create account.", result.Errors));
            }
        }

        /// <summary>
        /// Returns token after successfull authentication
        /// </summary>
        /// <param name="accountLoginDto">Object with login informations</param>
        /// <response code="200">Returns token and expiration timestamp</response>
        /// <response code="400">Invalid login attempt</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccountTokenDto>> Login(AccountLoginDto accountLoginDto)
        {
            var user = await _userManager.FindByEmailAsync(accountLoginDto.Email);
            if (user == null)
                return BadRequest(new ErrorMessage("Login attempt failed."));
            if (user.Banned)
                return BadRequest(new ErrorMessage("User is banned."));

            var result = await _signInManager.CheckPasswordSignInAsync(user, accountLoginDto.Password, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return Ok(await BuildToken(user));
            }
            else
                return BadRequest(new ErrorMessage("Login attempt failed."));
        }
        
        /// <summary>
        /// Returns token after successfull authentication
        /// </summary>
        /// <response code="200">Returns token and expiration timestamp</response>
        /// <response code="400">Renewing token failed</response>
        /// <response code="401">User not authenticated</response>
        [HttpPost("renewtoken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AccountTokenDto>> RenewToken()
        {
            var userDb = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (userDb == null)
                return NotFound();
            if (userDb.Banned)
                return BadRequest(new ErrorMessage("User is banned."));

            return Ok(await BuildToken(userDb));
        }

        /// <summary>
        /// Sets "Banned" property for user
        /// </summary>
        /// <param name="userId">User's id</param>
        /// <param name="status">Desired status (true/false)</param>
        /// <response code="204">Sets user "Banned" property</response>
        /// <response code="400">Request fails</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User not authorized</response>
        [HttpPost("block/{userId}/{status}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Moderator,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> BlockUser(string userId, bool status)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new ErrorMessage($"User with id {userId} not found."));
            user.Banned = status;
            await _userManager.UpdateAsync(user);

            return NoContent();
        }

        /// <summary>
        /// Confirms email address for an account
        /// </summary>
        /// <param name="aecDto">Object with email confirmation information</param>
        /// <response code="204">Email confirmed successfully</response>
        /// <response code="400">Request fails</response>
        [HttpPost("confirmEmail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ConfirmEmail(AccountEmailConfirmationDto aecDto)
        {
            var user = await _userManager.FindByIdAsync(aecDto.UserId);
            var result = await _userManager.ConfirmEmailAsync(user, aecDto.Token)    ;

            if (result.Succeeded)
                return NoContent();
            else
                return BadRequest(new ErrorMessage("Failed to confirm email."));
        }

        private async Task<AccountTokenDto> BuildToken(AppUser appUser)
        {
            var userClaims = await _userManager.GetClaimsAsync(appUser);

            var roles = await _userManager.GetRolesAsync(appUser);
            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }
            
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, appUser.UserName)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(15);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new AccountTokenDto()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}