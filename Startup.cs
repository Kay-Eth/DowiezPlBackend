using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DowiezPlBackend.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System.Net.Mime;

namespace DowiezPlBackend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DowiezPlDbContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(
                        Configuration.GetConnectionString("Connection"),
                        new MariaDbServerVersion(new Version(10, 5, 12))
                    )
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
            );

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<DowiezPlDbContext>()
                .AddDefaultTokenProviders();
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(Configuration["jwt:key"])
                        ),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddControllers()
                // .ConfigureApiBehaviorOptions(options => 
                // {
                //     options.InvalidModelStateResponseFactory = context =>
                //     {
                //         var result = new BadRequestObjectResult(context.ModelState);

                //         // TODO: add `using System.Net.Mime;` to resolve MediaTypeNames
                //         result.ContentTypes.Add(MediaTypeNames.Application.Json);
                //         result.ContentTypes.Add(MediaTypeNames.Application.Xml);

                //         return result;
                //     };
                // })
                .AddNewtonsoftJson(
                    s => {
                        s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        s.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                        s.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                        s.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    }
                );
            
            services.AddCors(options =>
            {
                options.AddPolicy("AllowDevClient",
                  builder =>
                  {
                      builder
                      .AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
                  });
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<IDowiezPlRepository, SqlDowiezPlDatabase>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "DowiezPlBackend",
                    Version = "v1"
                });
                c.AddSecurityDefinition("jwt_auth", new OpenApiSecurityScheme()
                {
                    Name = "Bearer",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Description = "Specify authorization token.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "jwt_auth"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            services.AddSwaggerGenNewtonsoftSupport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("Server"))
            {
                // app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/error-dev");
                
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DowiezPlBackend v1"));

            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
