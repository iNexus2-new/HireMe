namespace HireMe.Middlewares
{
    using HireMe.Data;
    using HireMe.Entities.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class SeedLocationMiddleware
    {
        private readonly RequestDelegate next;
        public SeedLocationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetService<FeaturesDbContext>();

            if (!dbContext.Location.Any())
            {
                await this.SeedLocation(dbContext);
            }

            await this.next(context);
        }

        public async Task SeedLocation(FeaturesDbContext dbContext)
        {
            var location = new List<Location>();
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\iNexus\Documents\source\repos\HireMe\HireMe\Location.txt");

            foreach (string line in lines)
            {
                location = new List<Location>
                {
                    new Location
                    {
                        City = line
                    }
                };
                await dbContext.Location.AddRangeAsync(location);
            }

            await dbContext.SaveChangesAsync();
        }



    }
}