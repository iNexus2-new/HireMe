using HireMe.Data;
using HireMe.Entities.Enums;
using HireMe.Entities.Input;
using HireMe.Entities.Models;
using HireMe.Services.Interfaces;
using HireMe.ViewModels.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace HireMe.Controllers
{
    public class JobsController : BaseController
    {
        private readonly FeaturesDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IBaseService _baseService;
        private readonly IJobsService _jobsService;
        private readonly ICategoriesService _categoriesService;
        private readonly ILocationService _locationService;
        private readonly ISkillsService _skillsService;
        private readonly ILanguageService _langService;
        private readonly ICompanyService _companyService;
        private readonly IFavoritesService _favoriteService;

        private readonly INotificationService _notifyService;
        private readonly IToastNotification _toastNotification;

        private readonly IResumeService _resumeService;

        public JobsController(
            FeaturesDbContext context,
            UserManager<User> userManager,
            IBaseService baseService,
            IJobsService jobsService,
            ICategoriesService categoriesService,
            ILocationService locationService,
            ISkillsService skillsService,
            ILanguageService langService,
            ICompanyService companyService,
            IFavoritesService favoriteService,
            INotificationService notifyService,
            IToastNotification toastNotification,
            IResumeService resumeService)
        {
            _context = context;
            _userManager = userManager;
            _baseService = baseService;
            _jobsService = jobsService;
            _locationService = locationService;
            _skillsService = skillsService;
            _langService = langService;
            _categoriesService = categoriesService;
            _companyService = companyService;
            _favoriteService = favoriteService;
            _notifyService = notifyService;
            _toastNotification = toastNotification;
            _resumeService = resumeService;
        }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int Count { get; set; }
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        [HttpGet]
        public async Task<IActionResult> Index(string searchString, int categoryId, int companyId, int locationId, string WorktypeId, string exprience, string LanguageId, string TagsId, int currentPage)
        {

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
            ViewData["Exprience"] = Enum.GetValues(typeof(ExprienceLevels)).Cast<ExprienceLevels>().ToList()
                .Select(x => new SelectListItem
                {
                    Value = x.ToString(),
                    Text = x.GetDisplayName()
                });

            var entity = _jobsService.GetAllAsNoTrackingMapped().Where(x => x.isApproved == 2);

            if (!String.IsNullOrEmpty(searchString))
            {
                entity = entity.Where(s => s.Name.ToLower().Contains(searchString));
            }
            if (!String.IsNullOrEmpty(LanguageId))
            {
                string[] langs = LanguageId.Split(',');
                entity = entity.Where(x => ((IList)langs).Contains(x.LanguageId));
            }
            if (!String.IsNullOrEmpty(TagsId))
            {
                string[] tags = TagsId.Split("%2C");
                entity = entity.Where(x => ((IList)tags).Contains(x.TagsId));
            }
            if (!String.IsNullOrEmpty(WorktypeId))
            {
                string[] types = WorktypeId.Split("%2C");
                entity = entity.Where(x => ((IList)types).Contains(x.WorkType));
            }
            if (!String.IsNullOrEmpty(exprience))
            {
                entity = entity.Where(x => exprience == x.ExprienceLevels.ToString());
            }
            if (categoryId > 0)
            {
                entity = entity.Where(x => x.CategoryId == categoryId);
            }
            if (companyId > 0)
            {
                entity = entity.Where(x => x.CompanyId == companyId);
            }
            if (locationId > 0)
            {
                entity = entity.Where(x => x.LocationId == locationId);
            }
            else
            {
                entity = entity.OrderByDescending(x => x.Id);
            }

            Count = await entity.CountAsync().ConfigureAwait(false); // check
            if (Count > 0) // prevent 'SqlException: The offset specified in a OFFSET clause may not be negative.'
            {
                CurrentPage = currentPage == 0 ? 1 : currentPage;

                if (CurrentPage > TotalPages)
                    CurrentPage = TotalPages;

                var result = entity
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .AsQueryable()
                .ToAsyncEnumerable();


                ViewData["Jobs"] = result;

                ViewBag.Count = Count;
                ViewBag.CurrentPage = CurrentPage;
                ViewBag.TotalPages = TotalPages;
                ViewBag.PageSize = PageSize;
            }
            else ViewData["Jobs"] = null;
            return View();
        }

        [Authorize(Roles = "Admin, Moderator, Employer, Recruiter")]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.profileConfirmed)
            {
                _baseService.ToastNotify(ToastMessageState.Error, "Грешка", "Моля попълнете личните си данни преди да продължите.", 7000);
                return Redirect($"/Identity/Account/Manage/EditProfile");
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

            ViewData["Companies"] = this._companyService.GetAllAsNoTrackingMapped()?.Where(x => x.PosterId == user.Id || x.Admin1_Id == user.Id || x.Admin2_Id == user.Id || x.Admin3_Id == user.Id)?
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TitleAndCount,
                });

            ViewData["Worktypes"] = Enum.GetValues(typeof(WorkType)).Cast<WorkType>()
    .Select(x => new SelectListItem
    {
        Value = x.ToString(),
        Text = Enum.GetName(typeof(WorkType), x)
    }).ToAsyncEnumerable();


            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateJobInputModel input)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
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
            ViewData["Companies"] = this._companyService.GetAllAsNoTrackingMapped()?.Where(x => x.Admin1_Id == user.Id || x.Admin2_Id == user.Id || x.Admin3_Id == user.Id)?
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TitleAndCount,
                });

            ViewData["Worktypes"] = Enum.GetValues(typeof(WorkType)).Cast<WorkType>()
    .Select(x => new SelectListItem
    {
        Value = x.ToString(),
        Text = Enum.GetName(typeof(WorkType), x)
    }).ToAsyncEnumerable();


            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            var result = await this._jobsService.Create(input, user);

            if (result.Success)
            {
                _baseService.ToastNotify(ToastMessageState.Warning, "Внимание", "Моля изчакайте заявката ви да се прегледа от администратор.", 7000);
                _baseService.ToastNotify(ToastMessageState.Success, "Успешно", "Обявата ви е добавена.", 5000);
                return Redirect($"/identity/Jobs/Posts");
            }

            return this.View(input);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var job = await _jobsService.GetByIdAsync<JobsViewModel>(id);
            if (job == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                string[] items;
                items = job.resumeFilesId?.Split(',');

                ViewData["resumeFiles"] = items != null ? _resumeService.GetAllByQueryable(user)
                .Where(x => !(((IList)items).Contains(x.Id.ToString())))
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Title,
                }) : null;
            }

            return this.View(job);
        }

        [Authorize(Roles = "Admin, Moderator")]
        public async Task<ActionResult> Approve(int id)
        {
            var result = await this._baseService.Approve(id, PostType.Job, 2);

            if (result.Success)
            {
                return Redirect($"/identity/admin/jobposts");
            }

            return NotFound($"You cannot approve post with ID: '{id}'.");
        }

        [Authorize]
        public async Task<ActionResult> AddToFavouriteAsync(int id, string returnUrl = "")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }

            await this._favoriteService.AddToFavourite(user, PostType.Job, id.ToString());

            if (!String.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<ActionResult> RemoveFromFavouriteAsync(int id, string returnUrl = "")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }

            await _favoriteService.RemoveFromFavourite(user, PostType.Job, id.ToString());

            if (!String.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
            //return PartialView("~/Views/Home/Partials/_QuickMenuPartial.cshtml");
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> ApplyWithResume(int jobId, JobsViewModel viewModel, string returnUrl = "")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }

            var result = await _jobsService.AddResumeFile(jobId, viewModel.resumeFilesId);

            if (result == true)
            {
                await _notifyService.Create("Резюмето ви е изпратено успешно.", $"jobs/details/{jobId}", DateTime.Now, NotifyType.Information, user);
                return Redirect(returnUrl);
            }

            if (!String.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }
        public JsonResult GetTags(string searchTerm)
        {
            var dataList = _skillsService.GetAll();

            if (searchTerm != null)
            {
                dataList = dataList.Where(x => x.Title.ToLower().Contains(searchTerm.ToLower()));
            }

            var dataList2 = dataList.Select(x => new
            {
                id = x.Id,
                text = x.Title
            });


            return Json(dataList2);
        }
        public JsonResult GetLang(string searchTerm)
        {
            var dataList = _langService.GetAll();

            if (searchTerm != null)
            {
                dataList = dataList.Where(x => x.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            var dataList2 = dataList.Select(x => new
            {
                id = x.Id,
                text = x.Name
            });


            return Json(dataList2);
        }

        public JsonResult GetCompany(string searchTerm)
        {
            var dataList = _companyService.GetAll();

            if (searchTerm != null)
            {
                dataList = dataList.Where(x => x.Title.ToLower().Contains(searchTerm.ToLower()));
            }

            var dataList2 = dataList.Select(x => new
            {
                id = x.Id,
                text = x.Title
            });


            return Json(dataList2);
        }

        public JsonResult GetWork()
        {
            var visiblityEnum = Enum.GetValues(typeof(WorkType))
                .Cast<WorkType>()
                .ToList();

            var result = visiblityEnum.Select(x => new
            {
                id = x.GetDisplayName(), /*(int)x,*/
                text = x.GetDisplayName()

            });

            return Json(result);
        }



    }

}