namespace HireMe.Areas.Identity.Pages.Resume
{
    using HireMe.Entities.Models;
    using HireMe.Services.Interfaces;
    using HireMe.ViewModels.Resume;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.Threading.Tasks;

    [Authorize]
    public partial class DeleteModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IResumeService _resumeService;

        public DeleteModel(IResumeService resumeService, UserManager<User> userManager)
        {
            _resumeService = resumeService;
            _userManager = userManager;
        }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var entity = await _resumeService.GetByIdAsync<ResumeViewModel>(id);
            if (entity == null)
            {
                return NotFound();
            }

            var posterId = this._userManager.GetUserId(User);
            if (posterId != entity.UserId)
            {
                return Redirect($"/Identity/Account/AccessDenied");
            }


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
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var entity = await _resumeService.GetByIdAsync<ResumeViewModel>(id);
            if (entity == null)
            {
                return NotFound();
            }

            var posterId = this._userManager.GetUserId(User);
            if (posterId != entity.UserId)
            {
                return Redirect($"/Identity/Account/AccessDenied");
            }

            await _resumeService.Delete(id, posterId);

            return Redirect($"/Identity/Resume/All/");
        }

    }
}
