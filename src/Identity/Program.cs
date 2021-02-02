using Microsoft.AspNetCore.Hosting;
using Bit.Core.Utilities;
using Serilog.Events;
using AspNetCoreRateLimit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Bit.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SerilogFix(false);
            Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseSerilog();
                  })
                .Build()
                .Run();
        }

        public static void SerilogFix(bool production=true)
        {
            var jconfig = production? "appsettings.Production.json":"appsettings.json";
            var config = new ConfigurationBuilder()
                .AddJsonFile(jconfig, optional: false)
                .Build();

            var logcfg = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                ;
            Log.Logger = logcfg.CreateLogger();
        }        
    }
}
