using HireMe.Services.Interfaces;
using HireMe.ViewModels;
using HireMe.ViewModels.Contestants;
using HireMe.ViewModels.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Diagnostics;
using System.Linq;

namespace HireMe.Controllers
{
    [RequireHttps]
    public class HomeController : BaseController
    {
        private readonly ICategoriesService _categoriesService;

        public HomeController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        [ResponseCache(CacheProfileName = "Hourly")]
        public IActionResult Index()
        {
            var categoriesContestant = _categoriesService.GetAllAsNoTrackingMapped()
             .Select(x => new SelectListItem
             {
                 Value = x.Id.ToString(),
                 Text = x.TitleAndCountContestant,
             });
             // .Cacheable(TimeSpan.FromSeconds(60*60));


            ViewData["CategoriesCont"] = categoriesContestant;


            var categoriesJobs = _categoriesService.GetAllAsNoTrackingMapped()
             .Select(x => new SelectListItem
             {
                 Value = x.Id.ToString(),
                 Text = x.TitleAndCount,
             });
              //  .Cacheable(TimeSpan.FromSeconds(60 * 60));

            ViewData["CategoriesJobs"] = categoriesJobs;
            

            return View();
        }

        [HttpPost]
        public IActionResult Jobs(JobsViewModel jobsViewModel)
        {
            string searchStr = jobsViewModel.SearchString;
            int categoryStr = jobsViewModel.CategoryId;

            string queryString = "~/jobs?" + "SearchString=" + searchStr + '&' + "CategoryId=" + categoryStr;

            return LocalRedirect(queryString);
        }
        [HttpPost]
        public IActionResult Candidates(ContestantViewModel contestantViewModel)
        {
            string searchStr = contestantViewModel.name;
            int categoryStr = contestantViewModel.CategoryId;

            string queryString = "name=" + searchStr + '&' + "category=" + categoryStr;

            return Redirect("~/contestants?" + queryString);
        }

        public IActionResult SearchContestantCategory(int categoryId)
        {
            string queryString = "~/contestants?" + "CategoryId=" + categoryId;

            return Redirect(queryString);
        }
        public IActionResult SearchJobsCategory(int categoryId)
        {
            string queryString = "~/jobs?" + "CategoryId=" + categoryId;

            return Redirect(queryString);
        }
        public IActionResult SearchJobsCompany(int companyId)
        {
            string queryString = "~/jobs?" + "CompanyId=" + companyId;

            return Redirect(queryString);
        }
        public IActionResult uploadFailure(string workerName)
        {
            return View("Errors/uploadFailure");
        }
        public IActionResult NotFound()
        {
            return View("Errors/404");
        }
        
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }


        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
