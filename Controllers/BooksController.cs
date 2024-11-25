using Microsoft.AspNetCore.Mvc;
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

        [ResponseCache(Duration = 282, Location = ResponseCacheLocation.Client)]
        public IActionResult Index()
        {
            var books = _context.Books.Include(b => b.Genre).ToList();
            return View(books);
        }
    }

}
