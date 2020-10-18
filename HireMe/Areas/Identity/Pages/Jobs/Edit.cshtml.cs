namespace HireMe.Areas.Identity.Pages.Jobs
{
    using HireMe.Core.Helpers;
    using HireMe.Entities.Enums;
    using HireMe.Entities.Input;
    using HireMe.Entities.Models;
    using HireMe.Services.Interfaces;
    using HireMe.ViewModels.Company;
    using HireMe.ViewModels.Jobs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    using System.Linq;
    using System.Threading.Tasks;


    [Authorize(Roles = "Admin, Moderator, Employer, Recruiter")]
    public partial class EditModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IJobsService _jobsService;
        private readonly ICategoriesService _categoriesService;
        private readonly ILocationService _locationService;
        private readonly ICompanyService _companyService;
        private readonly IConfiguration _config;
        private readonly IBaseService _baseService;

        public EditModel(
            UserManager<User> userManager,
            ICategoriesService categoriesService,
            ILocationService locationService,
            IJobsService jobsService,
            ICompanyService companyService,
            IConfiguration config,
           IBaseService baseService)
        {
            this._userManager = userManager;
            this._categoriesService = categoriesService;
            this._locationService = locationService;
            this._jobsService = jobsService;
            this._companyService = companyService;
            this._config = config;
            _baseService = baseService;
        }


        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel : CreateJobInputModel { }
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
            ViewData["Categories"] = this._categoriesService.GetAllAsNoTrackingMapped().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.TitleAndCountContestant,
            });

            ViewData["Locations"] = this._locationService.GetAllAsNoTrackingMapped().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.City,
            });
            ViewData["Companies"] = this._companyService.GetAllAsNoTrackingMapped().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.TitleAndCount,
            });
            Input = new InputModel
            {
                Id = job.Id,
                Name = job.Name,
                LocationId = job.LocationId,
                WorkType = job.WorkType,
                Description = job.Description,
                Adress = job.Adress,
                Visiblity = job.Visiblity,
                MinSalary = job.MinSalary,
                MaxSalary = job.MaxSalary,
                SalaryType = job.SalaryType,
                CategoryId = job.CategoryId,
                CompanyId = job.CompanyId,
                LanguageId = job.LanguageId,
                TagsId = job.TagsId,
                isApproved = false
            };

            string siteUrl = _config.GetSection("MySettings").GetValue<string>("CompanyImageUrl");
            var company = await _companyService.GetByIdAsync<CompanyViewModel>(Input.CompanyId);
            Input.CompanyLogo = siteUrl + company.Logo;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
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
            ViewData["Categories"] = this._categoriesService.GetAllAsNoTrackingMapped().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.TitleAndCountContestant,
            });

            ViewData["Locations"] = this._locationService.GetAllAsNoTrackingMapped().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.City,
            });
            ViewData["Companies"] = this._companyService.GetAllAsNoTrackingMapped().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.TitleAndCount,
            });

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Input.Id = id;

            OperationResult result = await _jobsService.Update(Input, user);
            if (result.Success)
            {
                _baseService.ToastNotify(ToastMessageState.Success, "Успешно", "Обявата ви е актулизирана.", 2000);
            }

            return RedirectToPage();
        }
    }
}
