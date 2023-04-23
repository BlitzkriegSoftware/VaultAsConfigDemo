namespace Blitz.Configuration.Vault.Library.Models
{
    /// <summary>
    /// Configuration to Connect to Vault
    /// </summary>
    public class VaultConfiguration
    {
        /// <summary>
        /// URL to Vault API (or server)
        /// <para>http[s]://...[:port]</para>
        /// </summary>
        public string VaultUrl { get; set; } = "http://127.0.0.1:8200";

        /// <summary>
        /// Token aka Hashicorp Vault Secret as a Token or Pass-Phrase
        /// <para>Should not be empty or null</para>
        /// </summary>
        public string VaultToken { get; set; } = string.Empty;

        /// <summary>
        /// Path to secret data e.g., 
        /// <example><![CDATA[v1/secret/data]]></example>
        /// <list type="number">
        /// <item>Must not start or end with <![CDATA[/]]></item>
        /// <item>Can be segmented with <![CDATA[/]]></item>
        /// <item>Can not be a null or empty string</item>
        /// </list>
        /// </summary>
        public string RootPath { get; set; } = "v1/secret/data";

        /// <summary>
        /// Application Name aka your application name e.g., <example>myApp</example>
        /// </summary>
        public string Application { get; set; } = "myApp";

        /// <summary>
        /// Environment Abbrev. aka the environment name in abbreviation form e.g., <![CDATA[dev]]>
        /// </summary>
        public string EnvironmentAbbrev { get; set; } = "dev";

        private static string CleanSegment(string segment)
        {
            if (string.IsNullOrWhiteSpace(segment))
            {
                segment = string.Empty;
            }
            else
            {
                if (segment.EndsWith("/"))
                {
                    segment = segment[..^1].Trim();
                }
                if (segment.StartsWith("/"))
                {
                    segment = segment[1..].Trim();
                }
            }
            return segment;
        }


        /// <summary>
        /// Parse Configuration Data into this Configuration
        /// <para>Configuration Keys for this Library</para>
        /// <list type="number">
        /// <item>vaulturl - <see cref="VaultUrl"/></item>
        /// <item>vaulttoken - <see cref="VaultToken"/></item>
        /// <item>vaultapp - <see cref="Application"/></item>
        /// <item>vaultenv - <see cref="EnvironmentAbbrev"/></item>
        /// <item>vaultrootpath - <see cref="RootPath"/></item>
        /// </list>
        /// </summary>
        /// <param name="name">(sic)</param>
        /// <param name="value">(sic)</param>
        public void ParseConfigurationField(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            if (string.IsNullOrWhiteSpace(value)) return;

            switch(name.ToLowerInvariant().Trim())
            {
                case "vaulturl": this.VaultUrl = value; break;
                case "vaulttoken": this.VaultToken = value; break;
                case "vaultapp": this.Application = value; break;
                case "vaultenv": this.EnvironmentAbbrev = value; break;
                case "vaultrootpath": this.RootPath = value; break;
            }
        }

        /// <summary>
        /// <see cref="VaultUrl"/>, <see cref="VaultToken"/>, <see cref="Application"/>, and <see cref="EnvironmentAbbrev"/> are not empty or null
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.VaultUrl) ||
                       !string.IsNullOrWhiteSpace(this.Application) ||
                       !string.IsNullOrWhiteSpace(this.EnvironmentAbbrev) ||
                       !string.IsNullOrWhiteSpace(this.VaultToken);
            }
        }

        /// <summary>
        /// Return subpath minus root url
        /// </summary>
        public string SubPath
        {
            get
            {
                char slash = '/';
                return $"/{(string.IsNullOrWhiteSpace(RootPath) ? string.Empty : this.RootPath + slash)}{CleanSegment(this.Application)}/{CleanSegment(this.EnvironmentAbbrev)}";
            }
        }

        /// <summary>
        /// Debugging String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            char slash = '/';
            return $"{CleanSegment(this.VaultUrl)}/{(string.IsNullOrWhiteSpace(RootPath) ? string.Empty : this.RootPath + slash)}{CleanSegment(this.Application)}/{CleanSegment(this.EnvironmentAbbrev)}";
        }
    }

}

