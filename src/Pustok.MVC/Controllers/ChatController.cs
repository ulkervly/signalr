using Microsoft.AspNetCore.Mvc;

namespace Pustok.MVC.Controllers
{
	public class ChatController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
