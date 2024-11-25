using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Models;

namespace RPBDIS_LibraryDB_lab5.Controllers
{
    public class LoanedBooksController : Controller
    {
        private readonly LibraryDbContext _context;

        public LoanedBooksController(LibraryDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var loanedBooks = _context.LoanedBooks.Include(lb => lb.Book).ThenInclude(b => b.Genre).ToList();
            return View(loanedBooks);
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewBag.Books = new SelectList(_context.Books, "BookId", "Title");
            return View();
        }

        public IActionResult Back()
        {
            return RedirectToAction("Index", "Home");
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LoanedBook loanedBook)
        {
            if (ModelState.IsValid)
            {
                _context.LoanedBooks.Add(loanedBook);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // При ошибке валидации повторно загружаем данные для ViewBag
            ViewBag.Books = new SelectList(_context.Books, "BookId", "Title");
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
    }
}
