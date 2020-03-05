using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using KryptoKid.Models;
using KryptoKid.Services;
using Microsoft.AspNetCore.Http;
using Hangfire;
using KryptoKid.Controllers;

namespace KryptoKid
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
            //services.AddHttpClient();

            services.AddHangfire(configuration =>
            {
                configuration.UseSqlServerStorage("Server=.;Database=DayTraderDb;Trusted_Connection=True;");
            });

            services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("ConnectionString");
                //simple way to read connectionstring section of config file

                options.UseSqlServer(connectionString);
                //using sql database
            });

            services.AddScoped<IUserService, DefaultUserService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<AppDbContext>();

           // vs AddMvc???
            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            // vs UseMvc??
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            

            // Background job
            RecurringJob.AddOrUpdate<IUserService>("Update-Db", x => x.DbUpdate(),
                "0 */15 * ? * *"); 
        }




    }
}
