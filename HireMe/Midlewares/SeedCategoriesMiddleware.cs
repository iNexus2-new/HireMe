namespace HireMe.Middlewares
{
    using HireMe.Data;
    using HireMe.Entities.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class SeedCategoriesMiddleware
    {
        private readonly RequestDelegate next;
        public SeedCategoriesMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetService<FeaturesDbContext>();

            if (!dbContext.Category.Any())
            {
                await this.SeedCategories(dbContext);
            }

            await this.next(context);
        }

        public async Task SeedCategories(FeaturesDbContext dbContext)
        {
            string[] lines = File.ReadAllLines(@"C:\Users\iNexus\Documents\source\repos\HireMe\HireMe\Categories.txt");
            var Categories = new List<Category>();
            var vals1 = lines;

            for (int i = 0; i <= lines.Length; i++)
            {
                vals1 = lines[i].Split('#');

                Categories = new List<Category>
                {
                    new Category
                    {
                        Title = vals1[0].ToString(),
                        Title_BG = vals1[1].ToString(),
                        Icon = vals1[2].ToString()
                    },
                };
                await dbContext.Category.AddRangeAsync(Categories);
                await dbContext.SaveChangesAsync();
            }

        }



    }
}