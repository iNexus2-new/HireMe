using HireMe.Entities.Enums;
using HireMe.Entities.Input;
using HireMe.Entities.Models;
using HireMe.Services.Core.Interfaces;
using HireMe.Services.Interfaces;
using HireMe.ViewModels.Contestants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace HireMe.Controllers
{
    public class ContestantsController : BaseController
    {
        private readonly IHttpContextAccessor _httpcontext;
        private readonly UserManager<User> _userManager;
        private readonly IBaseService _baseService;
        private readonly ICipherService _chipherService;

        private readonly INotificationService _notifyService;
        private readonly IContestantsService _contestantsService;
        private readonly ICategoriesService _categoriesService;
        private readonly ILanguageService _langService;
        private readonly ISkillsService _skillsService;
        private readonly ILocationService _locationService;
        private readonly IFavoritesService _favoriteService;

        public ContestantsController(
            IHttpContextAccessor httpContext,
            UserManager<User> userManager,
            IBaseService baseService,
            ICipherService chipherService,
            INotificationService notifyService,
            IContestantsService contestantsService,
            ICategoriesService categoriesService,
            ISkillsService skillsService,
            ILocationService locationService,
            ILanguageService langService,
            IFavoritesService favoriteService)
        {
            _httpcontext = httpContext;
            _userManager = userManager;
            _baseService = baseService;
            _chipherService = chipherService;

            _notifyService = notifyService;
            _contestantsService = contestantsService;
            _categoriesService = categoriesService;

            _skillsService = skillsService;
            _locationService = locationService;
            _langService = langService;
            _favoriteService = favoriteService;
        }


        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int Count { get; set; }
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));


        [FromQuery(Name = "name")]
        public string Name { get; set; }

        [FromQuery(Name = "category")]
        public int Category { get; set; }



        [HttpGet]
        public async Task<IActionResult> Index(ContestantViewModel viewModel, string name, int LocationId, int CategoryId, string WorkType, string userSkillsId, string LanguageId, int currentPage)
        {
            /* HttpRequest request = _httpcontext.HttpContext.Request;
             string query = request.QueryString.Value;

             string newQuery = query.Substring(query.LastIndexOf("=") + 1) ;
             var decryptedQuery = _chipherService.Decrypt(newQuery);
            */

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

            var entity = _contestantsService.GetAllAsNoTrackingMapped().Where(x => x.isApproved == 2 && (x.profileVisiblity == 0));


            if (!String.IsNullOrEmpty(name))
            {
                entity = entity.Where(s => s.Name.ToLower().Contains(name));
            }
            if (!String.IsNullOrEmpty(LanguageId))
            {
                string[] langs = LanguageId?.Split(',');
                entity = entity.Where(x => ((IList)langs).Contains(x.LanguagesId));
            }
            if (!String.IsNullOrEmpty(userSkillsId))
            {
                string[] tags = userSkillsId?.Split("%2C");
                entity = entity.Where(x => ((IList)tags).Contains(x.userSkillsId));
            }
            if (!String.IsNullOrEmpty(WorkType))
            {
                string[] type = WorkType?.Split("%2C");
                entity = entity.Where(x => ((IList)type).Contains(x.WorkType));
            }
            if (CategoryId > 0)
            {
                entity = entity.Where(x => x.CategoryId == CategoryId);
            }
            if (LocationId > 0)
            {
                entity = entity.Where(x => x.LocationId == LocationId);
            }
            else
            {
                entity = entity.OrderByDescending(x => x.Id);
            }

            Count = await entity.CountAsync().ConfigureAwait(false);

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

                ViewBag.Count = Count;
                ViewBag.CurrentPage = CurrentPage;
                ViewBag.TotalPages = TotalPages;
                ViewBag.PageSize = PageSize;
                ViewData["Candidates"] = result;

            }
            else ViewData["Candidates"] = null;


            //  var entity2 = entity.Where(x => x.Id == 2);
            // ViewData["ProfileModal"] = entity2;


            return View(viewModel);
        }

        [Authorize(Roles = "Admin, Moderator, User")]
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

            ViewData["Worktypes"] = Enum.GetValues(typeof(WorkType)).Cast<WorkType>()
.Select(x => new SelectListItem
{
    Value = x.ToString(),
    Text = Enum.GetName(typeof(WorkType), x)
}).ToAsyncEnumerable();

            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateContestantInputModel input)
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

            if (input.FormFile != null)
                input.ResumeFileId = await _baseService.UploadFileAsync(input.FormFile, user);

            var result = await this._contestantsService.Create(input, user);

            if (result.Success)
            {
                _baseService.ToastNotify(ToastMessageState.Warning, "Внимание", "Моля изчакайте заявката ви да се прегледа от администратор.", 7000);
                _baseService.ToastNotify(ToastMessageState.Success, "Успешно", "Кандидатурата ви е добавена.", 5000);
                return Redirect($"/identity/contestant/posts");
            }

            return this.View(input);
        }


        /*
        public async Task<IActionResult> Details(int id)
        {
            var contestant = await _contestantsService.GetByIdAsync<ContestantViewModel>(id);
            if (contestant == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return PartialView("~/Views/Contestants/Partials/_DetailsPartial.cshtml", contestant);
        }
        */
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var contestant = await _contestantsService.GetByIdAsync<ContestantViewModel>(id);
            if (contestant == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return this.View(contestant);
        }


        [Authorize(Roles = "Admin, Moderator")]
        public async Task<ActionResult> Approve(int id, string returnUrl = "")
        {
            var contestant = await _contestantsService.GetByIdAsync<ContestantViewModel>(id);
            if (contestant == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var user = await _userManager.FindByIdAsync(contestant.PosterID);
            if (user == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var result = await this._baseService.Approve(id, PostType.Contestant, 2);

            if (result.Success)
            {
                await _notifyService.Create("Твоята кандидатура е одобрена.", "identity/contestant/posts", DateTime.Now, NotifyType.Information, user);
                await _userManager.AddToRoleAsync(user, "Contestant");
                await _userManager.RemoveFromRoleAsync(user, "User");
                await _userManager.UpdateAsync(user);

                return Redirect(returnUrl);
            }

            if (!String.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
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
        }

        // SELECT2 GET
        public JsonResult GetCategory()
        {
            var dataList = _categoriesService.GetAll();


            var dataList2 = dataList.Select(x => new
            {
                id = x.Id,
                text = x.Title_BG
            });


            return Json(dataList2);
        }

        public JsonResult GetLocation()
        {
            var dataList = _locationService.GetAll();

            var dataList2 = dataList.Select(x => new
            {
                id = x.Id,
                text = x.City
            });


            return Json(dataList2);
        }

        public JsonResult GetWorktype()
        {
            var workEnum = Enum.GetValues(typeof(WorkType))
                .Cast<WorkType>()
                .ToList();

            var result = workEnum.Select(x => new
            {
                id = x.GetDisplayName(), /*(int)x,*/
                text = x.GetDisplayName()

            });

            return Json(result);
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

        public JsonResult GetTags(string searchTerm)
        {
            var dataList = _skillsService.GetAll();
            if (searchTerm != null)
            {
                dataList = dataList.Where(x => x.Title.ToLower().Contains(searchTerm.ToLower()));
            }

            var modifiedData = dataList.Select(x => new
            {
                id = x.Id,
                text = x.Title
            });

            return Json(modifiedData);
        }

    }
}