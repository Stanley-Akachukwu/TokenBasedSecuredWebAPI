using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Security.Claims;
using System.Text;
using UCare.Models.AppSettingsModel;
using UCare.Models.Mappings;
using UCare.WebAPI.CustomExceptionMiddleware;
using UCare.WebAPI.Data;
using UCare.WebAPI.Data.Abstract;
using UCare.WebAPI.Data.Concrete;

namespace UCare.WebAPI
{
    public class Startup
    {
      //  private readonly IOptions<AuthSettings> _authSettings;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
           // _authSettings = authSettings;
        }

        public IConfiguration Configuration
        {
            get;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddMvcCore().AddAuthorization().AddJsonFormatters();
            //database context
            services.AddDbContext<UCareDbContext>(options => options.UseSqlServer(Configuration["Data:UcareConnection:ConnectionString"]));
            services.AddDbContext<UcareIdentityDbContext>(options => options.UseSqlServer(Configuration["Data:UcareIdentity:ConnectionString"]));

            services.AddAutoMapper(typeof(MappingProfile));
           
            services.AddControllers();
            services.AddRazorPages();
            //Identity
            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<UcareIdentityDbContext>()
                .AddDefaultTokenProviders();
            //Authentication
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JWTSettings:Audience"],
                    ValidIssuer = Configuration["JWTSettings:Issuer"],
                    RequireExpirationTime = true,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTSettings:SecreteKey"])),
                    ValidateIssuerSigningKey = true
                };
            });
            //Authorization
            
            services.AddAuthorization(config =>
            {
                config.AddPolicy(Policies.IsUser, policy =>
                    policy.RequireClaim(ClaimTypes.Role, Policies.User));

                config.AddPolicy(Policies.IsAdmin, policy =>
                   policy.RequireClaim(ClaimTypes.Role, Policies.Admin));

                config.AddPolicy(Policies.IsAdminView, policy =>
                  policy.RequireClaim(ClaimTypes.Role, Policies.Admin)
                  .RequireClaim(Policies.ViewUserList));

                config.AddPolicy(Policies.IsAdminAction, policy => policy
                .RequireClaim(ClaimTypes.Role, Policies.Admin)
                 .RequireClaim(Policies.DeleteRoleAction)
                 .RequireClaim(Policies.CreateRoleAction)
                 .RequireClaim(Policies.EditRoleAction));
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddTransient<IMailRepository, SendGridMailRepository>(); 
            services.AddScoped<ISettingsRepository, SettingsRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            // Register the Swagger Generator service. This service is responsible for genrating Swagger Documents.  : 
            // Note: Add this service at the end after AddMvc() or AddMvcCore().
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "UCare API",
                    Version = "v1",
                    Description = "This API is for UCare product.",
                    Contact = new OpenApiContact
                    {
                        Name = "Stanley Akachukwu",
                        Email = string.Empty,
                        Url = new Uri("https://stannedu.com/"),
                    },
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "UCare API V1");
                c.RoutePrefix = "swagger/ui"; 
            });
            app.UseSerilogRequestLogging();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.ConfigureCustomExceptionMiddleware();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}