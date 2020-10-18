namespace HireMe.Areas.Identity.Pages.Jobs
{
    using HireMe.Entities.Models;
    using HireMe.Services.Interfaces;
    using HireMe.ViewModels.Jobs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

    [Authorize(Roles = "Admin, Moderator, Employer, Recruiter")]
    public partial class DeleteModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IJobsService _jobsService;

        public DeleteModel(IJobsService jobsService, UserManager<User> userManager)
        {
            this._jobsService = jobsService;
            this._userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public Jobs jobs { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public bool RequirePassword { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Моля влезте в системата отново или си изчистете кеш данните на браузъра.");
            }

            var job = await _jobsService.GetByIdAsync<JobsViewModel>(id);
            if (job == null)
            {
                return NotFound();
            }

            var posterId = this._userManager.GetUserId(User);
            if (posterId != job.PosterID)
            {
                return Redirect($"/Identity/Account/AccessDenied");
            }


            RequirePassword = await _userManager.HasPasswordAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Моля влезте в системата отново или си изчистете кеш данните на браузъра.");
            }

            var job = await _jobsService.GetByIdAsync<JobsViewModel>(id);
            if (job == null)
            {
                return NotFound();
            }

            var posterId = this._userManager.GetUserId(User);
            if (posterId != job.PosterID)
            {
                return Redirect($"/Identity/Account/AccessDenied");
            }

            await this._jobsService.Delete(id);

            return Redirect($"/Identity/Jobs/Posts/");
        }

    }
}
