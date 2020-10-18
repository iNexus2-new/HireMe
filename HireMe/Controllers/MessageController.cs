using HireMe.Core.Filters;
using HireMe.Entities.Enums;
using HireMe.Entities.Models;
using HireMe.Services.Interfaces;
using HireMe.ViewModels.Contestants;
using HireMe.ViewModels.Jobs;
using HireMe.ViewModels.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HireMe.Controllers
{
    [ValidateModel]
    [Authorize]
    public class MessageController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly IMessageService _messageService;

        public MessageController(UserManager<User> userManager, IMessageService messageService)
        {
            this._userManager = userManager;
            this._messageService = messageService;
        }

        [HttpPost]
        public async Task<IActionResult> Send(string receiver, ContestantViewModel input)
        {
            var senderId = _userManager.GetUserId(User);

            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            await this._messageService.Create(input.MessageInputModel.Title, input.MessageInputModel.Description, senderId, receiver);
            return View(input);
        }

        [HttpPost]
        public async Task<IActionResult> Report(string postName, JobsViewModel input)
        {
            var senderId = _userManager.GetUserId(User);
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            foreach (var item in _userManager.GetUsersInRoleAsync("Admin").Result)
            {
                await this._messageService.CreateReport(postName, input.Message.Description, senderId, item.Id);
            }

            return LocalRedirect($"/Jobs/Details/{input.Id}");
        }
        /*
        [Authorize]
        public IActionResult AddToStared(int id)
        {
                var msg = this._messageService.GetByIdMessage(id);

                var result = this._messageService.Add_MessageState(id, MessageStates.Stared, msg.isStared ? false : true);

                if (result.Success)
                {
                    return Redirect($"/identity/messenger/index");
                }
            return Redirect($"/identity/messenger/index");
        }

        [Authorize]
        public IActionResult AddToImportant(int id)
        {
            var msg = this._messageService.GetByIdMessage(id);

            if (this.ModelState.IsValid)
            {
                var result = this._messageService.Add_MessageState(id, MessageStates.Important, msg.isImportant ? false : true);

                if (result.Success)
                {
                    return Redirect($"/identity/messenger/index");
                }
            }
            return Redirect($"/identity/messenger/index");
        }
        */
        public async Task<ActionResult> Details(int id)
        {
            var entity = await _messageService.GetByIdAsync<MessageViewModel>(id);
            if (entity == null)
            {
                return Redirect($"/Identity/Messenger/Errors/NotFound");
            }

            await _messageService.Add_MessageState(id, MessageStates.Read, true);

            return Redirect($"/identity/messenger/details/{id}");
        }

    }
}
