using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace RPBDIS_LibraryDB_lab5.Controllers
{
    public class AdminController : Controller
    {
        UserManager<IdentityUser> _userManager;
        public AdminController(UserManager<IdentityUser> manager)
        {
            _userManager = manager;
        }
        public IActionResult Index()
        {
            return View(_userManager.Users.ToList());
        }
    }
}
