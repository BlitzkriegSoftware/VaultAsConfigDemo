using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CommandLine;
using Blitz.Configuration.Vault.Demo.Services;
using CommandLine.Text;

namespace Blitz.Configuration.Vault.Demo
{
    class Program
    {
        #region "Standard Shell Fields"
        static int exitCode = 0; // Zero is good, not zero is bad
        #endregion

        #region "Properties"
        static Library.Models.VaultConfiguration vaultConfiguration;
        #endregion

        static void Main(string[] args)
        {
            #region "Global error handler"
            // Notice a not handled exception from UOW will be caught by global error handler
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            #endregion

            Console.WriteLine($"{HeadingInfo.Default} {CopyrightInfo.Default}");

            Parser.Default.ParseArguments<Models.CommandLineOptions>(args)
                   .WithParsed<Models.CommandLineOptions>(o =>
                   {
                       var arguments = CommandLine.Parser.Default.FormatCommandLine(o);
                       Console.WriteLine($"{arguments}");

                       if (o.Dump)
                       {
                           var configDumper = Services.GetService<IConfigDumper>();
                           configDumper.Run();
                       }

                       vaultConfiguration = new Library.Models.VaultConfiguration()
                       {
                           Application = string.IsNullOrWhiteSpace(o.Application) ? Environment.GetEnvironmentVariable("application") : o.Application,
                           
                           EnvironmentPath = string.IsNullOrWhiteSpace(o.Environment) ? Environment.GetEnvironmentVariable("environment") : o.Environment,
                           
                           RootPath = string.IsNullOrWhiteSpace(o.RootPath) ? Environment.GetEnvironmentVariable("vaultrootpath") : o.RootPath,
                           
                           Token = string.IsNullOrWhiteSpace(o.Token) ? Environment.GetEnvironmentVariable("vaultrootpath") : o.Token,
                           
                           Url = string.IsNullOrWhiteSpace(o.VaultUrl) ? Environment.GetEnvironmentVariable("vaulturl") : o.VaultUrl
                       };

                       if(!vaultConfiguration.IsValid)
                       {
                           Console.WriteLine($"Configuration is invalid: {vaultConfiguration}, Token: {(string.IsNullOrWhiteSpace(vaultConfiguration.Token)? 'n':'y')}");
                           exitCode = -3;
                       } else
                       {
                           //var helper = new VaultHelper(programLogger, vaultConfig);
                           //var d = helper.SettingsGet(My_Application, My_Environment);
                           //foreach (var nv in d)
                           //{
                           //    programLogger.LogInformation($"{nv.Key}: {nv.Value}");
                           //}
                       }
                   })
                   .WithNotParsed(errs =>
                   {
                       foreach(var e in errs)
                       {
                           Console.WriteLine($"Error: {e.Tag}");
                       }
                       exitCode = -1;
                   });

            Environment.ExitCode = exitCode;
        }

        #region "Global Error Handler"

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = new ApplicationException("Unhandled exception caused crash, please see logs");
            if ((e != null) && (e.ExceptionObject != null))
            {
                if (e.ExceptionObject is Exception ex2)
                {
                    ex = ex2;
                }
            }

            Console.Error.WriteLine($"CurrentDomain_UnhandledException: {ex.Message}");
            exitCode = -1; // unhandled exception
        }

        #endregion

        #region "Builder"

        private static IServiceProvider _services;

        public static IServiceProvider Services
        {
            get
            {
                if (_services == null)
                {
                    // Create service collection
                    var serviceCollection = new ServiceCollection();

                    // Build DI Stack inc. Logging, Configuration, and Application
                    ConfigureServices(serviceCollection);

                    // Create service provider
                    _services = serviceCollection.BuildServiceProvider();
                }
                return _services;
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Logging
            services.AddLogging(loggingBuilder => {
                // This line must be 1st
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);

                // Console is generically cloud friendly
                loggingBuilder.AddConsole();
            });

            // Configuration
            var config = new ConfigurationBuilder()
                           .AddEnvironmentVariables()
                           .Build();

            services.AddSingleton(config);

            // App to run
            services.AddTransient<IConfigDumper, ConfigDumper>();
        }
        #endregion


    }
}
