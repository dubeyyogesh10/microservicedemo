using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class SeedData
    {
        public static void SeedDataForTest(IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedDataIntoDb(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
            }
        }

        private static void SeedDataIntoDb(AppDbContext context, bool isProd)
        {
            if (isProd)
            {
                try
                {
                    Console.WriteLine("Applying migration in production");
                    context.Database.Migrate();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"Applying migration in production Failed " + ex.Message);
                }

            }
            if (!context.Platforms.Any())
            {
                Console.WriteLine("Seeding data");
                context.Platforms.AddRange(new Platform() { Name = "Dotnet", Publisher = "Microsoft", Cost = "free" },
                new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "free" },
                new Platform() { Name = "Kubernetes", Publisher = "Cloud native Foundation", Cost = "free" });

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("We already have the data in the table");
            }
        }
    }
}