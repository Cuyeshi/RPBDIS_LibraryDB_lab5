using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Data;

namespace RPBDIS_LibraryDB_lab5.Controllers.Additional_controllers
{
    public class PersonnelDepartmentController : Controller
    {
        private readonly LibraryDbContext _context;

        public PersonnelDepartmentController(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? fullName, string? position, int page = 1, int pageSize = 10)
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(fullName))
            {
                query = query.Where(e => e.FullName.Contains(fullName));
            }

            if (!string.IsNullOrEmpty(position))
            {
                query = query.Where(e => e.Position.Contains(position));
            }

            var totalItems = await query.CountAsync();
            var employees = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.FullName = fullName;
            ViewBag.Position = position;

            return View(employees);
        }


        public async Task<IActionResult> Search(string? fullName, string? position)
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(fullName))
            {
                query = query.Where(e => e.FullName.Contains(fullName));
            }

            if (!string.IsNullOrEmpty(position))
            {
                query = query.Where(e => e.Position.Contains(position));
            }

            var employees = await query.ToListAsync();
            return View("Index", employees);
        }

        public IActionResult Back()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
