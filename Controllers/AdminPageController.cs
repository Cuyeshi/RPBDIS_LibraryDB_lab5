using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Models;

namespace RPBDIS_LibraryDB_lab5.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminPageController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminPageController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: AdminPage
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userWithRoles = new List<(IdentityUser user, IList<string> roles)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userWithRoles.Add((user, roles));
            }

            return View(userWithRoles);
        }

        // GET: AdminPage/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;

            return View(user);
        }

        // GET: AdminPage/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View();
        }

        // POST: AdminPage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityUser user, string password, string role)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    user.EmailConfirmed = true; // Подтверждаем Email
                    await _userManager.UpdateAsync(user); // Обновляем пользователя
                    if (!string.IsNullOrEmpty(role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View(user);
        }

        // GET: AdminPage/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.AllRoles = _roleManager.Roles.ToList();
            ViewBag.UserRoles = roles;

            return View(user);
        }

        // POST: AdminPage/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, IdentityUser user, List<string> roles)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;

                var result = await _userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    var userRoles = await _userManager.GetRolesAsync(existingUser);

                    var addedRoles = roles.Except(userRoles);
                    var removedRoles = userRoles.Except(roles);

                    await _userManager.AddToRolesAsync(existingUser, addedRoles);
                    await _userManager.RemoveFromRolesAsync(existingUser, removedRoles);

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            ViewBag.AllRoles = _roleManager.Roles.ToList();
            ViewBag.UserRoles = roles;
            return View(user);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Back()
        {
            return RedirectToAction("Index", "AdminPage");
        }
        
        public IActionResult BackToHome()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
