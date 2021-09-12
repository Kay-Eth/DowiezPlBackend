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
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DowiezPlBackend.Controllers
{
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

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AccountReadDto>> GetAccount()
        {
            var userDb = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (userDb == null)
                return BadRequest();
            
            var accountReadDto = _mapper.Map<AppUser, AccountReadDto>(userDb);
            var role = (await _userManager.GetRolesAsync(userDb))[0];
            if (role == "Standard")
                accountReadDto.Role = Enums.Role.Standard;
            else if (role == "Moderator")
                accountReadDto.Role = Enums.Role.Moderator;
            else if (role == "Admin")
                accountReadDto.Role = Enums.Role.Admin;
            else
                throw new System.ArgumentException("role");
            
            return Ok(accountReadDto);
        }

        [HttpPost("create")]
        public async Task<ActionResult<AccountTokenDto>> CreateStandardUser(AccountCreateDto accountCreateDto)
        {
            var user = _mapper.Map<AccountCreateDto, AppUser>(accountCreateDto);
            user.Banned = false;
            var result = await _userManager.CreateAsync(user, accountCreateDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Standard");
                return await BuildToken(new AccountLoginDto() {
                    Email = accountCreateDto.Email,
                    Password = accountCreateDto.Password
                });
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("create/moderator")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<AccountTokenDto>> CreateModeratorUser(AccountCreateDto accountCreateDto)
        {
            var user = _mapper.Map<AccountCreateDto, AppUser>(accountCreateDto);
            user.Banned = false;
            var result = await _userManager.CreateAsync(user, accountCreateDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Moderator");
                return await BuildToken(new AccountLoginDto() {
                    Email = accountCreateDto.Email,
                    Password = accountCreateDto.Password
                });
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AccountTokenDto>> Login(AccountLoginDto accountLoginDto)
        {
            var user = await _userManager.FindByNameAsync(accountLoginDto.Email);
            if (user == null)
                return BadRequest("Invalid login attempt 1");

            var result = await _signInManager.CheckPasswordSignInAsync(user, accountLoginDto.Password, lockoutOnFailure: false);

            Console.WriteLine($"Is Locked: {user.LockoutEnabled}");

            if (result.Succeeded)
                return await BuildToken(accountLoginDto);
            else
                return BadRequest("Invalid login attempt 2");
        }

        [HttpPost("renewtoken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AccountTokenDto>> RenewToken()
        {
            var accountLoginDto = new AccountLoginDto()
            {
                Email = HttpContext.User.Identity.Name
            };

            return await BuildToken(accountLoginDto);
        }

        [HttpPost("block/{userId}/{status}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Moderator,Admin")]
        public async Task<ActionResult> BlockUser(string userId, bool status)
        {
            var user = await _userManager.FindByIdAsync(userId);
            user.Banned = status;
            await _userManager.UpdateAsync(user);

            return NoContent();
        }
        
        private async Task<AccountTokenDto> BuildToken(AccountLoginDto accountLoginDto)
        {
            var user = await _userManager.FindByNameAsync(accountLoginDto.Email);
            
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