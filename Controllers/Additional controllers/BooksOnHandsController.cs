using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Data;

namespace RPBDIS_LibraryDB_lab5.Controllers.Additional_controllers
{
    public class BooksOnHandsController : Controller
    {
        private readonly LibraryDbContext _context;
        private const int PageSize = 10; // Количество записей на одной странице

        public BooksOnHandsController(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? author, string? genre, int page = 1)
        {
            var query = _context.LoanedBooks.Include(lb => lb.Book).Include(lb => lb.Book!.Genre).AsQueryable();

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(lb => lb.Book!.Author.Contains(author));
            }

            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(lb => lb.Book!.Genre!.Name.Contains(genre));
            }

            var totalItems = await query.CountAsync();
            var loanedBooks = await query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            ViewBag.Author = author;
            ViewBag.Genre = genre;

            return View(loanedBooks);
        }

        public async Task<IActionResult> Filter(string? author, string? genre)
        {
            var query = _context.LoanedBooks.Include(lb => lb.Book).Include(lb => lb.Book!.Genre).AsQueryable();

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(lb => lb.Book!.Author.Contains(author));
            }

            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(lb => lb.Book!.Genre!.Name.Contains(genre));
            }

            var loanedBooks = await query.ToListAsync();
            return View("Index", loanedBooks);
        }

        public IActionResult Back()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
