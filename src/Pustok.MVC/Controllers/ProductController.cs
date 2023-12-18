using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.Business.Services.Interfaces;
using Pustok.Core.Models;
using Pustok.Core.Repositories.Interfaces;
using Pustok.Data.DAL;
using Pustok.MVC.ViewModels;

namespace Pustok.Controllers;

public class ProductController : Controller
{
    private readonly IBookService _bookService;
    private readonly UserManager<AppUser> _userManager;
    private readonly PustokContext _context;
    private readonly IBookRepository _bookRepository;

    public ProductController(
                        IBookRepository bookRepository,
                        IBookService bookService,
                        UserManager<AppUser> userManager,
                        PustokContext context)
    {
        _bookRepository = bookRepository;
        _bookService = bookService;
        _userManager = userManager;
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Detail(int id)
    {
        Book book = await _bookService.GetByIdAsync(id);
        ProductDetailViewModel productDetailViewModel = new ProductDetailViewModel()
        {
            Book = book,
            RelatedBooks = await _bookService.GetAllRelatedBooksAsync(book)
        };

        return View(productDetailViewModel);
    }

    public async Task<IActionResult> GetBookModal(int id)
    {
        var book = await _bookService.GetByIdAsync(id);

        return PartialView("_BookModalPartial", book);
    }

    

    public async Task<IActionResult> AddToBasket(int bookId)
    {

        if (!_bookRepository.Table.Any(x => x.Id == bookId)) return NotFound(); // 404

        List<BasketItemViewModel> basketItemList = new List<BasketItemViewModel>();
        BasketItemViewModel basketItem = null;
        BasketItem userBasketItem = null;
        AppUser user = null;

        if (HttpContext.User.Identity.IsAuthenticated)
        {
            user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
        }

        if (user == null)
        {
            string basketItemListStr = HttpContext.Request.Cookies["BasketItems"];

            if (basketItemListStr != null)
            {
                basketItemList = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemListStr);

                basketItem = basketItemList.FirstOrDefault(x => x.BookId == bookId);

                if (basketItem != null)
                {
                    basketItem.Count++;
                }
                else
                {
                    basketItem = new BasketItemViewModel()
                    {
                        BookId = bookId,
                        Count = 1
                    };

                    basketItemList.Add(basketItem);
                }
            }
            else
            {
                basketItem = new BasketItemViewModel()
                {
                    BookId = bookId,
                    Count = 1
                };

                basketItemList.Add(basketItem);
            }

            basketItemListStr = JsonConvert.SerializeObject(basketItemList);

            HttpContext.Response.Cookies.Append("BasketItems", basketItemListStr);
        }
        else
        {
            userBasketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.BookId == bookId && x.AppUserId == user.Id && !x.IsDeleted);
            if (userBasketItem != null)
            {
                userBasketItem.Count++;
            }
            else
            {
                userBasketItem = new BasketItem
                {
                    BookId = bookId,
                    Count = 1,
                    AppUserId = user.Id,
                    IsDeleted = false
                };
                _context.BasketItems.Add(userBasketItem);
            }
            await _context.SaveChangesAsync();
        }

