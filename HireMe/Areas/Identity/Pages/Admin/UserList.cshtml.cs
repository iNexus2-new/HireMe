namespace HireMe.Areas.Identity.Pages.Admin
{
    using HireMe.Data;
    using HireMe.Entities.Models;
    using HireMe.Services.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System.Linq;
    using System.Threading.Tasks;


    public partial class UserListModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IAccountsService accountsService;
        private readonly BaseDbContext baseContext;

        public UserListModel(
            UserManager<User> userManager,
            IAccountsService accountsService,
            BaseDbContext basecontext)
        {
            this.userManager = userManager;
            this.accountsService = accountsService;
            this.baseContext = basecontext;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

            }

            var users = this.accountsService.GetAllUsers();
            // var userViewModels = this.mapper.Map<List<UserViewModel>>(users);
            //  this.ViewData["Users"] = userViewModels;

            return Page();
        }
        public IActionResult OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            var users = this.accountsService.GetAllUsers();
            // var userViewModels = this.mapper.Map<List<UserViewModel>>(users);
            // this.ViewData["Users"] = userViewModels;

            return RedirectToPage();
        }
        public IActionResult DemoteUser(string userId)
        {
            var user = this.userManager.Users.FirstOrDefault(u => u.Id == userId);

            var role = this.userManager.GetRolesAsync(user).GetAwaiter().GetResult();

            var userRole = this.baseContext.UserRoles.FirstOrDefault(ur => ur.UserId == userId);
            this.baseContext.UserRoles.Remove(userRole);

            this.userManager.AddToRoleAsync(user, "User").GetAwaiter().GetResult();
            return Page();
        }

    }

}
