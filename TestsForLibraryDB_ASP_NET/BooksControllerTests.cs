using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Controllers;
using RPBDIS_LibraryDB_lab5.Data;
using RPBDIS_LibraryDB_lab5.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace TestsForLibraryDB_ASP_NET
{

    public class BooksControllerTests
    {
        /// <summary>
        /// Тесты для Index
        /// </summary>
        [Fact]
        public void Index_Returns_ViewResult_With_FilteredBooks()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальное имя базы данных
                .Options;

            using (var context = new LibraryDbContext(options))
            {
                context.Genres.Add(new Genre { GenreId = 1, Name = "Fiction" });
                context.Publishers.Add(new Publisher { PublisherId = 1, Name = "Test Publisher", Address = "Test addres", City = "Test city" });
                context.Books.AddRange(
                    new Book { BookId = 1, Title = "Test Book 1", Author = "Author 1",  GenreId = 1, PublisherId = 1 },
                    new Book { BookId = 2, Title = "Another Book", Author = "Author 2", GenreId = 1, PublisherId = 1 }
                );
                context.SaveChanges();
            }

            using (var context = new LibraryDbContext(options))
            {
                var controller = new BooksController(context);

                // Act
                var result = controller.Index("Test", null, null, null) as ViewResult;

                // Assert
                Assert.NotNull(result);
                var books = Assert.IsAssignableFrom<List<Book>>(result.Model);
                Assert.Single(books); // Ожидаем одну книгу, которая соответствует фильтру
                Assert.Equal("Test Book 1", books[0].Title);
            }
        }


        /// <summary>
        /// Тесты для Create (GET)
        /// </summary>
        [Fact]
        public void Create_Returns_ViewResult_With_SelectLists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibraryDb")
                .Options;

            using (var context = new LibraryDbContext(options))
            {
                context.Genres.Add(new Genre { GenreId = 1, Name = "Fiction" });
                context.Publishers.Add(new Publisher { PublisherId = 1, Name = "Test Publisher", Address = "Test addres", City = "Test city" });
                context.SaveChanges();
            }

            using (var context = new LibraryDbContext(options))
            {
                var controller = new BooksController(context);

                // Act
                var result = controller.Create() as ViewResult;

                // Assert
                Assert.NotNull(result);
                Assert.NotNull(result.ViewData["Genres"]);
                Assert.NotNull(result.ViewData["Publishers"]);
            }
        }

        /// <summary>
        /// Тесты для Create (POST)
        /// </summary>
        [Fact]
        public void Create_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальное имя базы данных
                .Options;

            using (var context = new LibraryDbContext(options))
            {
                context.Genres.Add(new Genre { GenreId = 1, Name = "Fiction" });
                context.Publishers.Add(new Publisher { PublisherId = 1, Name = "Test Publisher", Address = "Test addres", City = "Test city" });
                context.SaveChanges();
            }

            using (var context = new LibraryDbContext(options))
            {
                var controller = new BooksController(context);
                var newBook = new Book
                {
                    Title = "New Book",
                    Author = "New Author",
                    GenreId = 1,
                    PublisherId = 1
                };

                // Act
                var result = controller.Create(newBook) as RedirectToActionResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Index", result.ActionName);

                // Проверяем, что книга добавлена
                Assert.Equal(1, context.Books.Count(b => b.Title == "New Book"));
            }
        }


        /// <summary>
        /// Тесты для Edit (GET)
        /// </summary>
        [Fact]
        public void Edit_Returns_ViewResult_With_Book()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibraryDb")
                .Options;

            using (var context = new LibraryDbContext(options))
            {
                context.Books.Add(new Book { BookId = 1, Title = "Test Book", Author = "Test Author", GenreId = 1, PublisherId = 1 });
                context.SaveChanges();
            }

            using (var context = new LibraryDbContext(options))
            {
                var controller = new BooksController(context);

                // Act
                var result = controller.Edit(1) as ViewResult;

                // Assert
                Assert.NotNull(result);
                var book = Assert.IsType<Book>(result.Model);
                Assert.Equal("Test Book", book.Title);
            }
        }

        /// <summary>
        /// Тесты для Delete (POST)
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteConfirmed_Removes_Book_And_RedirectsToIndex()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibraryDb")
                .Options;

            using (var context = new LibraryDbContext(options))
            {
                context.Books.Add(new Book { BookId = 1, Title = "Test Book", Author = "Test author" });
                context.SaveChanges();
            }

            using (var context = new LibraryDbContext(options))
            {
                var controller = new BooksController(context);

                // Act
                var result = await controller.DeleteConfirmed(1) as RedirectToActionResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Index", result.ActionName);
                Assert.Empty(context.Books); // Убедимся, что книга удалена
            }
        }

    }

}