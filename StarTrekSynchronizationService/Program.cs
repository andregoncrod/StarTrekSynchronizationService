using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StarTrekSynchronizationService;
using StarTrekSynchronizationService.Models;
using System;
using System.Configuration;

public class Program
{
    public static void Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                IConfiguration configuration = hostContext.Configuration;

                AppSettings.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                AppSettings.StarTrekAPIUrl = configuration.GetValue<string>("StarTrekAPIUrl");

                var optionsBuilder = new DbContextOptionsBuilder<StarTrekContext>();

                optionsBuilder.UseSqlServer(AppSettings.ConnectionString);

                services.AddScoped(db => new StarTrekContext(optionsBuilder.Options));

                services.AddHostedService<Worker>();
            })
            .Build();

        CreateDbIfNoneExist(host);

        host.Run();
    }

    private static void CreateDbIfNoneExist(IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var service = scope.ServiceProvider;

            try
            {
                var context = service.GetRequiredService<StarTrekContext>();
                context.Database.EnsureCreated();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
