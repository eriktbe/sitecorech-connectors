using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsWebAPISample
{
    public static class Configuration
    {
        public static string GetConnectionString()
        {
            IConfiguration Config = new ConfigurationBuilder()
                    .AddJsonFile("appSettings.json")
                    .Build();

            var Connect = Config.GetSection("Connect").Value;

            return Connect;
        }
    }
}
