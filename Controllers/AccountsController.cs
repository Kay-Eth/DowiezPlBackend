using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos.Account;
using DowiezPlBackend.Exceptions;
using DowiezPlBackend.Models;
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
    [Produces("application/json")]
    [Route("/api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        private readonly IMapper _mapper;
        private readonly DowiezPlDbContext _context;

        public AccountsController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration,
            IMapper mapper,
            DowiezPlDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _context = context;
        }

        public static void ThrowIfBanned(AppUser user)
        {
            if (user.Banned)
                throw new BannedUserException();
        }
        
        /// <summary>
        /// Returns account information
        /// </summary>
        /// <response code="200">Returns object with user data</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="404">User not found</response>
        /// <response code="423">User is banned</response>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        public async Task<ActionResult<AccountReadDto>> GetAccount()
        {
            var userDb = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (userDb == null)
                return NotFound();
            ThrowIfBanned(userDb);
            
            var accountReadDto = _mapper.Map<AppUser, AccountReadDto>(userDb);
            var role = (await _userManager.GetRolesAsync(userDb))[0];
            if (role == "Standard")
                accountReadDto.Role = Enums.Role.Standard;
            else if (role == "Moderator")
                accountReadDto.Role = Enums.Role.Moderator;
            else if (role == "Admin")
                accountReadDto.Role = Enums.Role.Admin;
            else
                throw new DowiezPlException($"Role {role} doesn't exist!");
            
            return Ok(accountReadDto);
        }

        /// <summary>
        /// Creates user with "Standard" role
        /// </summary>
        /// <param name="accountCreateDto">Object with information about new user</param>
        /// <response code="200">Returns token and expiration timestamp</response>
        /// <response code="400">Failed to create account</response>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccountTokenDto>> CreateStandardUser(AccountCreateDto accountCreateDto)
        {
            var user = _mapper.Map<AccountCreateDto, AppUser>(accountCreateDto);
            user.Banned = false;
            var userDb = await _userManager.FindByEmailAsync(accountCreateDto.Email);
            if (userDb != null)
                throw new DowiezPlException("Failed to create account.") { StatusCode = 400 };

            var result = await _userManager.CreateAsync(user, accountCreateDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Standard");
                return Ok(await BuildToken(new AccountLoginDto() {
                    Email = accountCreateDto.Email,
                    Password = accountCreateDto.Password
                }));
            }
            else
            {
                throw new DowiezPlException("Failed to create account.") { StatusCode = 400, Detail = JsonConvert.SerializeObject(result.Errors) };
            }
        }

        /// <summary>
        /// Creates user with "Moderator" role
        /// </summary>
        /// <param name="accountCreateDto">Object with information about new user</param>
        /// <response code="200">Returns token and expiration timestamp</response>
        /// <response code="400">Failed to create account</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User not authorized</response>
        [HttpPost("create/moderator")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccountTokenDto>> CreateModeratorUser(AccountCreateDto accountCreateDto)
        {
            var user = _mapper.Map<AccountCreateDto, AppUser>(accountCreateDto);
            user.Banned = false;
            var userDb = await _userManager.FindByEmailAsync(accountCreateDto.Email);
            if (userDb != null)
                throw new DowiezPlException("Failed to create account.") { StatusCode = 400 };

            var result = await _userManager.CreateAsync(user, accountCreateDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Moderator");
                return Ok(await BuildToken(new AccountLoginDto() {
                    Email = accountCreateDto.Email,
                    Password = accountCreateDto.Password
                }));
            }
            else
            {
                throw new DowiezPlException("Failed to create account.") { StatusCode = 400, Detail = JsonConvert.SerializeObject(result.Errors) };
            }
        }

        /// <summary>
        /// Returns token after successfull authentication
        /// </summary>
        /// <param name="accountLoginDto">Object with login informations</param>
        /// <response code="200">Returns token and expiration timestamp</response>
        /// <response code="400">Invalid login attempt</response>
        /// <response code="423">User is banned</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        public async Task<ActionResult<AccountTokenDto>> Login(AccountLoginDto accountLoginDto)
        {
            var user = await _userManager.FindByEmailAsync(accountLoginDto.Email);
            if (user == null)
                throw new InvalidLoginAttemptException();

            var result = await _signInManager.CheckPasswordSignInAsync(user, accountLoginDto.Password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                ThrowIfBanned(user);
                return Ok(await BuildToken(accountLoginDto));
            }
            else
                throw new InvalidLoginAttemptException();
        }
        
        /// <summary>
        /// Returns token after successfull authentication
        /// </summary>
        /// <response code="200">Returns token and expiration timestamp</response>
        /// <response code="401">User not authenticated</response>
        [HttpPost("renewtoken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        public async Task<ActionResult<AccountTokenDto>> RenewToken()
        {
            var accountLoginDto = new AccountLoginDto()
            {
                Email = HttpContext.User.Identity.Name
            };

            return Ok(await BuildToken(accountLoginDto));
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
        public async Task<ActionResult> BlockUser(string userId, bool status)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new DowiezPlException("Failed to set banned property.");
            user.Banned = status;
            await _userManager.UpdateAsync(user);

            return NoContent();
        }
        
        private async Task<AccountTokenDto> BuildToken(AccountLoginDto accountLoginDto)
        {
            var user = await _userManager.FindByNameAsync(accountLoginDto.Email);
            ThrowIfBanned(user);
            
            return await BuildToken(user);
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