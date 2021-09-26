using System.Threading.Tasks;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DowiezPlBackend.Policies
{
    public class NotBannedHandler : AuthorizationHandler<NotBannedRequirement>
    {
        private readonly UserManager<AppUser> _userManager;

        public NotBannedHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotBannedRequirement requirement)
        {
            if (context.User.Identity.Name == null)
            {
                
            }
            else
            {
                var user = await _userManager.FindByNameAsync(context.User.Identity.Name);
                if (user != null && !user.Banned)
                    context.Succeed(requirement);
            }
        }
    }
}