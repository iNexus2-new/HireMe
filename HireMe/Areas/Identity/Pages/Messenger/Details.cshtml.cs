using HireMe.Entities.Models;
using HireMe.Services.Interfaces;
using HireMe.ViewModels.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace HireMe.Areas.Identity.Pages.Messenger
{
    [Authorize]
    public partial class DetailsModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IMessageService _messageService;

        public DetailsModel(UserManager<User> userManager, IMessageService messageService)
        {
            this._userManager = userManager;
            this._messageService = messageService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            var entity = _messageService.GetById<MessageViewModel>(id);
            if (entity == null)
            {
                return Redirect($"/Identity/Messenger/Errors/NotFound");
            }

            // use instead viewmodel cuz razor page cannot map
            //  var content = await this._messageService.GetByIdMessage(id);
            ViewData["Message"] = entity;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // use instead viewmodel cuz razor page cannot map
            var entity = _messageService.GetById<MessageViewModel>(id);
            if (entity == null)
            {
                return Redirect($"/Identity/Messenger/Errors/NotFound");
            }

            if (entity.isRead == false)
                entity.isRead = true;

            ViewData["Message"] = entity;

            return RedirectToPage();
        }

    }
}
