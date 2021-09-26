using System.Threading.Tasks;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "NotBanned")]
    public class DowiezPlControllerBase : ControllerBase
    {
        protected readonly UserManager<AppUser> _userManager;

        public DowiezPlControllerBase(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        protected async Task<bool> UserExists(string userId)
        {
            return await AccountsController.CheckIfExistsAsync(userId, _userManager);
        }

        protected async Task<AppUser> GetMyUserAsync()
        {
            var userDb = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (userDb == null)
                return null;
            return userDb;
        }

        protected async Task<AppUser> GetUserAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        protected async Task<bool> IsModerator(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            for (int i = 0; i < roles.Count; i++)
            {
                if (roles[i] == "Moderator" || roles[i] == "Admin")
                    return true;
            }

            return false;
        }
    }
}