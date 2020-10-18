using HireMe.Entities.Models;
using HireMe.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HireMe.Areas.Identity.Pages.Messenger
{
    [Authorize]
    public partial class ReportsModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IMessageService _messageService;

        public ReportsModel(
            UserManager<User> userManager,
            IMessageService messageService)
        {
            this._userManager = userManager;
            this._messageService = messageService;
        }

        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public bool EnablePrevious => CurrentPage > 1;
        public bool EnableNext => CurrentPage < TotalPages;
        public IAsyncEnumerable<Message> MessageList { get; set; }

        public async Task<IActionResult> OnGetAsync(int currentPage)
        {
            var user = await _userManager.GetUserAsync(User);
            var userid = user.Id;
            if (user == null)
            {
                return NotFound($"Моля влезте в системата отново или си изчистете кеш данните на браузъра.");
            }


            var entity = _messageService.GetAllAsNoTracking().Where(x => x.ReceiverId == userid).OrderByDescending(x => x.Id);
            Count = await entity.CountAsync().ConfigureAwait(false);

            CurrentPage = currentPage == 0 ? 1 : currentPage;

            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;

            var result = entity
           .OrderByDescending(x => x.Id)
           .Skip((CurrentPage - 1) * PageSize)
           .Take(PageSize)
           .AsQueryable()
           .ToAsyncEnumerable();

            MessageList = Count > 0 ? result : null;

            return Page();
        }


    }
}
