namespace HireMe.Areas.Identity.Pages.Companies
{
    using HireMe.Core.Helpers;
    using HireMe.Core.Helpers.Interfaces;
    using HireMe.Entities.Enums;
    using HireMe.Entities.Input;
    using HireMe.Entities.Models;
    using HireMe.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize(Roles = "Admin, Moderator, Employer")]
    public partial class CreateModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        private readonly ICompanyService _companyService;
        private readonly ILocationService _locationService;

        private readonly IBaseService _baseService;
        private readonly IEikValidator _eikValidator;

        public CreateModel(
            UserManager<User> userManager,
            ICompanyService companyService,
            ILocationService locationService,
            IBaseService baseService,
            IEikValidator eikValidator)
        {
            _userManager = userManager;
            _companyService = companyService;
            _locationService = locationService;

            _baseService = baseService;
            _eikValidator = eikValidator;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel : CreateCompanyInputModel
        {
            public IFormFile FormFile { get; set; }
        }

        [BindProperty]
        public string EIK { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Моля влезте в системата отново или си изчистете кеш данните на браузъра.");
            }

            if (!user.profileConfirmed)
            {
                _baseService.ToastNotify(ToastMessageState.Error, "Грешка", "Моля попълнете личните си данни преди да продължите.", 20000);
                return Redirect($"/Identity/Account/Manage/EditProfile");
            }

            ViewData["Locations"] = this._locationService.GetAllAsNoTrackingMapped().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.City,
            });

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Моля влезте в системата отново или си изчистете кеш данните на браузъра.");
            }

            var posterId = _userManager.GetUserId(User);
            ViewData["Locations"] = this._locationService.GetAllAsNoTrackingMapped().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.City,
            });
            if (!ModelState.IsValid)
            {
                return Page();
            }

            OperationResult result;
            //   var eik_13 = _eikValidator.checkEIK(EIK);
            //  bool isAuthenticEIK = false;

            //  if (!eik_13)
            //      return Redirect($"/identity/companies/errors/wrongeik");
            //   else
            //    {

            if (Input.FormFile != null)
                Input.Logo = await _baseService.UploadImageAsync(Input.FormFile, user);

            if (Input.Admin1_Id != null)
            {
                var recruiterUser_1 = await _userManager.FindByIdAsync(Input.Admin1_Id);
                await _userManager.AddToRoleAsync(recruiterUser_1, "Recruiter");
            }

            if (Input.Admin2_Id != null)
            {
                var recruiterUser_2 = await _userManager.FindByIdAsync(Input.Admin2_Id);
                await _userManager.AddToRoleAsync(recruiterUser_2, "Recruiter");
            }

            if (Input.Admin3_Id != null)
            {
                var recruiterUser_3 = await _userManager.FindByIdAsync(Input.Admin3_Id);
                await _userManager.AddToRoleAsync(recruiterUser_3, "Recruiter");
            }

            result = await _companyService.Create(Input, true, user);

            if (result.Success)
            {
                _baseService.ToastNotify(ToastMessageState.Warning, "Внимание", "Моля изчакайте заявката ви да се прегледа от администратор.", 7000);
                _baseService.ToastNotify(ToastMessageState.Success, "Успешно", "Фирмата ви е добавена.", 5000);
                return Redirect($"/identity/companies/posts");
            }
            //  }
            return Page();
        }

    }
}