        return Ok(); //200
    }

    public IActionResult GetBasketItems()
    {
        List<BasketItemViewModel> basketItemList = new List<BasketItemViewModel>();

        string basketItemListStr = HttpContext.Request.Cookies["BasketItems"];

        if (basketItemListStr != null)
        {
            basketItemList = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemListStr);
        }
        return Json(basketItemList);
    }

    public async Task<IActionResult> Checkout()
    {
        List<CheckOutViewModel> checkoutItemList = new List<CheckOutViewModel>();
        List<BasketItemViewModel> basketItemList = new List<BasketItemViewModel>();
        List<BasketItem> userBasketItems = new List<BasketItem>();
        CheckOutViewModel checkoutItem = null;
        AppUser user = null;

        if (HttpContext.User.Identity.IsAuthenticated)
        {
            user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
        }


        if (user == null)
        {
            string basketItemListStr = HttpContext.Request.Cookies["BasketItems"];
            if (basketItemListStr != null)
            {
                basketItemList = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemListStr);

                foreach (var item in basketItemList)
                {
                    checkoutItem = new CheckOutViewModel
                    {
                        Book = await _bookRepository.GetByIdAsync(x => x.Id == item.BookId),
                        Count = item.Count
                    };
                    checkoutItemList.Add(checkoutItem);
                }
            }
        }
        else
        {
            userBasketItems = await _context.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == user.Id && !x.IsDeleted).ToListAsync();

            foreach (var item in userBasketItems)
            {
                checkoutItem = new CheckOutViewModel
                {
                    Book = item.Book,
                    Count = item.Count
                };
                checkoutItemList.Add(checkoutItem);
            }
        }

        OrderViewModel orderViewModel = new OrderViewModel
        {
            CheckoutViewModels = checkoutItemList,
            FullName = user?.FullName,
        };

        return View(orderViewModel);
    }


    [HttpPost]
    public async Task<IActionResult> Checkout(OrderViewModel orderViewModel)
    {
        if (!ModelState.IsValid) return View();
        List<CheckOutViewModel> checkoutItemList = new List<CheckOutViewModel>();
        List<BasketItemViewModel> basketItemList = new List<BasketItemViewModel>();
        List<BasketItem> userBasketItems = new List<BasketItem>();
        CheckOutViewModel checkoutItem = null;
        OrderItem orderItem = null;
        AppUser user = null;

        if (HttpContext.User.Identity.IsAuthenticated)
        {
            user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
        }

        Order order = new Order
        {
            FullName = orderViewModel.FullName,
            Country = orderViewModel.Country,
            Email = orderViewModel.Email,
            Address = orderViewModel.Address,
            Phone = orderViewModel.Phone,
            ZipCode = orderViewModel.ZipCode,
            Note = orderViewModel.Note,
            OrderItems = new List<OrderItem>(),
            AppUserId = user?.Id,
            CreatedDate = DateTime.UtcNow.AddHours(4)
        };
        if (user == null)
        {
            string basketItemListStr = HttpContext.Request.Cookies["BasketItems"];
            if (basketItemListStr != null)
            {
                basketItemList = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemListStr);

                foreach (var item in basketItemList)
                {
                    Book book = _context.Books.FirstOrDefault(x => x.Id == item.BookId);
                    orderItem = new OrderItem
                    {
                        Book = book,
                        BookName = book.Name,
                        CostPrice = book.CostPrice,
                        DiscountPercent = book.DiscountPercent,
                        SalePrice = book.SalePrice * ((100 - book.DiscountPercent) / 100),
                        Count = item.Count,
                        Order = order
                    };

                    order.TotalPrice += orderItem.SalePrice * orderItem.Count;
                    order.OrderItems.Add(orderItem);
                }
            }
        }
        else
        {
            userBasketItems = await _context.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == user.Id && !x.IsDeleted).ToListAsync();

            foreach (var item in userBasketItems)
            {
                Book book = _context.Books.FirstOrDefault(x => x.Id == item.BookId);
                orderItem = new OrderItem
                {
                    Book = book,
                    BookName = book.Name,
                    CostPrice = book.CostPrice,
                    DiscountPercent = book.DiscountPercent,
                    SalePrice = book.SalePrice * ((100 - book.DiscountPercent) / 100),
                    Count = item.Count,
                    Order = order
                };

                order.TotalPrice += orderItem.SalePrice * orderItem.Count;
                order.OrderItems.Add(orderItem);
                item.IsDeleted = true;
            }
        }

        //TODO Stripe


        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        return RedirectToAction("index", "home");
    }


    public async Task<IActionResult> SearchBooks(string value)
    {
        List<Book> searchedBooks = await _bookService.GetAllAsync(x => x.Name.ToLower().Contains(value.Trim().ToLower()));

        return Ok(searchedBooks);
    }

}