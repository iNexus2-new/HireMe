namespace HireMe.Areas.Identity.Pages.Contestant
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

    [Authorize]
    public partial class PostsModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IContestantsService _contestantService;

        public PostsModel(UserManager<User> userManager, IContestantsService contestantService)
        {
            this.userManager = userManager;
            this._contestantService = contestantService;
        }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int Count { get; set; }
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public bool EnablePrevious => CurrentPage > 1;
        public bool EnableNext => CurrentPage < TotalPages;
        public IAsyncEnumerable<Contestant> List { get; set; }

        public async Task<IActionResult> OnGetAsync(int currentPage)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Моля влезте в системата отново или си изчистете кеш данните на браузъра.");
            }

            var userid = user.Id;
            var entity = _contestantService.GetAllAsNoTracking().Where(x => x.PosterID == userid).OrderByDescending(x => x.Id);
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