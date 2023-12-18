using Pustok.Core.Models;

namespace Pustok.MVC.ViewModels
{
    public class ProductDetailViewModel
    {
        public Book Book { get; set; }
        public List<Book> RelatedBooks { get; set; }
    }
}
