using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

namespace gatekeeper
{
    // public class Program
    // {
    //     public static void Main(string[] args)
    //     {
    //         new WebHostBuilder()
    //             .UseKestrel()
    //             .UseContentRoot(Directory.GetCurrentDirectory())
    //             .ConfigureAppConfiguration((hostingContext, config) =>
    //             {
    //                 config
    //                     .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
    //                     .AddJsonFile("appsettings.json", true, true)
    //                     .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
    //                     .AddJsonFile("ocelot.json")
    //                     .AddEnvironmentVariables();
    //             })
    //             .ConfigureServices(s => {
    //                 s.AddOcelot();
    //             })
    //             .ConfigureLogging((hostingContext, logging) =>
    //             {
    //                 //add your logging
    //             })
    //             .UseIISIntegration()
    //             .Configure(app =>
    //             {
    //                 app.UseOcelot().Wait();
    //             })
    //             .Build()
    //             .Run();
    //     }
    // }
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq("http://seq:80", apiKey: "moH1cWrcBsBmR1K1q1HJ")
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
            // CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile($"ocelot.json",false,true);

                    // for local test use below one, multi env json file not worked
                    //config.AddJsonFile($"configuration.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
                    // https://github.com/ThreeMammals/Ocelot/issues/249
                });
    }
}