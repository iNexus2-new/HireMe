namespace HireMe.Middlewares
{
    using HireMe.Data;
    using HireMe.Entities.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    public class SeedLanguagesMiddleware
    {
        private readonly RequestDelegate next;
        public SeedLanguagesMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetService<FeaturesDbContext>();

            if (!dbContext.Language.Any())
            {
                await this.SeedCountries(dbContext);
            }

            await this.next(context);
        }

        public async Task SeedCountries(FeaturesDbContext dbContext)
        {
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var languages = new List<Language>();

            foreach (CultureInfo culture in cultures)
            {
                languages = new List<Language>
                {
                    new Language
                    {
                        Name = culture.EnglishName,
                        Code = culture.TwoLetterISOLanguageName
                    },
                };
                await dbContext.Language.AddRangeAsync(languages);
            }

            await dbContext.SaveChangesAsync();
        }

    }
}