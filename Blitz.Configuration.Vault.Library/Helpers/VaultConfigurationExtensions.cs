using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Blitz.Configuration.Vault.Library.Helpers
{
    /// <summary>
    /// Vault Configuration Extensions
    /// </summary>
    public static class VaultConfigurationExtensions
    {
        /// <summary>
        /// Use hashicorp vault as a configuration source for application settings
        /// </summary>
        /// <param name="builder">(this) IConfigurationBuilder</param>
        /// <param name="logger">(nullable) ILogger</param>
        /// <param name="configuration">IConfiguration</param>
        /// <returns>IConfigurationBuilder</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IConfigurationBuilder AddVault(this IConfigurationBuilder builder, ILogger logger, IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var vaultConfig = new Models.VaultConfiguration();
            
            foreach (var c in configuration.AsEnumerable())
            {
                vaultConfig.ParseConfigurationField(c.Key, c.Value);
            }

            if (!vaultConfig.IsValid) throw new ArgumentException("The configuration for the client is not valid", nameof(vaultConfig));

            var kvList = new List<KeyValuePair<string, string>>();

            var helper = new VaultConfigClient(logger, vaultConfig);

            var d = helper.SettingsGet();

            foreach (var kv in d)
            {
                kvList.Add(new KeyValuePair<string, string>(kv.Key, kv.Value));
            }

            logger?.LogDebug($"Vault Configuration: {kvList.Count} values added.");

            builder.AddInMemoryCollection(kvList);
            return builder;
        }
    }
}
