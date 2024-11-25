using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            var genres = _context.Genres.ToList();
            return View(genres);
        }
    }

}
