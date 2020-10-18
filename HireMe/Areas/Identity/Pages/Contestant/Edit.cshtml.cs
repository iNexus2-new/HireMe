namespace HireMe.Areas.Identity.Pages.Contestant
{
    using HireMe.Core.Helpers;
    using HireMe.Entities.Enums;
    using HireMe.Entities.Input;
    using HireMe.Entities.Models;
    using HireMe.Services.Interfaces;
    using HireMe.ViewModels.Contestants;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize(Roles = "Admin, Moderator, Contestant")]
    public partial class EditModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IBaseService _baseService;
        private readonly UserManager<User> _userManager;
        private readonly IContestantsService _contestantService;
        private readonly ICategoriesService _categoriesService;
        private readonly ILocationService _locationService;

        public EditModel(
            IConfiguration config,
            IBaseService baseService,
            ICategoriesService categoriesService,
            ILocationService locationService,
            IContestantsService contestantService,
            UserManager<User> userManager)
        {
            this._config = config;
            this._baseService = baseService;
            this._categoriesService = categoriesService;
            this._contestantService = contestantService;
            this._locationService = locationService;
            this._userManager = userManager;
        }


        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel : CreateContestantInputModel
        {
            public string PictureUrl { get; set; }

            public string resumeFullPath { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Моля влезте в системата отново или си изчистете кеш данните на браузъра.");
            }

            var contestant = await _contestantService.GetByIdAsync<ContestantViewModel>(id);

            if (contestant == null)
            {
                return NotFound();
            }

            var poster = this._userManager.GetUserId(User);

            if (poster != contestant.PosterID)
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

            Input = new InputModel
            {
                Id = contestant.Id,
                Name = contestant.Name,
                LocationId = contestant.LocationId,
                WorkType = contestant.WorkType,
                Description = contestant.Description,
                Genders = contestant.Genders,
                Age = contestant.Age,
                About = contestant.About,
                Experience = contestant.Experience,
                SalaryType = contestant.SalaryType,
                CategoryId = contestant.CategoryId,
                payRate = contestant.payRate,
                profileVisiblity = contestant.profileVisiblity,
                WebsiteUrl = contestant.WebsiteUrl,
                PortfolioUrl = contestant.PortfolioUrl,
                Facebook = contestant.Facebook,
                Linkedin = contestant.Linkedin,
                Twitter = contestant.Twitter,
                Github = contestant.Github,
                Dribbble = contestant.Dribbble,
                ResumeFileId = contestant.ResumeFileId,
                userSkillsId = contestant.userSkillsId,
                LanguagesId = contestant.LanguagesId
            };
            string imageUrl = _config.GetSection("MySettings").GetSection("SiteImageUrl").Value;
            Input.PictureUrl = imageUrl + user.PictureName;

            string siteUrl = _config.GetSection("MySettings").GetSection("SiteResumeUrl").Value;
            Input.resumeFullPath = siteUrl + contestant.ResumeFileId;

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

            var contestant = await _contestantService.GetByIdAsync<ContestantViewModel>(id);
            if (contestant == null)
            {
                return NotFound();
            }
            var posterId = _userManager.GetUserId(User);

            if (posterId != contestant.PosterID)
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

            Input.Id = id;

            if (Input.FormFile != null)
                Input.ResumeFileId = await _baseService.UploadFileAsync(Input.FormFile, user);

            OperationResult result = await this._contestantService.Update(Input, user);
            if (result.Success)
            {
                _baseService.ToastNotify(ToastMessageState.Success, "Успешно", "Обявата ви е актулизирана.", 2000);

            }
            return RedirectToPage();
        }


    }
}
