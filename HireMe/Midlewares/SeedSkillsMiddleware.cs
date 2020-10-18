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

    public class URLRewriter
    {
        private readonly RequestDelegate next;
        public URLRewriter(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetService<FeaturesDbContext>();

            if (!dbContext.Skills.Any())
            {
                await this.SeedSkills(dbContext);
            }

            await this.next(context);
        }

        public async Task SeedSkills(FeaturesDbContext dbContext)
        {
            var skills = new List<Skills>();
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\iNexus\Documents\source\repos\HireMe\HireMe\Skills.txt");

            foreach (string line in lines)
            {
                skills = new List<Skills>
                {
                    new Skills
                    {
                        Title = line
                    }
                };
                await dbContext.Skills.AddRangeAsync(skills);
            }

            await dbContext.SaveChangesAsync();
        }



    }
}