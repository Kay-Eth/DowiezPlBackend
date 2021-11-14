using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Microsoft.EntityFrameworkCore;
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

        protected async Task<bool> IsAdmin(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            for (int i = 0; i < roles.Count; i++)
            {
                if (roles[i] == "Admin")
                    return true;
            }

            return false;
        }

        protected async Task<bool> IsModerator(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            for (int i = 0; i < roles.Count; i++)
            {
                if (roles[i] == "Admin" || roles[i] == "Moderator")
                    return true;
            }

            return false;
        }

        protected async Task<AppUser> GetMyUserAsync()
        {
            var userDb = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (userDb == null)
                return null;
            return userDb;
        }

        /// <summary>
        /// Returns all accounts.
        /// </summary>
        /// <response code="200">Returns object with user data</response>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Moderator,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountReadDto>>> GetAccounts()
        {
            var result = await _userManager.Users.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<AccountReadDto>>(result));
        }

        /// <summary>
        /// Returns banned accounts.
        /// </summary>
        /// <response code="200">Returns object with user data</response>
        [HttpGet("banned")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Moderator,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountLimitedReadDto>>> GetBlockedAccounts()
        {
            var result = await _userManager.Users.Where(u => u.Banned).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<AccountLimitedReadDto>>(result));
        }

        [HttpGet("moderators")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountModeratorsDto>>> GetModeratorAccounts()
        {
            var role = await _context.Roles.AsQueryable().FirstOrDefaultAsync(r => r.Name == "Moderator");
            var userIds = _context.UserRoles.AsQueryable().Where(r => r.RoleId == role.Id).Select(r => r.UserId);
            var result = await _context.Users.AsQueryable().Where(u => userIds.Contains(u.Id)).ToListAsync();
            
            return Ok(_mapper.Map<IEnumerable<AccountModeratorsDto>>(result));
        }
        
        /// <summary>
        /// Returns my account's information
        /// </summary>
        /// <response code="200">Returns object with user data</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="404">User not found</response>
        /// <response code="423">User is banned</response>
        [HttpGet("my")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "NotBanned")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountReadDto>> GetMyAccount()
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
        /// Returns account's information
        /// </summary>
        /// <response code="200">Returns object with user data</response>
        /// <response code="400">Cannot retrieve information about moderator accounts</response>
        /// <response code="403">Only admin can do it</response>
        /// <response code="404">Account not found</response>
        [HttpGet("{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Moderator,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AccountReadDto>> GetUserAccount(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound();
            bool isNotAdmin = !(await IsAdmin(await GetMyUserAsync()));

            var roles = await _userManager.GetRolesAsync(user);
            for (int i = 0; i < roles.Count; i++)
            {
                if (isNotAdmin && (roles[i] == "Moderator" || roles[i] == "Admin"))
                    return BadRequest(new ErrorMessage("Cannot retrieve information about moderator accounts", "AC_GUA_1"));
            }

            var result = _mapper.Map<AccountReadDto>(user);

            if (roles[0] == "Standard")
                result.Role = Enums.Role.Standard;
            else if (roles[0] == "Moderator")
                result.Role = Enums.Role.Moderator;
            else if (roles[0] == "Admin")
                result.Role = Enums.Role.Admin;
            else
                return BadRequest(new ErrorMessage($"Role {roles[0]} doesn't exist!"));
            
            return Ok(result);
        }

        /// <summary>
        /// Returns account's basic information
        /// </summary>
        /// <response code="200">Returns object with user data</response>
        /// <response code="400">Cannot retrieve information about moderator accounts</response>
        /// <response code="403">Only moderators can do it</response>
        /// <response code="404">Account not found</response>
        [HttpGet("{userId}/small")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AccountLimitedReadDto>> GetUserAccountSmall(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound();
            
            return Ok(_mapper.Map<AccountLimitedReadDto>(user));
        }

        /// <summary>
        /// Creates user with "Standard" role and sends confirmation email
        /// </summary>
        /// <param name="accountCreateDto">Object with information about new user</param>
        /// <response code="204">User created successfully</response>
        /// <response code="400">Failed to create account</response>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
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
                var email_confirmation_token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                await _mailService.SendEmailConfirmationAsync(accountCreateDto.Email, user.Id.ToString(), email_confirmation_token);

                return NoContent();
            }
            else
            {
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
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
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
        [HttpPost("ban/{userId}/{status}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Moderator,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> BanUser(string userId, bool status)
        {
            var me = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new ErrorMessage($"User with id {userId} not found.", "AC_BU_1"));
            
            if (await IsModerator(user) && !await IsAdmin(me))
                return Forbid();
            
            if (await IsAdmin(user))
                return BadRequest(new ErrorMessage("Cannot ban admin account.", "AC_BU_2"));

            if (status && !user.Banned)
            {
                await _mailService.SendBannedAsync(user.Email, me.Email, me.FirstName + " " + me.LastName, me.Id.ToString());
            }

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
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ConfirmEmail(AccountEmailConfirmationDto aecDto)
        {
            var user = await _userManager.FindByIdAsync(aecDto.UserId);
            var result = await _userManager.ConfirmEmailAsync(user, aecDto.Token);

            if (result.Succeeded)
                return NoContent();
            else
                return BadRequest(new ErrorMessage("Failed to confirm email."));
        }

        /// <summary>
        /// Generates and sends a password reset token to given email address (if exists in database)
        /// </summary>
        /// <param name="email">Email address</param>
        /// <response code="204">Request ended</response>
        [HttpPost("requestResetPassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> SendResetPasswordToken([Required] [EmailAddress] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var password_reset_token = await _userManager.GeneratePasswordResetTokenAsync(user);

                if (await IsModerator(user))
                    await _mailService.SendModeratorPasswordResetAsync(email, user.Id.ToString(), password_reset_token);
                else
                    await _mailService.SendPasswordResetAsync(email, user.Id.ToString(), password_reset_token);
            }

            return NoContent();
        }

        /// <summary>
        /// Performs an password update
        /// </summary>
        /// <param name="arpDto">Password reset data object</param>
        /// <response code="204">Password successfully changed</response>
        /// <response code="400">An error occured</response>
        [HttpPut("resetPassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ResetPassword(AccountResetPasswordDto arpDto)
        {
            var user = await _userManager.FindByIdAsync(arpDto.UserId.ToString());
            if (user == null)
                return BadRequest(new ErrorMessage("Password reset failed.", "AC_RP_1"));
            
            var result = await _userManager.ResetPasswordAsync(user, arpDto.Token, arpDto.Password);
            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(new ErrorMessage("Password reset failed.", result.Errors));
            }
        }

        /// <summary>
        /// Updates password
        /// </summary>
        /// <param name="aupDto">Change password data</param>
        /// <response code="204">Password successfully changed</response>
        /// <response code="400">Password change failed</response>
        [HttpPut("changePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "NotBanned")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdatePassword(AccountUpdatePasswordDto aupDto)
        {
            var userDb = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (userDb == null)
                return NotFound();
            
            var result = await _userManager.ChangePasswordAsync(userDb, aupDto.OldPassword, aupDto.NewPassword);
            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(new ErrorMessage("Failed to update password.", result.Errors));
            }
        }
        
        /// <summary>
        /// Updates account information
        /// </summary>
        /// <param name="audDto">Account's new informations</param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "NotBanned")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccountReadDto>> UpdateAccount(AccountUpdateDataDto audDto)
        {
            var userDb = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (userDb == null)
                return NotFound();
            
            userDb.FirstName = audDto.FirstName;
            userDb.LastName = audDto.LastName;
            
            var result = await _userManager.UpdateAsync(userDb);
            if (result.Succeeded)
            {
                return Ok(_mapper.Map<AccountReadDto>(userDb));
            }
            else
            {
                return BadRequest(new ErrorMessage("Failed to update account.", result.Errors));
            }
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

            var expiration = DateTime.UtcNow.AddDays(30);

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