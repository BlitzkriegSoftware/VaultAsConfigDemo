using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Blitz.Configuration.Vault.Library.Helpers;
using Blitz.Configuration.Vault.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Blitz.Configuration.Vault.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfigurationRoot _config;

        public HomeController(ILogger<HomeController> logger, IConfigurationRoot config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            var d = new List<Models.Triplet>();

            foreach (var c in _config.AsEnumerable())
            {
                _logger.LogTrace($"Key: {0}, Value: {1}", c.Key, c.Value);
                var t = new Models.Triplet()
                {
                    Key = c.Key,
                    Value = c.Value
                };

                if (c.Key.StartsWith("vault"))
                {
                    t.Category = "Configuration from Environment";
                    t.Weight = 3;
                }
                else if (c.Key.StartsWith("key"))
                {
                    t.Category = "Configuration from Vault";
                    t.Weight = 2;
                }
                else
                {
                    t.Category = "Misc.";
                    t.Weight = 0;
                }

                if (t.Weight > 0) d.Add(t);
            }

            d.Sort((a, b) => (b.Weight.CompareTo(a.Weight)));

            return View(d);
        }

        [HttpGet("/Metadata")]
        public IActionResult Metadata() {
            var d = VaultConfigurationExtensions.GetMetadata(_logger, _config);
            return View(d);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
