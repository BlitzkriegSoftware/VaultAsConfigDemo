namespace Blitz.Configuration.Vault.Library.Models
{
    /// <summary>
    /// Configuration to Connect to Vault
    /// </summary>
    public class VaultConfiguration
    {
        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; } = "http://localhost:8200";

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; } = string.Empty;

        public string RootPath { get; set; } = "v1/secret/data";

        public string Application { get; set; } = "myApp";

        public string EnvironmentPath { get; set; } = "dev";

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
                    segment = segment.Substring(0, segment.Length - 1).Trim();
                }
                if (segment.StartsWith("/"))
                {
                    segment = segment.Substring(1).Trim();
                }
            }
            return segment;
        }


        /// <summary>
        /// Parse Configuration Data into this Configuration
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void ParseConfigurationField(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            if (string.IsNullOrWhiteSpace(value)) return;

            switch(name.ToLowerInvariant().Trim())
            {
                case "vaulturl": this.Url = value; break;
                case "vaulttoken": this.Token = value; break;
                case "vaultapp": this.Application = value; break;
                case "vaultenv": this.EnvironmentPath = value; break;
                case "vaultrootpath": this.RootPath = value; break;
            }
        }

        /// <summary>
        /// Is Valid (URL, PATH, TOKEN) are not empty or null
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Url) ||
                       !string.IsNullOrWhiteSpace(this.Application) ||
                       !string.IsNullOrWhiteSpace(this.EnvironmentPath) ||
                       !string.IsNullOrWhiteSpace(this.Token);
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

                return $"/{(string.IsNullOrWhiteSpace(RootPath) ? string.Empty : this.RootPath + slash)}{CleanSegment(this.Application)}/{CleanSegment(this.EnvironmentPath)}";
            }
        }

        /// <summary>
        /// Debugging String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            char slash = '/';

            return $"{CleanSegment(this.Url)}/{(string.IsNullOrWhiteSpace(RootPath) ? string.Empty : this.RootPath + slash)}{CleanSegment(this.Application)}/{CleanSegment(this.EnvironmentPath)}";
        }
    }

}

