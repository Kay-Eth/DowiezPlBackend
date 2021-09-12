using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DowiezPlBackend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = await Task.Run(() => CreateHostBuilder(args).Build());

            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    var services = scope.ServiceProvider;

                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    if (! await roleManager.RoleExistsAsync("Standard"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("Standard"));
                        await roleManager.CreateAsync(new IdentityRole("Moderator"));
                        await roleManager.CreateAsync(new IdentityRole("Admin"));

                        var userManager = services.GetRequiredService<UserManager<AppUser>>();
                        var user = new AppUser() { Email = "admin@dowiez.pl", UserName = "DefaultAdmin", FirstName = "Admin", LastName = "Admin", Banned = false };

                        await userManager.CreateAsync(user, "DefaultPassword@123");
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
