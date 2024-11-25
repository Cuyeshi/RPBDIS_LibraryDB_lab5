using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Models;

namespace RPBDIS_LibraryDB_lab5.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public IActionResult Index()
        {
            var books = _context.Books.Include(b => b.Genre).Include(b => b.Publisher).ToList();
            return View(books);
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewBag.Genres = new SelectList(_context.Genres, "GenreId", "Name");
            ViewBag.Publishers = new SelectList(_context.Publishers, "PublisherId", "Name");
            return View();
        }

        public IActionResult Back()
        {
            return RedirectToAction("Index", "Home");
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Add(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Genres = new SelectList(_context.Genres, "GenreId", "Name", book.GenreId);
            ViewBag.Publishers = new SelectList(_context.Publishers, "PublisherId", "Name", book.PublisherId);
            return View(book);
        }

        // GET: Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }

            ViewBag.Genres = new SelectList(_context.Genres, "GenreId", "Name", book.GenreId);
            ViewBag.Publishers = new SelectList(_context.Publishers, "PublisherId", "Name", book.PublisherId);
            return View(book);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Books.Any(b => b.BookId == id))
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

            ViewBag.Genres = new SelectList(_context.Genres, "GenreId", "Name", book.GenreId);
            ViewBag.Publishers = new SelectList(_context.Publishers, "PublisherId", "Name", book.PublisherId);
            return View(book);
        }

        // GET: Books/Delete
        public IActionResult Delete(int id)
        {
            var book = _context.Books
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .FirstOrDefault(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book); // Возвращаем представление с данными книги
        }

        // POST: Books/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            // Проверяем наличие связанных данных
            var hasLoanedBooks = _context.LoanedBooks.Any(lb => lb.BookId == id);
            if (hasLoanedBooks)
            {
                ModelState.AddModelError("", "Невозможно удалить книгу, так как она связана с выданными книгами.");
                return View(book); // Возвращаем представление с ошибкой
            }

            _context.Books.Remove(book); // Удаляем книгу
            await _context.SaveChangesAsync(); // Сохраняем изменения

            return RedirectToAction(nameof(Index)); // Перенаправляем на список книг
        }
    }
}
