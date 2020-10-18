using HireMe.Core.Helpers;
using HireMe.Entities.Enums;
using HireMe.Entities.Models;
using HireMe.Services.Interfaces;
using HireMe.ViewModels.Message;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace HireMe.Areas.Identity.Pages.Messenger
{
    public partial class DeleteModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IMessageService _messageService;
        private readonly IBaseService _baseService;
        public DeleteModel(
            IMessageService messageService,
            IBaseService baseService,
            UserManager<User> userManager)
        {
            this._messageService = messageService;
            this._userManager = userManager;
            this._baseService = baseService;
        }

        [BindProperty]
        public Message Message { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var entity = await _messageService.GetByIdAsync<MessageViewModel>(id);

            if (entity == null)
            {
                return Redirect($"/Identity/Messenger/Errors/NotFound");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
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

            OperationResult result = await this._messageService.Delete(id, user.Id);

            if (result.Success)
            {
                _baseService.ToastNotify(ToastMessageState.Info, "Съобщението", "е изтрито.", 2000);
            }

            return Redirect($"/Identity/Messenger/");
        }

    }
}
