using CoolLibrary.Models;
using CoolLibrary.Models.ViewModels;
using CoolLibrary.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolLibrary.Controllers
{
    [Authorize(Roles = AppGlobals.AdminRole)]
    public class BookController : Controller
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IBookRepository _bookRepository;
        public BookController(IGenreRepository genreRepository, IBookRepository bookRepository)
        {
            _genreRepository = genreRepository;
            _bookRepository = bookRepository;
        }


        public async Task<IActionResult> Index()
        {
            IEnumerable<Book> bookList = await _bookRepository.GetAllAsync(includeProperties: "Genre");
            return View(bookList);
        }


        //GET - UPSERT
        public async Task<IActionResult> Upsert(int? id)
        {
            var genreList = await _genreRepository.GetAllAsync();

            BookVM bookVM = new BookVM()
            {
                Book = new Book(),
                GenreSelectList = genreList.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id == null)
            {
                // create book
                return View(bookVM);
            }
            else
            {
                // update book
                bookVM.Book = await _bookRepository.FindAsync(id.Value);

                if (bookVM.Book == null)
                {
                    return NotFound();
                }

                return View(bookVM);
            }
        }


        //POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(BookVM bookVM)
        {
            if (ModelState.IsValid)
            {
                if (bookVM.Book.Id == 0)
                {
                    await _bookRepository.AddAsync(bookVM.Book);
                }
                else
                {
                    await _bookRepository.UpdateAsync(bookVM.Book);
                }

                await _bookRepository.SaveAsync();

                return RedirectToAction("Index");
            }
            var genreList = await _genreRepository.GetAllAsync();
            bookVM.GenreSelectList = genreList.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            return View(bookVM);
        }



        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var book = await _bookRepository.FirstOrDefaultAsync(i => i.Id == id, "Genre");

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        //POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var obj = await _bookRepository.FindAsync(id.GetValueOrDefault());

            if (obj == null)
            {
                return NotFound();
            }

            _bookRepository.Remove(obj);
            await _bookRepository.SaveAsync();

            return RedirectToAction("Index");
        }
    }
}
