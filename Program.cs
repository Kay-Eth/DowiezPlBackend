using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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

                    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
                    if (! await roleManager.RoleExistsAsync("Standard"))
                        await roleManager.CreateAsync(new AppRole("Standard"));
                    if (! await roleManager.RoleExistsAsync("Moderator"))
                        await roleManager.CreateAsync(new AppRole("Moderator"));
                    if (! await roleManager.RoleExistsAsync("Admin"))
                        await roleManager.CreateAsync(new AppRole("Admin"));
                    
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    if (await userManager.FindByEmailAsync("admin@dowiez.pl") == null)
                    {
                        var user = new AppUser() {
                            Email = "admin@dowiez.pl",
                            UserName = "admin@dowiez.pl",
                            FirstName = "Admin",
                            LastName = "Admin",
                            Banned = false,
                            EmailConfirmed = true
                        };
                        await userManager.CreateAsync(user, "DefaultPassword@123");
                        await userManager.AddToRoleAsync(user, "Admin");
                        await userManager.AddClaimAsync(user, new Claim("Banned", "false"));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment == "VPS")
            {
                return Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.ConfigureKestrel(options => {
                            options.ConfigureHttpsDefaults(co =>
                            {
                                co.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
                            });

                            var port = 5001;
                            var pfxFilePath = "/home/kayeth/DowiezPlBackendUpdate/certificate.pfx";
                            var pfxPassword = "DowiezPl1234@"; 

                            options.Listen(IPAddress.Any, port, listenOptions => {
                                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                                listenOptions.UseHttps(pfxFilePath, pfxPassword);
                            });
                        });

                        webBuilder.UseStartup<Startup>();
                    });
            }
            else
            {
                return Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    });
            }
        }
    }
}
