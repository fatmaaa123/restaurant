using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NToastNotify;
using Restaurantt.Data;
using Restaurantt.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurantt
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
            {
                CloseButton = true,
                PositionClass=ToastPositions.BottomRight,
                PreventDuplicates=true

            });


            services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()


            
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddAuthentication()

            .AddGoogle(options =>                            //google+facebook ayar�!
             {
                 options.ClientId = Configuration["App:GoogleClientId"];
                 options.ClientSecret = Configuration["App:GoogleClientSecret"];
             });
            //.AddFacebook(options =>

            //{
            //    options.AppId = Configuration["App:FacebookClientId"];
            //    options.ClientSecret = Configuration["App:FacebookClientSecret"];
            //});


            services.AddSingleton<IEmailSender, EMailSender>();
            services.Configure<EMailOptions>(Configuration);// email i�in gerekli servisi a�t�m
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
            services.AddRazorPages();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{Area=Musteri}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
