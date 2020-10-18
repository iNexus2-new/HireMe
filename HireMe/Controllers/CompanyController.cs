using HireMe.Data;
using HireMe.Entities.Enums;
using HireMe.Entities.Models;
using HireMe.Services.Interfaces;
using HireMe.ViewModels.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HireMe.Controllers
{
    public class CompanyController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly ICompanyService _companyService;
        private readonly INotificationService _notifyService;
        private readonly IFavoritesService _favoriteService;
        private readonly IConfiguration _config;
        private readonly IBaseService _baseService;

        private readonly ILocationService _locationService;
        private readonly BaseDbContext _contextBase;

        public CompanyController(
            UserManager<User> userManager,
            ICompanyService companyService,
            INotificationService notifyService,
            IFavoritesService favoriteService,
            ILocationService locationService,
            IBaseService baseService,
            BaseDbContext contextBase,
            IConfiguration config)
        {
            _userManager = userManager;
            _companyService = companyService;
            _notifyService = notifyService;
            _favoriteService = favoriteService;
            _locationService = locationService;

            _config = config;
            _baseService = baseService;
            _contextBase = contextBase ?? throw new ArgumentNullException(nameof(contextBase));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var company = await _companyService.GetByIdAsync<CompanyViewModel>(id);

            if (company == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return this.View(company);
        }

        [Authorize(Roles = "Admin, Moderator")]
        public async Task<ActionResult> Approve(int id, string returnUrl = "")
        {
            var company = await _companyService.GetByIdAsync<CompanyViewModel>(id);
            if (company == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var user = await _userManager.FindByIdAsync(company.PosterId);
            if (user == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var result = await this._baseService.Approve(id, PostType.Company, 2);

            if (result.Success)
            {
                await _notifyService.Create("Твоята фирма е одобрена.", "identity/companies/posts", DateTime.Now, NotifyType.Information, user);

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
            return PartialView("~/Views/Home/Partials/_QuickMenuPartial.cshtml");
            /*   if (!String.IsNullOrEmpty(returnUrl))
                   return Redirect(returnUrl);
               else
                   return RedirectToAction("Index", "Home");*/
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

            return PartialView("~/Views/Home/Partials/_QuickMenuPartial.cshtml");
        }

        [Produces("application/json")]
        public JsonResult Search()
        {
            string term = HttpContext.Request.Query["term"].ToString();
            var url = _config.GetSection("MySettings").GetSection("SiteImageUrl").Value;

            var nameData = _contextBase.Users
                 .AsQueryable()
                 .Where(X => X.FirstName.Contains(term))
                 .Select(x => new
                 {
                     id = x.Id,
                     firstname = x.FirstName,
                     lastname = x.LastName,
                     picture = x.PictureName,
                     siteurl = url
                 })
                 .ToArray();

            return Json(nameData);
        }
    }
}