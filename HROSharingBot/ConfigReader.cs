using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace HROSharingBot
{
    public static class ConfigReader
    {
        public static IConfiguration Configuration { get; }

        static ConfigReader()
        {
            var environment = Environment.GetEnvironmentVariable("APP_ENVIRONMENT");
            var configfile = environment != null ? $"appsettings.{environment}.json" : "appsettings.json";
            
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configfile);
            Configuration = configBuilder.Build();
        }
    }
}