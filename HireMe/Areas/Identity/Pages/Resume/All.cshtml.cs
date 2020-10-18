namespace HireMe.Areas.Identity.Pages.Resume
{
    using HireMe.Core.Helpers;
    using HireMe.Entities.Enums;
    using HireMe.Entities.Input;
    using HireMe.Entities.Models;
    using HireMe.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.Threading.Tasks;

    [ValidateAntiForgeryToken]
    [Authorize]
    public partial class AllModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IResumeService _resumeService;
        private readonly IBaseService _baseService;

        public AllModel(
            UserManager<User> userManager,
            IResumeService resumeService,
            IBaseService baseService)
        {
            _userManager = userManager;
            _resumeService = resumeService;
            _baseService = baseService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel : CreateResumeInputModel
        {
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Моля влезте в системата отново или си изчистете кеш данните на браузъра.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.FormFile != null)
            {
                Input.FileId = await _baseService.UploadFileAsync(Input.FormFile, user);


                OperationResult result = await _resumeService.Create(Input.FormFile.FileName, Input.FileId, user);

                if (result.Success)
                {
                    _baseService.ToastNotify(ToastMessageState.Success, "Успешно", "файлът ви е е качен.", 2000);
                }
            }

            return Page();
        }

    }
}