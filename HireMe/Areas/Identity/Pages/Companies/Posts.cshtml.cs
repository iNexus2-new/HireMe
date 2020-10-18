namespace HireMe.Areas.Identity.Pages.Companies
{
    using HireMe.Entities.Models;
    using HireMe.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize(Roles = "Admin, Moderator, Employer, Recruiter")]
    public partial class PostsModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly ICompanyService _companyService;

        public PostsModel(UserManager<User> userManager, ICompanyService companyService)
        {
            this.userManager = userManager;
            this._companyService = companyService;
        }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int Count { get; set; }
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public bool EnablePrevious => CurrentPage > 1;
        public bool EnableNext => CurrentPage < TotalPages;
        public IAsyncEnumerable<Company> List { get; set; }

        public async Task<IActionResult> OnGetAsync(int currentPage)
        {
            var user = await userManager.GetUserAsync(User);
            var userid = user.Id;

            if (user == null)
            {
                return NotFound($"Моля влезте в системата отново или си изчистете кеш данните на браузъра.");
            }

            var entity = _companyService.GetAllAsNoTracking()
                .Where(x => x.PosterId == userid || x.Admin1_Id == userid || x.Admin2_Id == userid || x.Admin3_Id == userid)
                .OrderByDescending(x => x.Id);

            Count = await entity.CountAsync().ConfigureAwait(false);

            CurrentPage = currentPage == 0 ? 1 : currentPage;

            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;

            var result = entity
           .Skip((CurrentPage - 1) * PageSize)
           .Take(PageSize)
           .AsQueryable()
           .ToAsyncEnumerable();

            List = Count > 0 ? result : null;

            return Page();
        }



    }
}