using HireMe.Entities.Enums;
using HireMe.Entities.Models;
using HireMe.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace HireMe.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public partial class EditProfileModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly IBaseService _baseService;
        private readonly ISenderService _senderService;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;


        public EditProfileModel(
            IConfiguration config,
            IBaseService baseService,
            ISenderService senderService,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _config = config;
            _baseService = baseService;
            _senderService = senderService;

            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Display(Name = "Потребителско име")]
        public string Username { get; set; }
        public bool IsEmailConfirmed { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Емайл")]
            public string Email { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Телефон")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Име")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Фамилия")]
            public string LastName { get; set; }

            public string PictureFullPath { get; set; }

            [Display(Name = "Аватар")]
            public IFormFile FormFile { get; set; }
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Моля влезте в системата отново или си изчистете кеш данните на браузъра.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Email = email,
                PhoneNumber = phoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            string siteUrl = _config.GetSection("MySettings").GetSection("SiteImageUrl").Value;
            Input.PictureFullPath = siteUrl + user.PictureName;

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            /*
            if (!IsEmailConfirmed)
            {
                _baseService.ToastNotify(ToastMessageState.Warning, "Внимание", "Моля потвърдете вашият е-майл.", 2000);
                return Page();
            }
            */
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
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

            if (Input.FormFile != null)
            {
                user.PictureName = await _baseService.UploadImageAsync(Input.FormFile, user);
            }
            if (Input.FirstName != user.FirstName)
            {
                user.FirstName = Input.FirstName;
            }

            if (Input.LastName != user.LastName)
            {
                user.LastName = Input.LastName;
            }
            if (!user.profileConfirmed && user.EmailConfirmed)
            {
                user.profileConfirmed = true;
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }


            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            _baseService.ToastNotify(ToastMessageState.Success, "Успешно", "Детайлите са актулизирани.", 2000);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);

            await _senderService.SendEmailAsync(email,
                "Потвърди емайл адрес",
                $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Потвърждаване</a>.");

            _baseService.ToastNotify(ToastMessageState.Info, "Информация", "Емайлът за потвърждение е изпратен. Моля проверете!", 4000);
            return RedirectToPage();
        }

    }
}
