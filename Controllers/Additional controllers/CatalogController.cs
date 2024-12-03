using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Data;

namespace RPBDIS_LibraryDB_lab5.Controllers.Additional_controllers
{
    public class CatalogController : Controller
    {
        private readonly LibraryDbContext _context;
        private const int PageSize = 10; // Количество записей на страницу

        public CatalogController(LibraryDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page, int? genreId, int? publisherId, decimal? priceMin, decimal? priceMax)
        {
            // Загружаем списки жанров и издательств
            ViewBag.Genres = _context.Genres
                .Select(g => new SelectListItem { Value = g.GenreId.ToString(), Text = g.Name })
                .ToList();

            ViewBag.Publishers = _context.Publishers
                .Select(p => new SelectListItem { Value = p.PublisherId.ToString(), Text = p.Name })
                .ToList();

            // Фильтрация
            var books = _context.Books.Include(b => b.Genre).Include(b => b.Publisher).AsQueryable();

            if (genreId.HasValue)
            {
                books = books.Where(b => b.GenreId == genreId.Value);
                ViewBag.SelectedGenreId = genreId;
            }

            if (publisherId.HasValue)
            {
                books = books.Where(b => b.PublisherId == publisherId.Value);
                ViewBag.SelectedPublisherId = publisherId;
            }

            if (priceMin.HasValue)
            {
                books = books.Where(b => b.Price >= priceMin.Value);
                ViewBag.PriceMin = priceMin;
            }

            if (priceMax.HasValue)
            {
                books = books.Where(b => b.Price <= priceMax.Value);
                ViewBag.PriceMax = priceMax;
            }

            // Пагинация
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var paginatedBooks = books.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(books.Count() / (double)pageSize);

            return View(paginatedBooks);
        }



        public async Task<IActionResult> Filter(string? genre, string? publisher, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Books.Include(b => b.Genre).Include(b => b.Publisher).AsQueryable();

            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(b => b.Genre!.Name.Contains(genre));
            }

            if (!string.IsNullOrEmpty(publisher))
            {
                query = query.Where(b => b.Publisher!.Name.Contains(publisher));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(b => b.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(b => b.Price <= maxPrice.Value);
            }

            var books = await query.ToListAsync();
            return View("Index", books);
        }
    }
}
