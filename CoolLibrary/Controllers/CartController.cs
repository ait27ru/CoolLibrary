using CoolLibrary.Models;
using CoolLibrary.Models.ViewModels;
using CoolLibrary.Repository.IRepository;
using CoolLibrary.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoolLibrary.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBorrowingRepository _borrowingRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public BookUserVM BookUserVM { get; set; }
        public CartController(IBookRepository bookRepository, IBorrowingRepository borrowingRepository, IApplicationUserRepository applicationUserRepository, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _bookRepository = bookRepository;
            _borrowingRepository = borrowingRepository;
            _applicationUserRepository = applicationUserRepository;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> Index()
        {

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if ((HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppGlobals.SessionCart)?.Count() ?? 0) > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(AppGlobals.SessionCart);
            }

            List<int> bookInCart = shoppingCartList.Select(i => i.BookId).ToList();
            IEnumerable<Book> bookList = await _bookRepository.GetAllAsync(u => bookInCart.Contains(u.Id));

            return View(bookList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Summary));
        }


        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if ((HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppGlobals.SessionCart)?.Count() ?? 0) > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(AppGlobals.SessionCart);
            }

            List<int> bookInCart = shoppingCartList.Select(i => i.BookId).ToList();
            IEnumerable<Book> bookList = await _bookRepository.GetAllAsync(u => bookInCart.Contains(u.Id));


            BookUserVM = new BookUserVM()
            {
                ApplicationUser = await _applicationUserRepository.FirstOrDefaultAsync(u => u.Id == claim.Value),
                BookList = bookList.ToList()
            };

            return View(BookUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(BookUserVM BookUserVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            StringBuilder bookListSB = new StringBuilder();

            using var tran = await _borrowingRepository.BeginTransactionAsync();
            try
            {
                foreach (var book in BookUserVM.BookList)
                {
                    var bookFromDb = await _bookRepository.FindAsync(book.Id);

                    if ((bookFromDb?.Quantity ?? 0) > 0)
                    {
                        bookFromDb.Quantity -= 1;

                        var borrowing = new Borrowing
                        {
                            ApplicationUserId = claim.Value,
                            BookId = book.Id,
                            BorrowDate = DateTime.Now,
                        };
                        await _borrowingRepository.AddAsync(borrowing);

                        bookListSB.Append($" - {book.Title} ({book.PublishedYear}), {book.Author} <br />");
                    }
                }
                await _borrowingRepository.SaveAsync();
                await tran.CommitAsync();
            }
            catch (Exception)
            {
                await tran.RollbackAsync();
                throw;
            }

            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() +
                "Notification.html";

            var subject = "Library notification";
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = await sr.ReadToEndAsync();
            }

            string messageBody = string.Format(HtmlBody,
                bookListSB.ToString());

            await _emailSender.SendEmailAsync(BookUserVM.ApplicationUser.Email, subject, messageBody);

            return RedirectToAction("BorrowingConfirmation");
        }
        public IActionResult BorrowingConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult Remove(int id)
        {

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if ((HttpContext.Session.Get<IEnumerable<ShoppingCart>>(AppGlobals.SessionCart)?.Count() ?? 0) > 0)
            {
                //session exsits
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(AppGlobals.SessionCart);
            }

            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.BookId == id));
            HttpContext.Session.Set(AppGlobals.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }
    }
}
