namespace HireMe.Areas.Identity.Pages.Contestant
{
    using HireMe.Entities.Models;
    using HireMe.Services.Interfaces;
    using HireMe.ViewModels.Contestants;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

    [Authorize(Roles = "Admin, Moderator, Contestant")]
    public partial class DeleteModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IContestantsService _contestantsService;

        public DeleteModel(
            IContestantsService contestantsService,
            UserManager<User> userManager)
        {
            this._contestantsService = contestantsService;
            this._userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public Contestant Contestant { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Парола")]
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

            var contestant = await _contestantsService.GetByIdAsync<ContestantViewModel>(id);
            if (contestant == null)
            {
                return NotFound();
            }

            var poster = this._userManager.GetUserId(User);
            if (poster != contestant.PosterID)
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

            var contestant = await _contestantsService.GetByIdAsync<ContestantViewModel>(id);
            if (contestant == null)
            {
                return NotFound();
            }
            var poster = this._userManager.GetUserId(User);
            if (poster != contestant.PosterID)
            {
                return Redirect($"/Identity/Account/AccessDenied");
            }

            await _userManager.RemoveFromRoleAsync(user, "Contestant");
            await _userManager.AddToRoleAsync(user, "User");
            await _userManager.UpdateAsync(user);

            await this._contestantsService.Delete(id);

            return Redirect($"/Identity/Account/Manage/Index/");
        }

    }
}
