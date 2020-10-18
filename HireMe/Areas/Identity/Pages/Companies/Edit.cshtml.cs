namespace HireMe.Areas.Identity.Pages.Companies
{
    using HireMe.Core.Helpers;
    using HireMe.Entities.Enums;
    using HireMe.Entities.Input;
    using HireMe.Entities.Models;
    using HireMe.Services.Interfaces;
    using HireMe.ViewModels.Company;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    // [ValidateAntiForgeryToken]
    //[PreventDuplicateRequest]
    [Authorize(Roles = "Admin, Moderator, Employer, Recruiter")]
    public partial class EditModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IBaseService _baseService;

        private readonly UserManager<User> _userManager;
        private readonly ICompanyService _companyService;
        private readonly ILocationService _locationService;


        public EditModel(
            IConfiguration config,
            IBaseService baseService,
            UserManager<User> userManager,
            ICompanyService companyService,
            ILocationService locationService)
        {
            _config = config;
            _baseService = baseService;
            _userManager = userManager;
            _companyService = companyService;
            _locationService = locationService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel : CreateCompanyInputModel
        {
            public string PictureFullPath { get; set; }

            public IFormFile FormFile { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Моля влезте в системата отново или си изчистете кеш данните на браузъра.");
            }

            var company = await _companyService.GetByIdAsync<CompanyViewModel>(id);
            if (company == null)
            {
                return NotFound();
            }


            /*
			var userid = this._userManager.GetUserId(User);
			var entity = _companyService.GetAllAsNoTracking()
	.Where(x => x.PosterId == userid || x.Admin1_Id == userid || x.Admin2_Id == userid || x.Admin3_Id == userid)
	.Any();

			
			
			if (!entity)
			{
				return Redirect($"/Identity/Account/AccessDenied");
			}

			*/

            ViewData["Locations"] = this._locationService.GetAllAsNoTrackingMapped().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.City,
            });


            Input = new InputModel
            {
                Email = company.Email,
                PhoneNumber = company.PhoneNumber,
                Title = company.Title,
                About = company.About,
                Private = company.Private,
                Logo = company.Logo,
                LocationId = company.LocationId,
                Adress = company.Adress,
                Website = company.Website,
                isAuthentic_EIK = company.isAuthentic_EIK,
                Admin1_Id = company.Admin1_Id,
                Admin2_Id = company.Admin2_Id,
                Admin3_Id = company.Admin3_Id
            };

            string siteUrl = _config.GetSection("MySettings").GetSection("SiteImageUrl").Value;
            Input.PictureFullPath = siteUrl + Input.Logo;

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

            var company = await _companyService.GetByIdAsync<CompanyViewModel>(id);

            if (company == null)
            {
                return NotFound();
            }

            var poster = this._userManager.GetUserId(User);
            if (poster != company.PosterId || poster != company.Admin1_Id || poster != company.Admin2_Id || poster != company.Admin3_Id)
            {
                return Redirect($"/Identity/Account/AccessDenied");
            }

            ViewData["Locations"] = this._locationService.GetAllAsNoTrackingMapped().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.City,
            });


            Input.Id = id;
            if (Input.FormFile != null)
            {
                Input.Logo = await _baseService.UploadImageAsync(Input.FormFile, user);
            }

            OperationResult result = await this._companyService.Update(Input, true, user);

            if (result.Success)
            {
                _baseService.ToastNotify(ToastMessageState.Success, "Успешно", "Детайлите ви са актулизирани.", 2000);
                return Redirect($"/Company/Details/{id}");
            }

            return Page();
        }

    }
}
