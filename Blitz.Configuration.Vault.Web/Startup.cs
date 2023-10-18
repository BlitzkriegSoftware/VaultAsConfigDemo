using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Blitz.Configuration.Vault.Library.Helpers;
using System.Reflection.PortableExecutable;

namespace Blitz.Configuration.Vault.Web
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
            // Logging
            services.AddLogging(loggingBuilder => {
                // This line must be 1st
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);

                // Console is generically cloud friendly
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            // Configuration
            var config = new ConfigurationBuilder()
                           .AddEnvironmentVariables()
                           .AddVault(ProgramLogger, this.Configuration)
                           .Build();

            services.AddSingleton(config);

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        #region "Program Logger"

        private ILogger _programlogger;

        public ILogger ProgramLogger
        {
            get {
                if (_programlogger == null) {
                    var loggerFactory = LoggerFactory.Create(builder =>
                    {
                        builder
                            .AddConsole()
                            .AddDebug();
                    });
                    _programlogger = loggerFactory.CreateLogger<Startup>();
                }
                return _programlogger;
            }
        }

        #endregion

    }
}
