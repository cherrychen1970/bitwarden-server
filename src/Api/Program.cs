using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Bit.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builer = WebHost.CreateDefaultBuilder(args);
            SerilogFix(false);
            builer.UseStartup<Startup>().UseSerilog().Build().Run();
        }

        public static void SerilogFix(bool production = true)
        {
            var jconfig = production ? "appsettings.Production.json" : "appsettings.json";
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
