using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using src.Models;
using src.Models.Entities;

namespace src
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<OneDbContext>();
                    var userManager = services.GetRequiredService<UserManager<UserOne>>();
                    var roleManager = services.GetRequiredService<RoleManager<RoleOne>>();
                    var config = services.GetRequiredService<IConfiguration>();

                    DbInitializer.Initialize(context, userManager, roleManager, config)
                        .ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch (Exception)
                {
                    //Log.ForContext("EventSource", "Sistem Altius").Error("Terdapat eksepsi saat membuat data-data inisial ke dalam database." + " -- " + ex.Message);
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
