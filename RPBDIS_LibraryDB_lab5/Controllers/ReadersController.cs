using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Data;
using RPBDIS_LibraryDB_lab5.Models;

namespace RPBDIS_LibraryDB_lab5.Controllers
{
    [Authorize()]
    public class ReadersController : Controller
    {
        private readonly LibraryDbContext _context;

        public ReadersController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Readers
        public IActionResult Index(string nameFilter, string genderFilter, int page = 1)
        {
            int pageSize = 7; // Количество записей на странице

            // Передаем фильтры в ViewBag для отображения
            ViewBag.NameFilter = nameFilter;
            ViewBag.GenderFilter = genderFilter;
            ViewBag.CurrentPage = page;

            // Фильтрация данных
            var readers = _context.Readers.AsQueryable();

            if (!string.IsNullOrEmpty(nameFilter))
            {
                readers = readers.Where(r => r.FullName.Contains(nameFilter));
            }

            if (!string.IsNullOrEmpty(genderFilter))
            {
                readers = readers.Where(r => r.Gender == genderFilter);
            }

            // Пагинация
            int totalReaders = readers.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalReaders / pageSize);

            var paginatedReaders = readers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(paginatedReaders);
        }


        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reader reader)
        {
            if (ModelState.IsValid)
            {
                _context.Readers.Add(reader);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(reader);
        }

        // GET: Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reader = _context.Readers.Find(id);
            if (reader == null)
            {
                return NotFound();
            }

            return View(reader);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Reader reader)
        {
            if (id != reader.ReaderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reader);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Readers.Any(r => r.ReaderId == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reader);
        }

        // GET: Delete
        public IActionResult Delete(int id)
        {
            var reader = _context.Readers.FirstOrDefault(r => r.ReaderId == id);
            if (reader == null)
            {
                return NotFound();
            }

            return View(reader);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var reader = _context.Readers.Find(id);
            if (reader != null)
            {
                _context.Readers.Remove(reader);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Back()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
