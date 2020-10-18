using HireMe.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireMe.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public partial class FavoriteListModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public FavoriteListModel(UserManager<User> userManager)
        {
            this._userManager = userManager;
        }

    }
}
