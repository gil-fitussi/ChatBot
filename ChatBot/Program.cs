using ChatBot.DB;
using ChatBot.Interface;
using ChatBot.Models;
using ChatBot.Network;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace ChatBot
{
    class Program
    {
        private static IRegistration _registration;

        static void Main(string[] args)
        {
            // Basic .Net Core Configuration
            Configuation();

            // Login/SignUp UI And Logic
            ResultSet result = _registration.CheckUserLoginDecision();

            if (result.Status == Status.Failed)
                if (GeneralHelper.CheckIfType<string>(result.ReturnResult))
                    Console.WriteLine(result.ReturnResult.ToString());

            if (result.Status == Status.Success)
            {
                if (GeneralHelper.CheckIfType<User>(result.ReturnResult))
                {
                    User user = result.ReturnResult as User;
                    Console.WriteLine($"Welcome {user.UserName}");

                    Client client = new Client();
                    Console.WriteLine($"Connecting To Server..");
                    client.Connect();
                }
            }
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
                services.AddSingleton<IRegistration, Registration>()
                .AddSingleton<IDbQuery, DbQuery>()
                .AddSingleton(provider => config).BuildServiceProvider(); 
            })
            .UseSerilog()
            .Build();

            _registration = ActivatorUtilities.GetServiceOrCreateInstance<IRegistration>(host.Services);
        }
    }
}
