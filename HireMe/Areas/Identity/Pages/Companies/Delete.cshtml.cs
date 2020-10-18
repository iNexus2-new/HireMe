namespace HireMe.Areas.Identity.Pages.Companies
{
    using HireMe.Entities.Models;
    using HireMe.Services.Interfaces;
    using HireMe.ViewModels.Company;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

    [Authorize(Roles = "Admin, Moderator, Employer")]
    public partial class DeleteModel : PageModel
    {
        private readonly ICompanyService _companyService;
        private readonly IJobsService _jobsService;
        private readonly UserManager<User> _userManager;

        public DeleteModel(
            ICompanyService companyService,
            IJobsService jobsService,
            UserManager<User> userManager)
        {
            this._companyService = companyService;
            this._jobsService = jobsService;
            this._userManager = userManager;
        }

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

            var entity = await _companyService.GetByIdAsync<CompanyViewModel>(id);
            if (entity == null)
            {
                return NotFound();
            }

            var poster = this._userManager.GetUserId(User);
            if (poster != entity.PosterId)
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

            var entity = await _companyService.GetByIdAsync<CompanyViewModel>(id);
            if (entity == null)
            {
                return NotFound();
            }

            var poster = this._userManager.GetUserId(User);
            if (poster != entity.PosterId)
            {
                return Redirect($"/Identity/Account/AccessDenied");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Password not correct.");
                    return Page();
                }
            }

            await _jobsService.DeleteAllByCompany(id);

            var result = await _companyService.Delete(id);

            if (result.Success)
            {
                Redirect($"/Identity/Companies/Posts/");
            }

            return Page();
        }

    }
}
