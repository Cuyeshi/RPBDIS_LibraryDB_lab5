using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Models;

namespace RPBDIS_LibraryDB_lab5.Controllers
{
    public class GenresController : Controller
    {
        private readonly LibraryDbContext _context;

        public GenresController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Genres
        public IActionResult Index(string nameFilter, int page = 1)
        {
            int pageSize = 10; // Количество записей на странице

            // Фильтрация
            var query = _context.Genres.AsQueryable();
            if (!string.IsNullOrEmpty(nameFilter))
            {
                query = query.Where(g => g.Name.Contains(nameFilter));
            }

            // Пагинация
            int totalGenres = query.Count();
            var genres = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Передача данных для отображения
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalGenres / (double)pageSize);
            ViewBag.NameFilter = nameFilter;

            return View(genres);
        }




        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Back()
        {
            return RedirectToAction("Index", "Home");
        }

        // POST: Genres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Genre genre)
        {
            if (ModelState.IsValid)
            {
                _context.Genres.Add(genre);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // GET: Genres/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = _context.Genres.Find(id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // POST: Genres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Genre genre)
        {
            if (id != genre.GenreId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genre);
                    _context.SaveChanges();
                }
                catch (Exception)
                {
                    if (!_context.Genres.Any(g => g.GenreId == id))
                    {
                        return NotFound();
                    }
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(genre);
        }

        // GET: Genres/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _context.Genres
                .Include(g => g.Books) // Если нужно отобразить связанные данные
                .FirstOrDefaultAsync(g => g.GenreId == id);

            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
