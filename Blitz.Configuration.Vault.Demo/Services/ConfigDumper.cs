using Blitz.Configuration.Vault.Demo.Libs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Blitz.Configuration.Vault.Demo.Services
{
    /// <summary>
    /// Config Dumper
    /// </summary>
    public class ConfigDumper : IConfigDumper
    {
        private readonly ILogger _logger;
        private readonly IConfigurationRoot _config;

        private ConfigDumper() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="config">IConfigurationRoot</param>
        public ConfigDumper(ILogger<ConfigDumper> logger, IConfigurationRoot config)
        {
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Main Entry Point
        /// </summary>
        public void Run()
        {
            _logger.LogInformation("\nAssembly Info\n");
            var ai = AssemblyInfo();
            foreach (var kv in ai)
            {
                _logger.LogInformation("{0}: {1}", kv.Key, kv.Value);
            }

            _logger.LogInformation("\nConfiguration\n");
            foreach (var c in _config.AsEnumerable())
            {
                _logger.LogInformation("Key: {0}, Value: {1}", c.Key, c.Value);
            }
        }

        private List<KeyValuePair<string, string>> AssemblyInfo()
        {
            var results = new List<KeyValuePair<string, string>>();
            var propsToGet = new List<string>() { "AssemblyProductAttribute", "AssemblyCopyrightAttribute", "AssemblyCompanyAttribute", "AssemblyDescriptionAttribute", "AssemblyFileVersionAttribute" };
            var assembly = typeof(Program).Assembly;
            foreach (var attribute in assembly.GetCustomAttributesData())
            {
                if (propsToGet.Contains(attribute.AttributeType.Name))
                {
                    if (!attribute.TryParse(out string value))
                    {
                        value = string.Empty;
                    }
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        results.Add(new KeyValuePair<string, string>(attribute.AttributeType.Name, value));
                    }
                }
            }
            return results;
        }

    }
}
