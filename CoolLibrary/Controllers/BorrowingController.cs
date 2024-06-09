using CoolLibrary.Models;
using CoolLibrary.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoolLibrary.Controllers
{
    [Authorize]
    public class BorrowingController : Controller
    {
        private readonly IBorrowingRepository _borrowingRepository;
        private readonly IBookRepository _bookRepository;

        public BorrowingController(IBorrowingRepository borrowingRepository, IBookRepository bookRepository)
        {
            _borrowingRepository = borrowingRepository;
            _bookRepository = bookRepository;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var nameClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = claimsIdentity.FindFirst(ClaimTypes.Role);

            var objList = await _borrowingRepository.GetAllAsync(
                i => i.ApplicationUserId == nameClaim.Value || roleClaim.Value == AppGlobals.AdminRole,
                i => i.OrderBy(b => b.ReturnDate),
                "Book,ApplicationUser");

            return View(objList);
        }

        //GET - EDIT
        [Authorize(Roles = AppGlobals.AdminRole)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = await _borrowingRepository.FindAsync(id.Value);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST - EDIT
        [Authorize(Roles = AppGlobals.AdminRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Borrowing obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.ReturnDate != null)
                {
                    using var tran = await _borrowingRepository.BeginTransactionAsync();
                    try
                    {
                        var bookFromDb = await _bookRepository.FindAsync(obj.BookId);

                        if (bookFromDb != null)
                        {
                            bookFromDb.Quantity++;
                            await _borrowingRepository.UpdateAsync(obj);
                            await _borrowingRepository.SaveAsync();
                        }
                        await tran.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await tran.RollbackAsync();
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(obj);
        }
    }
}
