namespace HireMe.Areas.Identity.Pages.Jobs
{
    using HireMe.Entities.Models;
    using HireMe.Services.Interfaces;
    using HireMe.ViewModels.Jobs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize(Roles = "Admin, Moderator, Employer, Recruiter")]
    public partial class PostsModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IJobsService _jobsService;
        private readonly IResumeService _resumeService;
        public PostsModel(
            UserManager<User> userManager,
            IJobsService jobsService,
            IResumeService resumeService)
        {
            _userManager = userManager;
            _jobsService = jobsService;
            _resumeService = resumeService;
        }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int Count { get; set; }
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public bool EnablePrevious => CurrentPage > 1;
        public bool EnableNext => CurrentPage < TotalPages;
        public IAsyncEnumerable<Jobs> JobList { get; set; }

        public async Task<IActionResult> OnGetAsync(int currentPage)
        {
            var user = await _userManager.GetUserAsync(User);
            var userid = user.Id;

            if (user == null)
            {
                return NotFound($"Моля влезте в системата отново или си изчистете кеш данните на браузъра.");
            }

            var entity = _jobsService.GetAllAsNoTracking().Where(x => x.PosterID == userid).OrderByDescending(x => x.Id); ;
            Count = await entity.CountAsync().ConfigureAwait(false);

            CurrentPage = currentPage == 0 ? 1 : currentPage;

            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;

            var result = entity
           .OrderByDescending(x => x.Id)
           .Skip((CurrentPage - 1) * PageSize)
           .Take(PageSize)
           .AsQueryable()
           .ToAsyncEnumerable();

            JobList = Count > 0 ? result : null;

            return Page();
        }

        public async Task OnGetApplicationsAsync(int jobId, int currentPage)
        {
            var job = await _jobsService.GetByIdAsync<JobsViewModel>(jobId);
            string[] items = job.resumeFilesId.Split(',');

            var entity = _resumeService.GetAllAsNoTrackingMapped().Where(x => (((IList)items).Contains(x.Id.ToString())));
            Count = await entity.CountAsync().ConfigureAwait(false);

            CurrentPage = currentPage == 0 ? 1 : currentPage;

            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;

            var result = entity
           .OrderByDescending(x => x.Id)
           .Skip((CurrentPage - 1) * PageSize)
           .Take(PageSize)
           .AsQueryable()
           .ToAsyncEnumerable();

            //   ApplicationsList = Count > 0 ? result : null;
            ViewData["Applications"] = Count > 0 ? result : null;
        }

    }
}