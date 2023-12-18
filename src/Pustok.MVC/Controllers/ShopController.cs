using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Data.DAL;
using Pustok.ViewModels;

namespace Pustok.Controllers
{
    public class ShopController : Controller
    {
        private readonly PustokContext _context;

        public ShopController(PustokContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? genreId)
        {
            var query = _context.Books.Include(x => x.Author).Include(x => x.BookImages).AsQueryable();

            if (genreId != null)
            {
                query = query.Where(x => x.GenreId == genreId);
            }

            ShopViewModel model = new ShopViewModel()
            {
                Books = await query.ToListAsync(),
                Genres = await _context.Genres.Include(x => x.Books).ToListAsync(),
            };

            return View(model);
        }
    }
}
