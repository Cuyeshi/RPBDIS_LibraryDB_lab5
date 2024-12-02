using Microsoft.AspNetCore.Mvc;
using RPBDIS_LibraryDB_lab5.Models;
using Microsoft.EntityFrameworkCore;

namespace RPBDIS_LibraryDB_lab5.Controllers.Additional_controllers
{
    public class FallReadersController : Controller
    {
        private readonly LibraryDbContext _context;

        public FallReadersController(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? readerName, string? returnDate, int page = 1)
        {
            int pageSize = 10;

            // Парсинг даты, если она указана
            DateOnly? parsedDate = null;
            if (!string.IsNullOrEmpty(returnDate) && DateOnly.TryParse(returnDate, out var date))
            {
                parsedDate = date;
            }

            // Базовый запрос с фильтрацией по дате возврата
            var query = _context.Readers
                .Include(r => r.LoanedBooks)
                .ThenInclude(lb => lb.Book)
                .Where(r => r.LoanedBooks.Any(lb => !lb.Returned &&
                                                     lb.ReturnDate < (parsedDate ?? DateOnly.FromDateTime(DateTime.Now))))
                .AsQueryable();

            if (!string.IsNullOrEmpty(readerName))
            {
                query = query.Where(r => r.FullName.Contains(readerName));
            }

            // Подсчет общего числа строк
            int totalItems = await query
                .SelectMany(r => r.LoanedBooks
                    .Where(lb => !lb.Returned && lb.ReturnDate < (parsedDate ?? DateOnly.FromDateTime(DateTime.Now)))
                    .Select(lb => new { Reader = r, LoanedBook = lb }))
                .CountAsync();

            // Пагинация и выборка данных
            var readersWithBooks = await query
                .SelectMany(r => r.LoanedBooks
                    .Where(lb => !lb.Returned && lb.ReturnDate < (parsedDate ?? DateOnly.FromDateTime(DateTime.Now)))
                    .Select(lb => new { Reader = r, LoanedBook = lb }))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Передача параметров в представление
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.ReaderName = readerName;
            ViewBag.ReturnDate = parsedDate;

            return View(readersWithBooks);
        }

        public IActionResult Back()
        {
            return RedirectToAction("Index", "Home");
        }

    }
}
