using HireMe.Entities.Models;
using HireMe.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HireMe.Controllers
{
    public class ResumeController : BaseController
    {
        private readonly IResumeService _resumeService;
        private readonly IJobsService _jobsService;
        private readonly UserManager<User> _userManager;

        public ResumeController(
            IResumeService resumeService,
            IJobsService jobsService,
            UserManager<User> userManager)
        {
            _resumeService = resumeService;
            _jobsService = jobsService;
            _userManager = userManager;
        }



        [Authorize]
        public async ValueTask<ActionResult> DeleteApplication(int id, string returnUrl = "")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user.");
            }

            await _jobsService.RemoveResumeFromReceived(id.ToString(), user);

            if (!String.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }
    }
}
