namespace HireMe.Middlewares.Extensions
{
    using Microsoft.AspNetCore.Builder;

    public static class SeedRolesMiddlewareExtensions
    {
        public static IApplicationBuilder UseSeed(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<SeedRolesMiddleware>();
            builder.UseMiddleware<SeedCategoriesMiddleware>();
            builder.UseMiddleware<SeedLanguagesMiddleware>();
            builder.UseMiddleware<SeedLocationMiddleware>();
            builder.UseMiddleware<URLRewriter>();

            return builder;
        }
    }
}