
using Microsoft.AspNetCore.Mvc;
using Pustok.Business.Services.Interfaces;
using Pustok.Core.Models;

namespace Pustok.MVC.Areas.Manage.Controllers
{
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public async Task<IActionResult> Index()
        {
            var genres = await _genreService.GetAllAsync();
            return View(genres);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            await _genreService.CreateAsync(genre);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            var existGenre = await _genreService.GetByIdAsync(id);
            if (existGenre == null) return NotFound();

            return View(existGenre);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            await _genreService.UpdateAsync(genre);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _genreService.Delete(id);

            return Ok(); // 200
        }
    }
}
