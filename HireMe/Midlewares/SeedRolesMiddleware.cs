namespace HireMe.Middlewares
{
    using HireMe.Data;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class SeedRolesMiddleware
    {
        private readonly RequestDelegate next;

        public SeedRolesMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider, RoleManager<IdentityRole> roleManager)
        {
            var dbContext = serviceProvider.GetService<BaseDbContext>();

            if (!dbContext.Roles.Any())
            {
                await this.SeedRoles(roleManager);
            }

            await this.next(context);
        }

        private async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("Moderator"));
            await roleManager.CreateAsync(new IdentityRole("Employer"));
            await roleManager.CreateAsync(new IdentityRole("Recruiter"));
            await roleManager.CreateAsync(new IdentityRole("Contestant"));
            await roleManager.CreateAsync(new IdentityRole("User"));
        }


    }
}