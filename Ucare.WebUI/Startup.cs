using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.Toast;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ucare.WebUI.Services.Abstract;
using Ucare.WebUI.Services.Concrete;
using UCare.Models.AppSettingsModel;
using UCare.Models.Mappings;

namespace Ucare.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredModal();
            services.AddBlazoredLocalStorage();
            services.AddOptions();
            services.AddAuthenticationCore();
            services.AddScoped<AuthenticationStateProvider, LocalAuthenticationStateProvider>();

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
            services.AddHttpClient<IUserService, UserService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["APIBaseUrl"]);
            });
            services.AddHttpClient<IRoleService, RoleService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["APIBaseUrl"]);
            });
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddBlazoredToast();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.  :  : 
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           // Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzU1NzU2QDMxMzgyZTMzMmUzMG1xazl3a1VyQlpxY1VQTXhEWDNxMlI3S1pvanljNml2TEEzYi9FRGJUMTg9");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
               //.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
