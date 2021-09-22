using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MusicPlatform.Model.User;
using MusicPlatform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;

namespace MusicPlatform
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
            services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder("BasicAuthentication")
                .RequireAuthenticatedUser()
                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddDbContext<IdentityDb>(opts =>
            {
                opts.UseSqlServer(Configuration["ConnectionStrings:MusicPlatform"]).EnableSensitiveDataLogging();
            });
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequiredLength = 4;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30);
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("AdeDowloadAPISpecification", new OpenApiInfo()
                {
                    Title = "AdeDownload API",
                    Version = "1.0",
                    Description = "Download Music for Free",
                    Contact = new OpenApiContact()
                    {
                        Email = "adeolaaderibigbe09@gmail.com",
                        Name = "Adeola Aderibigbe",
                        Url = new Uri("https://github.com/Adexandria")

                    }

                });
                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                   
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "Basic",
                    Description = "Enter Token Only"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "basic"
                                }
                            },
                            new string[] {}
                    }
                });
            });
           
            services.AddIdentity<UserModel, IdentityRole>().AddEntityFrameworkStores<IdentityDb>().AddSignInManager().AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
               // options.Cookie.Expiration = new TimeSpan(1,10,10,100);
                options.LoginPath = PathString.Empty;
                options.AccessDeniedPath = PathString.Empty;

            });
            services.AddAuthentication(AzureADDefaults.OpenIdScheme).AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", options => { })
               .AddAzureAD(opt=> Configuration.Bind("AzureAd", opt));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("BasicAuthentication", new AuthorizationPolicyBuilder("BasicAuthentication").RequireAuthenticatedUser().Build());
                
            });
          
        }
      

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                if (env.IsDevelopment())
                {
                app.UseDeveloperExceptionPage();
                }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint("/swagger/AdeDowloadAPISpecification/swagger.json", "AdeDowload API");
                setupAction.RoutePrefix = string.Empty;
            });
            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            }
    }
}
