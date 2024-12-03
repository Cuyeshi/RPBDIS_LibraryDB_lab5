using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class GenresControllerTests
    {
        /// <summary>
        /// Тест для Index (список жанров с фильтром и пагинацией)
        /// </summary>
        [Fact]
        public void Index_Returns_FilteredGenres_WithPagination()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new LibraryDbContext(options))
            {
                context.Genres.AddRange(
                    new Genre { GenreId = 1, Name = "Fiction" },
                    new Genre { GenreId = 2, Name = "Fantasy" },
                    new Genre { GenreId = 3, Name = "Non-Fiction" },
                    new Genre { GenreId = 4, Name = "Science Fiction" }
                );
                context.SaveChanges();
            }

            using (var context = new LibraryDbContext(options))
            {
                var controller = new GenresController(context);

                // Act
                var result = controller.Index("Fiction", 1) as ViewResult;

                // Assert
                Assert.NotNull(result);
                var genres = Assert.IsAssignableFrom<List<Genre>>(result.Model);
                Assert.Equal(3, genres.Count); // Ожидаем 2 жанра, содержащих "Fiction" в названии
                Assert.Contains(genres, g => g.Name == "Fiction");
                Assert.Contains(genres, g => g.Name == "Science Fiction");
            }
        }

        /// <summary>
        /// Тест для Create (GET)
        /// </summary>
        [Fact]
        public void Create_Get_Returns_ViewResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new LibraryDbContext(options))
            {
                var controller = new GenresController(context);

                // Act
                var result = controller.Create();

                // Assert
                Assert.IsType<ViewResult>(result);
            }
        }

        /// <summary>
        /// Тест для Create (POST)
        /// </summary>
        [Fact]
        public void Create_Post_ValidGenre_RedirectsToIndex()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new LibraryDbContext(options))
            {
                var controller = new GenresController(context);
                var genre = new Genre { Name = "New Genre" };

                // Act
                var result = controller.Create(genre) as RedirectToActionResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Index", result.ActionName);

                // Проверяем, что жанр добавлен
                Assert.Equal(1, context.Genres.Count(g => g.Name == "New Genre"));
            }
        }

        /// <summary>
        /// Тест для Edit (GET)
        /// </summary>
        [Fact]
        public void Edit_Get_ExistingGenre_Returns_ViewResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new LibraryDbContext(options))
            {
                context.Genres.Add(new Genre { GenreId = 1, Name = "Existing Genre" });
                context.SaveChanges();
            }

            using (var context = new LibraryDbContext(options))
            {
                var controller = new GenresController(context);

                // Act
                var result = controller.Edit(1) as ViewResult;

                // Assert
                Assert.NotNull(result);
                var genre = Assert.IsType<Genre>(result.Model);
                Assert.Equal("Existing Genre", genre.Name);
            }
        }

        /// <summary>
        /// Тест для Delete (GET)
        /// </summary>
        [Fact]
        public async Task Delete_Get_ExistingGenre_Returns_ViewResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new LibraryDbContext(options))
            {
                context.Genres.Add(new Genre { GenreId = 1, Name = "Genre to Delete" });
                context.SaveChanges();
            }

            using (var context = new LibraryDbContext(options))
            {
                var controller = new GenresController(context);

                // Act
                var result = await controller.Delete(1) as ViewResult;

                // Assert
                Assert.NotNull(result);
                var genre = Assert.IsType<Genre>(result.Model);
                Assert.Equal("Genre to Delete", genre.Name);
            }
        }

        /// <summary>
        /// Тест для Delete (POST)
        /// </summary>
        [Fact]
        public async Task Delete_Post_ExistingGenre_RedirectsToIndex()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new LibraryDbContext(options))
            {
                context.Genres.Add(new Genre { GenreId = 1, Name = "Genre to Delete" });
                context.SaveChanges();
            }

            using (var context = new LibraryDbContext(options))
            {
                var controller = new GenresController(context);

                // Act
                var result = await controller.DeleteConfirmed(1) as RedirectToActionResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Index", result.ActionName);

                // Проверяем, что жанр удален
                Assert.Empty(context.Genres);
            }
        }
    }
}
