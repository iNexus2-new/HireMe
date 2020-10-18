namespace HireMe.Areas.Identity.Pages.Admin
{
    using HireMe.Entities.Models;
    using HireMe.Services.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class JobPostsModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IJobsService _jobsService;

        public JobPostsModel(UserManager<User> userManager, IJobsService jobsService)
        {
            this.userManager = userManager;
            this._jobsService = jobsService;
        }
        // [BindProperty]
        //public PaginationModel Pagination { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int Count { get; set; }
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public bool EnablePrevious => CurrentPage > 1;
        public bool EnableNext => CurrentPage < TotalPages;
        public IAsyncEnumerable<Jobs> JobList { get; set; }

        public async Task<IActionResult> OnGetAsync(int currentPage)
        {
            var user = await userManager.GetUserAsync(User);
            var userid = user.Id;

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var entity = _jobsService.GetAllAsNoTracking().Where(x => x.isApproved != 2).OrderByDescending(x => x.Id);
            Count = await entity.CountAsync().ConfigureAwait(false);

            CurrentPage = currentPage == 0 ? 1 : currentPage;

            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;

            var result = entity
           .Skip((CurrentPage - 1) * PageSize)
           .Take(PageSize)
           .AsQueryable()
           .ToAsyncEnumerable();

            JobList = Count > 0 ? result : null;

            return Page();
        }



    }
}