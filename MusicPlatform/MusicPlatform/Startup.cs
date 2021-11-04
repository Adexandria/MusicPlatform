using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MusicPlatform.Model.User;
using MusicPlatform.Services;
using System;
using Microsoft.AspNetCore.Mvc.Authorization;
using Text_Speech.Services;
using Azure.Storage.Blobs;
using Azure.Core.Extensions;
using Microsoft.Extensions.Azure;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

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
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });
            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });
            services.AddScoped<IUser, UserDetail>();
            services.AddScoped<IUserProfile, UserProfileRepository>();
            services.AddScoped<ISong, SongRepository>();
            services.AddScoped<IBlob, Blob>();
            services.AddScoped<ILibrary, LibraryRepository>();
            services.AddDbContext<IdentityDb>(opts =>
            {
                opts.UseSqlServer(Configuration["ConnectionStrings:MusicPlatform"], o => o.EnableRetryOnFailure()).EnableSensitiveDataLogging();
            });
            services.AddDbContext<DataDb>(opts =>
            {
                opts.UseSqlServer(Configuration["ConnectionStrings:MusicPlatform"],o=> o.EnableRetryOnFailure()).EnableSensitiveDataLogging();
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
                c.SwaggerDoc("v1", new OpenApiInfo()
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
                
                c.SwaggerDoc("v2", new OpenApiInfo() 
                {
                    Title = "AdeDownload API",
                    Version = "2.0",
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
                options.Cookie.Name = "DownloadMusic";
                options.ExpireTimeSpan = new TimeSpan(1, 0, 0, 0);
                options.LoginPath = PathString.Empty;   
                options.AccessDeniedPath = PathString.Empty;

            });
            services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", options => { });
                
            services.AddAuthorization(options =>
            {
                options.AddPolicy("BasicAuthentication", new AuthorizationPolicyBuilder("BasicAuthentication").RequireAuthenticatedUser().Build());
                
            });
            services.AddScoped(_ => {
                return new BlobServiceClient(Configuration.GetConnectionString("AzureBlobStorage"));
            });
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(Configuration["ConnectionStrings:AzureBlobStorage:blob"], preferMsi: true);
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataDb context, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseSwagger();

            app.UseSwaggerUI(setupAction =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    setupAction.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
                setupAction.RoutePrefix = string.Empty;
            });
            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            context.Database.EnsureCreated();
            
         }
        }
    internal static class StartupExtensions
    {
        public static IAzureClientBuilder<BlobServiceClient, BlobClientOptions> AddBlobServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri serviceUri))
            {
                return builder.AddBlobServiceClient(serviceUri);
            }
            else
            {
                return builder.AddBlobServiceClient(serviceUriOrConnectionString);
            }
        }
        public static IAzureClientBuilder<QueueServiceClient, QueueClientOptions> AddQueueServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri serviceUri))
            {
                return builder.AddQueueServiceClient(serviceUri);
            }
            else
            {
                return builder.AddQueueServiceClient(serviceUriOrConnectionString);
            }
        }
    }
}
 

