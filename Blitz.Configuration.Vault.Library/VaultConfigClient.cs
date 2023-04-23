using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Blitz.Configuration.Vault.Library.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Blitz.Configuration.Vault.Library
{
    /// <summary>
    /// Hashicorp Vault Client For Configuration
    /// </summary>
    public class VaultConfigClient
    {
        #region "Constants"
        private const string TOKEN_HEADER = "X-Vault-Token";
        private const string CONTENT_TYPE = "application/json";
        private const string RENEW_PATH = "v1/auth/token/renew-self";
        #endregion

        #region "Properties"
        private readonly ILogger _logger;
        private readonly Models.VaultConfiguration _config;
        private readonly IHttpClientFactory _factory;
        #endregion

        #region "CTOR"
        private VaultConfigClient() { }

        /// <summary>
        /// CTOR (Projects w/o an <c>IHttpClientFactory)</c>
        /// <para>Will build a <c>IHttpClientFactory</c></para>
        /// </summary>
        /// <param name="logger">(nullable) ILogger</param>
        /// <param name="vaultConfig">VaultConfiguration</param>
        public VaultConfigClient(ILogger logger, Models.VaultConfiguration vaultConfig)
        {
            _logger = logger;
            _config = vaultConfig;

            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
            _factory = serviceProvider.GetService<IHttpClientFactory>();
        }

        /// <summary>
        /// CTOR (Web and other DI Projects)
        /// </summary>
        /// <param name="logger">(nullable) ILogger</param>
        /// <param name="vaultConfig">VaultConfiguration</param>
        /// <param name="factory">IHttpClientFactory</param>
        public VaultConfigClient(ILogger logger, VaultConfiguration vaultConfig, IHttpClientFactory factory) : this(logger, vaultConfig)
        {
            _factory = factory;
        }

        #endregion

        #region "Helpers"

        private HttpClient MakeClient()
        {
            var client = _factory.CreateClient("VaultConfigClient");
            client.BaseAddress = new Uri(_config.VaultUrl);
            client.DefaultRequestHeaders.Add(TOKEN_HEADER, _config.VaultToken);
            return client;
        }

        /// <summary>
        /// Parse Vault Json to Name/Value Dictionary
        /// </summary>
        /// <param name="json">Vault Json</param>
        /// <returns>Dictionary</returns>
        public static Dictionary<string, string> JsonToDictionary(string json)
        {
            JObject jobject = JObject.Parse(json);

            return jobject.Descendants()
                .Where(j => j.Children().Count() == 0)
                .Aggregate(
                    new Dictionary<string, string>(),
                    (props, jtoken) =>
                    {
                        props.Add(jtoken.Path, jtoken.ToString());
                        return props;
                    });
        }

        /// <summary>
        /// Dictionary To Json
        /// </summary>
        /// <param name="d">Dictionary</param>
        /// <returns>Well formed vault settings from a name/value pair</returns>
        public static string DictionaryToJson(Dictionary<string, string> d)
        {
            var sb = new System.Text.StringBuilder();

            sb.Append("{ \"data\": { ");

            var ct = 0;
            foreach (var kv in d)
            {
                ct++;
                sb.Append('"');
                sb.Append(kv.Key);
                sb.Append("\": \"");
                sb.Append(kv.Value);
                sb.Append('"');
                if (ct < d.Count) sb.Append(", ");
            }

            sb.Append(" }, \"options\":{} }");

            return sb.ToString();
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Renew Token
        /// <para>Called by Get as well</para>
        /// <para>It does an empty-body POST to the path <c>RENEW_PATH</c> which bumps the existing tokens expiration out in time.</para>
        /// </summary>
        public void TokenRenew()
        {
            var client = MakeClient();
            _ = client.PostAsync(RENEW_PATH, null).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get Settings from Vault
        /// </summary>
        /// <returns>Settings Dictionary</returns>
        public Dictionary<string, string> SettingsGet()
        {
            TokenRenew();

            var d = new Dictionary<string, string>();

            var client = MakeClient();

            var path = _config.SubPath;
            var response = client.GetAsync(path).GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode)
            {
                var jsonMessage = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var o = JObject.Parse(jsonMessage);
                IEnumerable<JToken> kvs = o.SelectTokens("data.data");
                var children = kvs.Children();
                foreach (var item in children)
                {
                    var key = ((Newtonsoft.Json.Linq.JProperty)item).Name;
                    var value = ((Newtonsoft.Json.Linq.JProperty)item).Value.ToString();
                    d.Add(key, value);
                }
            }
            else
            {
                var msg = $"{response.StatusCode}: {response.ReasonPhrase}";
                throw new HttpRequestException(msg);
            }

            return d;
        }

        // $ curl -X PUT -H "X-Vault-Token: $(vault print token)" -d '{"data":{"key1":"data1","key2":"data2","key3":"data3"},"options":{}}' https://localhost:8200/v1/secret/data/myApp/dev

        /// <summary>
        /// Put Settings into Vault
        /// </summary>
        /// <param name="settings">Settings to put into Vault</param>
        public void SettingPut(Dictionary<string, string> settings)
        {
            if ((settings == null) || (settings.Count <= 0)) throw new ArgumentNullException(nameof(settings));

            var json = DictionaryToJson(settings);
            _logger?.LogDebug($"Settings: {json}");

            var client = MakeClient();

            var path = _config.SubPath;
            using (var content = new StringContent(json, Encoding.UTF8, CONTENT_TYPE))
            {
                var response = client.PutAsync(path, content).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    var msg = $"{response.StatusCode}: {response.ReasonPhrase}";
                    throw new HttpRequestException(msg);
                }
            }
        }

        #endregion

    }
}
