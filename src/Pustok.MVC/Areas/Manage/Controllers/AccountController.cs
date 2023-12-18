using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Pustok.Core.Models;
using Pustok.MVC.Areas.ViewModels;
using Pustok.MVC.ViewModels;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel adminLoginVM)
        {
            if (!ModelState.IsValid) return View(adminLoginVM);

            AppUser admin = null;

            admin = await _userManager.FindByNameAsync(adminLoginVM.Username);

            if (admin is null)
            {
                ModelState.AddModelError("", "Invalid username or password!");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(admin, adminLoginVM.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid username or password!");
                return View();
            }

            return RedirectToAction("Index", "home");
        }
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(MemberRegisterViewModel memberRegisterVM)
		{
			if (!ModelState.IsValid) return View();

			AppUser user = null;

			user = await _userManager.FindByNameAsync(memberRegisterVM.Username);

			if (user != null)
			{
				ModelState.AddModelError("Username", "Username already exist!");
				return View();
			}

			user = await _userManager.FindByEmailAsync(memberRegisterVM.Email);
			if (user != null)
			{
				ModelState.AddModelError("Email", " Email already exist!");
				return View();
			}

			AppUser appUser = new AppUser()
			{
				FullName = memberRegisterVM.Fullname,
				UserName = memberRegisterVM.Username,
				Email = memberRegisterVM.Email
			};

			var result = await _userManager.CreateAsync(appUser, memberRegisterVM.Password);

			if (!result.Succeeded)
			{
				foreach (var err in result.Errors)
				{
					ModelState.AddModelError("", err.Description);
				}
			}

			await _userManager.AddToRoleAsync(appUser, "Member");

			await _signInManager.SignInAsync(appUser, isPersistent: false);

			return RedirectToAction("index", "home");
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("index", "home");
		}
	}
}
