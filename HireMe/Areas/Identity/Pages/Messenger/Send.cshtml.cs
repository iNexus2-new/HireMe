using HireMe.Core.Helpers;
using HireMe.Entities.Enums;
using HireMe.Entities.Models;
using HireMe.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace HireMe.Areas.Identity.Pages.Messenger
{
    [Authorize]
    public partial class SendModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IMessageService _messageService;
        private readonly IBaseService _baseService;

        public SendModel(
            UserManager<User> userManager,
            IBaseService baseService,
            IMessageService messageService)
        {
            this._userManager = userManager;
            this._messageService = messageService;
            this._baseService = baseService;
        }

        [BindProperty]
        public Message Message { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Моля влезте в системата отново или си изчистете кеш данните на браузъра.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userid = user.Id;

            if (user == null)
            {
                return NotFound($"Моля влезте в системата отново или си изчистете кеш данните на браузъра.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            OperationResult result = await this._messageService.Create(Message.Title, Message.Description, userid, Message.ReceiverId);

            if (result.Success)
            {
                _baseService.ToastNotify(ToastMessageState.Success, "Успешно", "Обявата ви е актулизирана.", 2000);

            }

            return RedirectToPage();
        }

    }

}
