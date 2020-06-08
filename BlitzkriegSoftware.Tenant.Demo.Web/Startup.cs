using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;


namespace BlitzkriegSoftware.Tenant.Demo.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Cultures
        /// </summary>
        public string[] Cultures
        {
            get
            {
                var setting = this.Configuration["AllowedLanguages"];
                if(string.IsNullOrWhiteSpace(setting))
                {
                    setting = "en-US";
                }
                var cultures = setting.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return cultures;
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => this.Configuration.Bind("AzureAd", options));

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(opts =>
            {
                var supportedCultures = new List<CultureInfo>();

                foreach(var c in this.Cultures)
                {
                    supportedCultures.Add(new CultureInfo(c));
                }
                
                opts.DefaultRequestCulture = new RequestCulture(this.Cultures[0]);

                // Formatting numbers, dates, etc.
                opts.SupportedCultures = supportedCultures;

                // UI strings that we have localized.
                opts.SupportedUICultures = supportedCultures;
            });


            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddRazorPages();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

                app.UseHsts();
            }
            app.UseHttpsRedirection();

            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(this.Cultures[0])
                .AddSupportedCultures(this.Cultures)
                .AddSupportedUICultures(this.Cultures);

            app.UseRequestLocalization(localizationOptions);

            // var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            // app.UseRequestLocalization(options.Value);

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
