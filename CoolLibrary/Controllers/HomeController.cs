using CoolLibrary.Models;
using CoolLibrary.Models.ViewModels;
using CoolLibrary.Repository.IRepository;
using CoolLibrary.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CoolLibrary.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGenreRepository _genreRepository;
        private readonly IBookRepository _bookRepository;

        public HomeController(ILogger<HomeController> logger, IGenreRepository genreRepository, IBookRepository bookRepository)
        {
            _logger = logger;
            _genreRepository = genreRepository;
            _bookRepository = bookRepository;
        }

        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Books = await _bookRepository.GetAllAsync(includeProperties: "Genre"),
                Genres = await _genreRepository.GetAllAsync()
            };
            return View(homeVM);
        }

        public async Task<IActionResult> Details(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppGlobals.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppGlobals.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(AppGlobals.SessionCart);
            }

            DetailsVM DetailsVM = new DetailsVM()
            {
                Book = await _bookRepository.FirstOrDefaultAsync(u => u.Id == id, "Genre"),
                ExistsInCart = false
            };


            foreach (var item in shoppingCartList)
            {
                if (item.BookId == id)
                {
                    DetailsVM.ExistsInCart = true;
                }
            }

            return View(DetailsVM);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppGlobals.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppGlobals.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(AppGlobals.SessionCart);
            }
            shoppingCartList.Add(new ShoppingCart { BookId = id });
            HttpContext.Session.Set(AppGlobals.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppGlobals.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppGlobals.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(AppGlobals.SessionCart);
            }

            var itemToRemove = shoppingCartList.SingleOrDefault(r => r.BookId == id);
            if (itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(AppGlobals.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
