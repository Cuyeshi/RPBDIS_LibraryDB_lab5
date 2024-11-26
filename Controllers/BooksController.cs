using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Models;
using System.Linq;

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
        public IActionResult Index(string titleFilter, string authorFilter, int? genreId, int? publisherId, int page = 1)
        {
            int pageSize = 10; // Количество записей на странице

            // Базовый запрос
            var booksQuery = _context.Books
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .AsQueryable();

            // Применение фильтров
            if (!string.IsNullOrWhiteSpace(titleFilter))
            {
                booksQuery = booksQuery.Where(b => b.Title.Contains(titleFilter));
            }
            if (!string.IsNullOrWhiteSpace(authorFilter))
            {
                booksQuery = booksQuery.Where(b => b.Author.Contains(authorFilter));
            }
            if (genreId.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.GenreId == genreId);
            }
            if (publisherId.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.PublisherId == publisherId);
            }

            // Пагинация
            var totalItems = booksQuery.Count();
            var books = booksQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Передача данных в представление
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.Genres = new SelectList(_context.Genres, "GenreId", "Name");
            ViewBag.Publishers = new SelectList(_context.Publishers, "PublisherId", "Name");
            ViewBag.TitleFilter = titleFilter;
            ViewBag.AuthorFilter = authorFilter;
            ViewBag.SelectedGenreId = genreId;
            ViewBag.SelectedPublisherId = publisherId;

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

            // Для отладки
            Console.WriteLine($"Удаляем книгу с ID {id}");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Книга с ID {id} успешно удалена.");
            return RedirectToAction(nameof(Index));
        }
    }
}
