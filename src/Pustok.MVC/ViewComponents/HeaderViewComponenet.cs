using Microsoft.AspNetCore.Mvc;
using Pustok.Business.Services.Interfaces;
using Pustok.Data.DAL;
using Pustok.MVC.ViewModels;

namespace Pustok.MVC.ViewComponents
{
    public class HeaderViewComponenet:ViewComponent
    {
        private readonly IGenreService _genreService;
        private readonly PustokContext _context;
        public HeaderViewComponenet(IGenreService genreService, PustokContext context)
        {
            _context = context;
            _genreService = genreService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            HeaderViewModel headerViewModel = new HeaderViewModel();

            headerViewModel.Genres = await _genreService.GetAllAsync();
            //headerViewModel.Settings = _context.Settings.ToList();
            return View(headerViewModel);
        }
    }
}
