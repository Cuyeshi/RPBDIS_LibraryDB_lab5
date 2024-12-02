using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Models;

namespace RPBDIS_LibraryDB_lab5.Controllers
{
    public class PublishersController : Controller
    {
        private readonly LibraryDbContext _context;

        public PublishersController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Publishers
        public async Task<IActionResult> Index(string nameFilter, string cityFilter, int page = 1)
        {
            int pageSize = 10;

            // Применяем фильтры
            var publishers = _context.Publishers.AsQueryable();

            if (!string.IsNullOrEmpty(nameFilter))
            {
                publishers = publishers.Where(p => p.Name.Contains(nameFilter));
            }
            if (!string.IsNullOrEmpty(cityFilter))
            {
                publishers = publishers.Where(p => p.City.Contains(cityFilter));
            }

            // Сохраняем параметры фильтрации для передачи в представление
            ViewBag.NameFilter = nameFilter;
            ViewBag.CityFilter = cityFilter;

            // Подсчитываем общее количество страниц
            int totalItems = await publishers.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.CurrentPage = page;

            // Пагинация
            var paginatedPublishers = await publishers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(paginatedPublishers);
        }


        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Publisher publisher)
        {
            if (ModelState.IsValid)
            {
                _context.Publishers.Add(publisher);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(publisher);
        }

        // GET: Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = _context.Publishers.Find(id);
            if (publisher == null)
            {
                return NotFound();
            }
            return View(publisher);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Publisher publisher)
        {
            if (id != publisher.PublisherId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(publisher);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Publishers.Any(p => p.PublisherId == id))
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
            return View(publisher);
        }

        // GET: Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = _context.Publishers.FirstOrDefault(p => p.PublisherId == id);
            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var publisher = _context.Publishers.Find(id);
            if (publisher != null)
            {
                _context.Publishers.Remove(publisher);
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
