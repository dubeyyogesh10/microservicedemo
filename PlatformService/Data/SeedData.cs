using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class SeedData
    {
        public static void SeedDataForTest(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedDataIntoDb(serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }

        private static void SeedDataIntoDb(AppDbContext context)
        {
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