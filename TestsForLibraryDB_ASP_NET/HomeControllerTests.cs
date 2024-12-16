using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RPBDIS_LibraryDB_lab5.Controllers;
using RPBDIS_LibraryDB_lab5.Models;
using System.Diagnostics;
using Xunit;

namespace TestsForLibraryDB_ASP_NET
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockLogger.Object);
        }

        [Fact]
        public void Index_Returns_ViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); // Default view name
        }

        [Fact]
        public void Privacy_Returns_ViewResult()
        {
            // Act
            var result = _controller.Additionally();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); // Default view name
        }

        [Fact]
        public void Error_Returns_ViewResult_With_ErrorViewModel()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(loggerMock.Object);

            // Создаём мок для HttpContext
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.TraceIdentifier).Returns("test-trace-id");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            // Устанавливаем Activity.Current в null для теста
            Activity.Current = null;

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal("test-trace-id", model.RequestId);
        }



    }
}