using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Data;
using RPBDIS_LibraryDB_lab5.Models;

namespace RPBDIS_LibraryDB_lab5.Controllers
{
    [Authorize()]
    public class LoanedBooksController : Controller
    {
        private readonly LibraryDbContext _context;

        public LoanedBooksController(LibraryDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string bookTitleFilter, DateTime? loanDateFilter, int page = 1)
        {
            int pageSize = 7; // Количество записей на странице

            // Фильтрация
            var query = _context.LoanedBooks
                .Include(lb => lb.Book)
                .ThenInclude(b => b.Genre)
                .Include(lb => lb.Reader)
                .Include(lb => lb.Employee)
                .AsQueryable();

            if (!string.IsNullOrEmpty(bookTitleFilter))
            {
                query = query.Where(lb => lb.Book.Title.Contains(bookTitleFilter));
            }
            

            // Пагинация
            int totalLoanedBooks = query.Count();
            var loanedBooks = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Передача данных для отображения
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalLoanedBooks / (double)pageSize);
            ViewBag.BookTitleFilter = bookTitleFilter;
            ViewBag.LoanDateFilter = loanDateFilter?.ToString("yyyy-MM-dd");

            return View(loanedBooks);
        }



        // GET: LoanedBooks/Create
        public IActionResult Create()
        {
            ViewData["Books"] = new SelectList(_context.Books, "BookId", "Title");
            ViewData["Readers"] = new SelectList(_context.Readers, "ReaderId", "FullName");
            ViewData["Employees"] = new SelectList(_context.Employees, "EmployeeId", "FullName");
            return View();
        }



        // POST: LoanedBooks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,ReaderId,LoanDate,ReturnDate,Returned,EmployeeId")] LoanedBook loanedBook)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine($"BookId: {loanedBook.BookId}, EmployeeId: {loanedBook.EmployeeId}");
                _context.Add(loanedBook);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Перезагрузка ViewData на случай ошибок
            ViewData["Books"] = new SelectList(_context.Books, "BookId", "Title", loanedBook.BookId);
            ViewData["Readers"] = new SelectList(_context.Readers, "ReaderId", "FullName", loanedBook.ReaderId);
            ViewData["Employees"] = new SelectList(_context.Employees, "EmployeeId", "FullName", loanedBook.EmployeeId);

            return View(loanedBook);
        }

        // GET: Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanedBook = _context.LoanedBooks.Find(id);
            if (loanedBook == null)
            {
                return NotFound();
            }

            // Заполнение ViewBag
            ViewBag.Books = new SelectList(_context.Books, "BookId", "Title");
            ViewBag.Readers = new SelectList(_context.Readers, "ReaderId", "FullName");
            ViewBag.Employees = new SelectList(_context.Employees, "EmployeeId", "FullName");

            return View(loanedBook);

        }

        // POST: Edit
        // POST: LoanedBooks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoanId,BookId,ReaderId,LoanDate,ReturnDate,Returned,Employee,EmployeeId")] LoanedBook loanedBook)
        {
            if (id != loanedBook.LoanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loanedBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanedBookExists(loanedBook.LoanId))
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

            // Повторная загрузка данных для списков в случае ошибки
            ViewBag.Books = new SelectList(_context.Books, "BookId", "Title", loanedBook.BookId);
            ViewBag.Readers = new SelectList(_context.Readers, "ReaderId", "FullName", loanedBook.ReaderId);
            ViewBag.Employees = new SelectList(_context.Employees, "EmployeeId", "FullName", loanedBook.EmployeeId);

            return View(loanedBook);
        }

        private bool LoanedBookExists(int id)
        {
            return _context.LoanedBooks.Any(e => e.LoanId == id);
        }


        // GET: Delete
        public IActionResult Delete(int id)
        {
            var loanedBook = _context.LoanedBooks.Include(lb => lb.Book).FirstOrDefault(lb => lb.LoanId == id);
            if (loanedBook == null) return NotFound();

            return View(loanedBook);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loanedBook = await _context.LoanedBooks.FindAsync(id);
            if (loanedBook == null)
            {
                return NotFound();
            }

            _context.LoanedBooks.Remove(loanedBook);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); // После удаления перенаправляем на список
        }

        public IActionResult Back()
        {
            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public async Task<IActionResult> SearchBooks(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<object>());
            }

            var books = await _context.Books
                .Where(b => b.Title.Contains(term))
                .Select(b => new { b.BookId, b.Title })
                .ToListAsync();

            return Json(books.Select(b => new { id = b.BookId, title = b.Title }));
        }
    }
}
