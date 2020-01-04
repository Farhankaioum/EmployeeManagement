using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement
{
    public class Startup
    {
        private IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940


        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));

            //for identity registration and override PasswordOptions class properties
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {

                //for account logout retry
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                //for email confirming ensuring registration
                options.SignIn.RequireConfirmedEmail = true;
                //for passwordOptions class prop
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                //add token service
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

               
            }).
                AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");


            // add token provider lifetime/lifespan registration
            services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(5));
            //registration custom token provider service
            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromDays(3));

            //ways to override  build in PasswordOptions class 

            //services.Configure<IdentityOptions>(options =>
            //{
            //    options.Password.RequiredLength = 10;
            //    options.Password.RequiredUniqueChars = 3;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequireLowercase = false;
            //});


            //register access denied controller and view
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
               
            });

            //add or registration claims based authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role", "true"));
                //custom authorization handler call
                options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));

                //options.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role", "true"));
                //options.AddPolicy("SuperPowerPolicy", policy => policy.RequireAssertion(context =>
                //            context.User.IsInRole("Admin") && context.User.HasClaim(
                //                claim => claim.Type == "EditRolePolicy" && claim.Value == "true") ||
                //                context.User.IsInRole("Super Admin")));
            });
            

            //Globally Authenticate users, start from options =>
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();

            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();

            // registration custom authorization requirement
            services.AddTransient<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHander>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
            services.AddSingleton<DataProtectionPurposeStrings>();


            //provide external authentication registration like google, facebook, twitter etc.
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "222931360616-haq6oho8jtbl5ommjs6for416lp73khe.apps.googleusercontent.com";
                options.ClientSecret = "7-sBuc3_ZOYYBihxyoDAwXWW";
            })
            .AddFacebook(options =>
            {
                options.AppId = "518784335402886";
                options.AppSecret = "1d1691fec19bdd145d35b791417e8c4b";
            });

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseExceptionHandler("/Error");
            }

            //app.UseStatusCodePagesWithReExecute("/Error/{0}");
            //app.UseExceptionHandler("/Error");

            app.UseStaticFiles();
            //register authentication for identity
            app.UseAuthentication();
            //app.UseMvcWithDefaultRoute();

            app.UseMvc(routes =>
           {
               routes.MapRoute(
                   name: "dafault",
                   template: "{controller=Home}/{action=index}/{id?}");
           });   




            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});


        }
    }
}
