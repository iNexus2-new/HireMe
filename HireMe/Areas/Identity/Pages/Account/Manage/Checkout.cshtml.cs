using HireMe.Entities.Models;
using HireMe.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace HireMe.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class CheckoutModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ICipherService _chipherService;
        private readonly IConfiguration _config;

        public CheckoutModel(UserManager<User> userManager, IConfiguration config, ICipherService chipherService)
        {
            _userManager = userManager;
            _chipherService = chipherService;
            _config = config;
        }
        [BindProperty]
        public string ClientId { get; set; }
        public IActionResult OnGet()
        {

            string key = _config.GetSection("PayPal").GetSection("ClientId").Value;
            ClientId = _chipherService.Encrypt(key);

            return Page();
        }

    }
}