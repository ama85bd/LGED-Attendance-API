using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LGED.Core.Constants;
using LGED.Core.Helper;
using LGED.Core.Interfaces;
using LGED.Data.Base;
using LGED.Domain.Base;
using LGED.Model.Common;
using LGED.Model.Context;
using LGED.Model.Entities.Profile;
using LGED.Web.Extensions;
using LGED.Web.Middleware;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace API
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //cache for session and roles
            services.AddDistributedMemoryCache();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //add cross origin rules

            services.AddCors(opt => 
            {
                opt.AddPolicy("CorsPolicy", policy => 
                {
                    policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000");
                });
            });

            // services.AddCors(opt =>
            // {
            //     opt.AddPolicy("CorsPolicy", policy =>
            //     {
            //         policy.AllowAnyHeader()
            //             .AllowAnyMethod()
            //             .WithExposedHeaders("WWW-Authenticate")
            //             .WithOrigins(Configuration.GetSection("CorsOrigins").Get<string[]>())
            //             .AllowCredentials();
            //     });
            // });

            // services.AddControllers();
            services.AddMediatR(typeof(CommandBase<>).Assembly);

            //add request culture
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("ms-MY");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US"), new CultureInfo("ms-MY") };
            });

            //add slug transformation
            services.AddMvc(options =>
                {
                    //slugify the name of controller
                    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                    //due to .net core 3.1
                    //if return Ok(null) will received an empty response
                    //that couldn't be wrapped normally by ApiWrapper
                    //so remove the "no-content" output type
                    options.OutputFormatters.RemoveType<StringOutputFormatter>();
                    options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();

                    options.CacheProfiles.Add("default", new CacheProfile
                    {
                        NoStore = true,
                        Location = ResponseCacheLocation.None
                    });
                }).AddNewtonsoftJson(options =>
                {
                    //exclude looping values
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    //for serialize from JSON DateObject <-> C# DateTime without timezone data (similar to DateTimeOffset.ToLocalTime)
                    //any C# model has DateTime will be applied
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                })
                .AddRazorRuntimeCompilation();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IUserContext, UserContext>();

            //add swagger
            services.ConfigureSwagger();

            // //add automapper
            services.ConfigureAutoMapper();
            
            var connectionString =
                Configuration["ConnectionStrings:DefaultConnection"]; // => load connection string from secrets
            services.AddDbContext<LgedDbContext>(c => c.UseSqlServer(connectionString));

            
            //add identity
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<LgedDbContext>()
                .AddUserStore<LgedUserRoleStore>()
                .AddUserManager<LgedUserManager>()
                .AddSignInManager<SignInManager<User>>()
                .AddDefaultTokenProviders();

            //add authentication  
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                //add jwt
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    var tokenValidatorParams = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = Configuration[AppConfigs.InternalIdServer.ValidAudience],
                        ValidIssuer = Configuration[AppConfigs.InternalIdServer.ValidIssuer],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(Configuration[AppConfigs.InternalIdServer.Secret]))
                    };
                    options.TokenValidationParameters = tokenValidatorParams;
                    //SignalR: transmit the access-token in the query string to the request context for authentication with jwt
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Path.StartsWithSegments(AppConstants.PluginUrl.Hub))
                            {
                                var accessToken = context.Request.Query["access_token"];

                                if (!string.IsNullOrEmpty(accessToken))
                                {
                                    context.Token = accessToken;
                                }
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

                
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "LGED-API-v1", Version = "v1" });
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseSwagger();
                // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
            }
else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("CorsPolicy");

            // //use resource static files such as images, css, js...
            app.UseStaticFiles();
            app.UseCookiePolicy();
            
            app.UseAuthentication();

            // Enable custom middle ware for wrapping response, binding user context, and handling exception
            // Must placed after authentication and before authorization to bind user context
            app.UseApiWrapperMiddleware(new ApiWrapperMiddlewareOptions
            {
                //exclude these paths
                ExcludePaths = new List<string>
                {
                    AppConstants.PluginUrl.Swagger,
                    AppConstants.PluginUrl.HangFire,
                    AppConstants.PluginUrl.Manager,
                    AppConstants.PluginUrl.Attachment,
                    AppConstants.PluginUrl.Hub
                },
                Version = "1.0a"
            });

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(AppConstants.PluginUrl.Swagger + "/v1/swagger.json", "LGED API V1");
                //c.OAuthAppName("");
                c.EnableFilter();
                c.DefaultModelRendering(ModelRendering.Model);
                c.DisplayRequestDuration();
                //inject css for swagger ui
                //c.InjectStylesheet("/css/swagger.css");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
