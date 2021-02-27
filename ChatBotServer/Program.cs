using ChatBotServer.Network;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace ChatBotServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Basic .Net Core Configuration
            Configuation();

            // Start listen for socket connection
            Server.StartListening();
            Console.ReadLine();
            Server.CloseAllSockets();
        }

        private static void Configuation()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIROMENT") ?? "Production"}.json", optional: true)
              .AddEnvironmentVariables();

            var config = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.FromLogContext()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var host = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
            })
            .UseSerilog()
            .Build();
        }
    }
}
