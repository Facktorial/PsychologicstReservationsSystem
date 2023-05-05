using DataLayer;
using DataLayer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Globalization;
using WebApp.Models;
using System.Resources;
using System.Collections;
using Microsoft.Extensions.Options;

namespace WebApp
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            DbConnectionString = configuration["myData:ConnectionString"];
            ResourcesPath = configuration["myData:ResourcesPath"];
            ResourceFiles = configuration["myData:ResourceFiles"];
        }
        public IConfiguration Configuration { get; }
        public string DbConnectionString { get; set; }
        public string ResourcesPath { get; set; }
        public string ResourceFiles { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            SqlConnector sql = new SqlConnector(DbConnectionString);

            services.AddScoped(provider => new PatientService(sql));
            services.AddScoped(provider => new ReservationService(sql));
            services.AddScoped(provider => new ConsultantService(sql));

            services.AddLocalization(options => options.ResourcesPath = ResourcesPath);

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("cs-CZ");
                options.SupportedCultures = new[] { new CultureInfo("cs-CZ"), new CultureInfo("en-US") };
                options.SupportedUICultures = new[] { new CultureInfo("cs-CZ"), new CultureInfo("en-US") };
                options.RequestCultureProviders.Insert(0, new CustomCultureProvider());
            });

            services.AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            // Add the ResourceManager as a service
            services.AddSingleton<ResourceManager>(serviceProvider =>
            {
                return new ResourceManager(ResourceFiles, typeof(Startup).Assembly);
            });

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddHttpContextAccessor();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAuthenticatedUser", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        return context.User.Identity.IsAuthenticated;
                    });
                });
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "reservation",
                    pattern: "/Reservation",
                    defaults: new { controller = "Reservation", action = "Index" })
                    .RequireAuthorization("RequireAuthenticatedUser");
            });

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
        }
    }

    class CustomCultureProvider : RequestCultureProvider
    {
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            Console.WriteLine("headers: " + httpContext.Request.Headers["User-Agent"]);
            bool isEdge = httpContext.Request.Headers["User-Agent"].Contains("Edg");
                //.First()
                //.IndexOf("chrome", StringComparison.InvariantCultureIgnoreCase) != -1;

            string culture = isEdge ? "cs-CZ" : "en-US";
            Console.WriteLine($"is Edge: {isEdge}: {culture}");


            return Task.FromResult(new ProviderCultureResult(culture));
        }
    }
}