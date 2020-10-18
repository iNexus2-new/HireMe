using HireMe.Entities.Enums;
using HireMe.Entities.Models;
using HireMe.Services.Interfaces;
using HireMe.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace HireMe.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly INotificationService _notifyService;
        private readonly IBaseService _baseService;
        private readonly ISenderService _senderService;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            INotificationService notifyService,
            IBaseService baseService,
            ILogger<RegisterModel> logger,
            ISenderService senderService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notifyService = notifyService;
            _baseService = baseService;
            _logger = logger;
            _senderService = senderService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }


        public class InputModel
        {
            [Required]
            [EmailAddress]
            [EmailUserUnique]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {

                var user = new User
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    isEmployer = false,
                    PictureName = @"default-avatar.png"
                };



                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var email = await _userManager.GetEmailAsync(user);

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _senderService.SendEmailAsync(email,
         "Потвърди емайл адрес",
         $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Потвърждаване</a>.");

                    if (_userManager.Users.Count() == 1)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                        await _userManager.RemoveFromRoleAsync(user, "User");
                    }
                    else
                    {
                        
                        await _userManager.AddToRoleAsync(user, "User");
                    }


                    await _notifyService.Create("Моля попълнете личните си данни.", "identity/account/manage/editprofile", DateTime.Now, NotifyType.Information, user);
                    _baseService.ToastNotify(ToastMessageState.Alert, "Детайли", "Моля попълнете личните си данни.", 9000);
                    _baseService.ToastNotify(ToastMessageState.Success, "Успешно", "се регистрирахте. Благодарим ви за отделеното време !", 5000);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();

        }
    }
}
