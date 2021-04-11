using BookStore.API.Contracts;
using BookStore.API.Data;
using BookStore.API.Mappings;
using BookStore.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BookStore.API
{
    // MIDDLEWARE
    // We selected "Web App (with Razor Pages)" as project type to take advantage of 'Individual user Account' option for authentication.
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
                    Configuration.GetConnectionString("DefaultConnection"))); // Use SQL Server with Conn String from appsettings.json
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // CORS
            services.AddCors(o =>
            {
                o.AddPolicy("corsPolicy",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    });
            });

            // We use "AutoMapper" & its Extensions.DI [nuget pkg] for mapping Data entities with Data Models or DTOs (API abstractions)
            services.AddAutoMapper(typeof(Maps));

            // Nuget Swashbuckle.AspNetCore.Swagger,Gen,UI for auto-documenting API. NLog.extensions.logging pkg for logging.
            // Add Swagger service
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Book Store API",
                    Version = "v1",
                    Description = "This is an educational API for a Book Store"
                });

                var xfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; // XML documentation file
                var xpath = Path.Combine(AppContext.BaseDirectory, xfile);
                c.IncludeXmlComments(xpath);
            });

            // Scoped lifetime services are created once per client request (connection).
            // Transient lifetime services are created each time they're requested from the service container.
            // This lifetime works best for lightweight, stateless services.
            // Singleton lifetime services are created the first time they're requested
            // (or when Startup.ConfigureServices is run and an instance is specified with the service registration).
            // Every subsequent request uses the same instance.
            // If the app requires singleton behavior, allowing the service container to manage the service's lifetime is recommended.
            // Don't implement the singleton design pattern and provide user code to manage the object's lifetime in the class.

            // Add Singleton instance of Logger Service
            services.AddSingleton<ILoggerService, LoggerService>();

            // Add Repository
            services.AddScoped<IAuthorRepository, AuthorRepository>();

            //services.AddRazorPages();
            //APIs use Controllers (MVC), so rather than Razor PAges, we add those..
            services.AddControllers();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Store API"); // URL: /swagger/
                c.RoutePrefix = ""; // make swagger endpoint come up on startup
            });

            app.UseHttpsRedirection();

            // Static files (in wwwroot folder that was auto created) not needed, coz API isn't going to have a UI
            // Remove wwwroot, Areas, and Pages folders. Add Controllers folder and Add 'API Controller - R/W Actions'.
            // app.UseStaticFiles();

            app.UseCors("corsPolicy"); // defined in ConfigureServices above

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
