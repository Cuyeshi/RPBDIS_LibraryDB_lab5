using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Data;
using RPBDIS_LibraryDB_lab5.Models;

namespace RPBDIS_LibraryDB_lab5.Controllers
{
    [Authorize()]
    public class EmployeesController : Controller
    {
        private readonly LibraryDbContext _context;

        public EmployeesController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public IActionResult Index(string nameFilter, string positionFilter, DateOnly? hireDateFilter, int page = 1)
        {
            int pageSize = 10; // Количество записей на странице

            var query = _context.Employees.AsQueryable();

            // Применяем фильтры
            if (!string.IsNullOrEmpty(nameFilter))
                query = query.Where(e => e.FullName.Contains(nameFilter));
            if (!string.IsNullOrEmpty(positionFilter))
                query = query.Where(e => e.Position.Contains(positionFilter));
            if (hireDateFilter.HasValue)
                query = query.Where(e => e.HireDate == hireDateFilter);

            // Получаем общее количество страниц
            int totalItems = query.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentPage = page;

            // Сохраняем состояние фильтров
            ViewBag.NameFilter = nameFilter;
            ViewBag.PositionFilter = positionFilter;
            ViewBag.HireDateFilter = hireDateFilter;

            // Пагинация
            var employees = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(employees);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = _context.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Employees.Any(e => e.EmployeeId == id))
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
            return View(employee);
        }

        // GET: Delete
        public IActionResult Delete(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Back()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
