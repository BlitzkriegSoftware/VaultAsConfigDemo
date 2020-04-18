using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Blitz.Configuration.Vault.Demo.Models
{
    /// <summary>
    /// Models: Command Line Options
    /// </summary>
    public class CommandLineOptions
    {
        [Option('u',"VaultUrl", Required =false, HelpText = "Vault Url (env: 'vaulturl')")]
        public string VaultUrl { get; set; }

        /// <summary>
        /// JSON Configuration
        /// </summary>
        [Option('t', "Token", Required = false, HelpText = "Vault Token (env: 'vaulttoken')")]
        public string Token { get; set; }

        /// <summary>
        /// Application
        /// </summary>
        [Option('a', "App", Required = false, HelpText = "Application Name (env: 'application')")]
        public string Application { get; set; }

        /// <summary>
        /// Environment
        /// </summary>
        [Option('e', "Env", Required = false, HelpText = "Environment (env: 'environment')")]
        public string Environment { get; set; }

        /// <summary>
        /// Dump all configuration
        /// </summary>
        [Option('d', "Dump", Required = false, HelpText = "Dump All Read Configuration")]
        public bool Dump { get; set; }

        /// <summary>
        /// Root Path
        /// </summary>
        [Option('r', "Root Path", Required =false, HelpText = "RootPath e.g. 'v1/secret/data'  (env: 'vaultrootpath')", Default = "v1/secret/data")]
        public string RootPath { get; set; }

        /// <summary>
        /// Help
        /// </summary>
        [Usage(ApplicationAlias = "VaultDemo")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example("From Environment Variables", new CommandLineOptions { }),
                    new Example("Vary Environment", new CommandLineOptions { Environment = "dev", Application = "myApp", Token = "s3cr3t-fake"  }),
                    new Example("Full command line", new CommandLineOptions { Application = "myApp", Environment = "dev", Dump = true, Token = "s3cr3t-fake", VaultUrl = "http://localhost:8200", RootPath= "v1/secret/data" })
                };
            }
        }

    }
}
