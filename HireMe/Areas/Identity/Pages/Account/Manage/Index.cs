using HireMe.Entities.Models;
using HireMe.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace HireMe.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public partial class DashboardModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IContestantsService _contestantService;


        public DashboardModel(UserManager<User> userManager,
            IContestantsService contestantService)
        {
            _userManager = userManager;
            this._contestantService = contestantService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
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

            var contestant = await _contestantService.GetByPosterId(user.Id);

            if (contestant == null)
            {
                return NotFound();
            }

            ViewData["contestantViews"] = contestant.Views;
            return RedirectToPage();
        }
    }
}
