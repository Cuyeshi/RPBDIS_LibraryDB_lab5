using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RPBDIS_LibraryDB_lab5.Controllers;
using RPBDIS_LibraryDB_lab5.Data;
using RPBDIS_LibraryDB_lab5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsForLibraryDB_ASP_NET
{
    public class LoanedBooksControllerTests
    {
        private LibraryDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new LibraryDbContext(options);
        }

        [Fact]
        public void Index_ReturnsViewResult_WithCorrectData()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetInMemoryDbContext(dbName);
            context.Books.Add(new Book { BookId = 1, Title = "Test Book", Author = "Test author" });
            context.LoanedBooks.Add(new LoanedBook { LoanId = 1, BookId = 1, LoanDate = new DateOnly(2024, 12, 12) });
            context.SaveChanges();

            var controller = new LoanedBooksController(context);

            // Act
            var result = controller.Index(null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<LoanedBook>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal("Test Book", model.First().Book.Title);
        }

        [Fact]
        public void Create_GET_ReturnsViewWithSelectList()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetInMemoryDbContext(dbName);
            context.Books.Add(new Book { BookId = 1, Title = "Test Book", Author = "Test author" });
            context.SaveChanges();

            var controller = new LoanedBooksController(context);

            // Act
            var result = controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.ViewData["Books"]);
        }

        [Fact]
        public void Create_POST_AddsLoanedBookAndRedirects()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetInMemoryDbContext(dbName);
            context.Books.Add(new Book { BookId = 1, Title = "Test Book", Author = "Test author" });
            context.SaveChanges();

            var controller = new LoanedBooksController(context);
            var loanedBook = new LoanedBook
            {
                LoanId = 1,
                BookId = 1,
                LoanDate = new DateOnly(2024, 12, 12)
            };

            // Act
            var result = controller.Create(loanedBook);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Single(context.LoanedBooks);
        }

        [Fact]
        public void Edit_GET_ReturnsViewWithCorrectData()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetInMemoryDbContext(dbName);
            context.LoanedBooks.Add(new LoanedBook { LoanId = 1, BookId = 1, LoanDate = new DateOnly(2024, 12, 12) });
            context.SaveChanges();

            var controller = new LoanedBooksController(context);

            // Act
            var result = controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LoanedBook>(viewResult.Model);
            Assert.Equal(1, model.LoanId);
        }

        [Fact]
        public void Delete_GET_ReturnsViewWithLoanedBook()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetInMemoryDbContext(dbName);
            context.LoanedBooks.Add(new LoanedBook { LoanId = 1, BookId = 1 });
            context.SaveChanges();

            var controller = new LoanedBooksController(context);

            // Act
            var result = controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LoanedBook>(viewResult.Model);
            Assert.Equal(1, model.LoanId);
        }

        [Fact]
        public async void Delete_POST_RemovesLoanedBookAndRedirects()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetInMemoryDbContext(dbName);
            context.LoanedBooks.Add(new LoanedBook { LoanId = 1, BookId = 1 });
            context.SaveChanges();

            var controller = new LoanedBooksController(context);

            // Act
            var result = await controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Empty(context.LoanedBooks);
        }
    }
}
