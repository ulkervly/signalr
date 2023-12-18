﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Core.Models;

namespace Pustok.MVC.Controllers
{
	public class ChatController : Controller
	{
        private readonly UserManager<AppUser> _userManager;
        public ChatController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }
    }
}
