using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Core.Models;

namespace Pustok.MVC.Areas.Manage.Controllers
{
    [Area("Manage")]
	[Authorize(Roles ="SuperAdmin,Admin")]


    public class DashboardController : Controller
    {
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public DashboardController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}
		public IActionResult Index()
        {
            return View();
        }
   
    }
}
