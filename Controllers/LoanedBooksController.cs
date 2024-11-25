﻿using Microsoft.AspNetCore.Mvc;
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

        [ResponseCache(Duration = 282, Location = ResponseCacheLocation.Client)]
        public IActionResult Index()
        {
            var loanedBooks = _context.LoanedBooks.Include(lb => lb.Book).ThenInclude(b => b.Genre).ToList();
            return View(loanedBooks);
        }
    }

}
