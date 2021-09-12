using System.Threading.Tasks;
using DowiezPlBackend.Exceptions;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DowiezPlControllerBase : ControllerBase
    {
        protected readonly UserManager<AppUser> _userManager;

        public DowiezPlControllerBase(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        protected async Task CheckUser()
        {
            var userDb = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (userDb == null)
                throw new UserNotFoundException(HttpContext.User.Identity.Name);
            AccountsController.ThrowIfBanned(userDb);
        }
    }
}