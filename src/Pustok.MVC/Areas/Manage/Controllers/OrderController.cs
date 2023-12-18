using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Pustok.Business.Hubs;
using Pustok.Core.Models;
using Pustok.Data.DAL;
using Pustok.PaginationHelper;
using System;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class OrderController : Controller
    {
        private readonly PustokContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(PustokContext context, IHubContext<ChatHub> hubContext, UserManager<AppUser> userManager)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var query = _context.Orders.AsQueryable();
            //List<Order> orders = await _context.Orders.ToListAsync();
            PaginatedList<Order> paginatedOrder = PaginatedList<Order>.Create(query, page, 1);
            return View(paginatedOrder);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Order order = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);
            if (order is null) return NotFound();

            return View(order);
        }

        public async Task<IActionResult> Accept(int id)
        {
            Order order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (order is null) return NotFound();
            order.OrderStatus = Core.Enums.OrderStatus.Accepted;

            await _context.SaveChangesAsync();

            if (order.AppUserId != null)
            {

                var user = await _userManager.FindByIdAsync(order.AppUserId);
                if (user != null)
                {
                    await _hubContext.Clients.Client(user.ConnectionId).SendAsync("OrderAccepted");
                }
            }

            return RedirectToAction("index", "order");
        }

        public async Task<IActionResult> Reject(int id, string AdminComment)
        {

            Order order = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);
            if (order is null) return NotFound();
            if (AdminComment == null)
            {
                ModelState.AddModelError("AdminComment", "Must be written");
                return View("detail", order);
            }

            order.OrderStatus = Core.Enums.OrderStatus.Rejected;
            order.AdminComment = AdminComment;

            await _context.SaveChangesAsync();

            if (order.AppUserId != null)
            {

                var user = await _userManager.FindByIdAsync(order.AppUserId);
                if (user != null)
                {
                    await _hubContext.Clients.Client(user.ConnectionId).SendAsync("OrderRejected", AdminComment);
                }
            }

            return RedirectToAction("index", "order");
        }
    }
}
