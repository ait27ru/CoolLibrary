using CoolLibrary.Models;
using CoolLibrary.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoolLibrary.Controllers
{
    [Authorize(Roles = AppGlobals.AdminRole)]
    public class GenreController : Controller
    {
        private readonly IGenreRepository _genreRepository;

        public GenreController(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<IActionResult> Index()
        {
            var objList = await _genreRepository.GetAllAsync();
            return View(objList);
        }


        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }


        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Genre obj)
        {
            if (ModelState.IsValid)
            {
                await _genreRepository.AddAsync(obj);
                await _genreRepository.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }


        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = await _genreRepository.FindAsync(id.Value);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Genre obj)
        {
            if (ModelState.IsValid)
            {
                await _genreRepository.UpdateAsync(obj);
                await _genreRepository.SaveAsync();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = await _genreRepository.FindAsync(id.Value);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = await _genreRepository.FindAsync(id.Value);

            if (obj == null)
            {
                return NotFound();
            }
            _genreRepository.Remove(obj);
            await _genreRepository.SaveAsync();
            return RedirectToAction("Index");
        }
    }
}
