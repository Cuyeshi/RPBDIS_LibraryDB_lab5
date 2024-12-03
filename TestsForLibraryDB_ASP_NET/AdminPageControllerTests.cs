using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RPBDIS_LibraryDB_lab5.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsForLibraryDB_ASP_NET
{
    public class AdminPageControllerTests
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly AdminPageController _controller;

        public AdminPageControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);

            _controller = new AdminPageController(_userManagerMock.Object, _roleManagerMock.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithUsersAndRoles()
        {
            // Arrange
            var users = new List<IdentityUser>
        {
            new IdentityUser { Id = "1", UserName = "user1" },
            new IdentityUser { Id = "2", UserName = "user2" }
        };
            _userManagerMock.Setup(um => um.Users).Returns(users.AsQueryable());
            _userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(new List<string> { "admin" });

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<(IdentityUser user, IList<string> roles)>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Details_UserExists_ReturnsViewWithUser()
        {
            // Arrange
            var user = new IdentityUser { Id = "1", UserName = "user1" };
            _userManagerMock.Setup(um => um.FindByIdAsync("1")).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "admin" });

            // Act
            var result = await _controller.Details("1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<IdentityUser>(viewResult.Model);
            Assert.Equal("user1", model.UserName);
        }

        [Fact]
        public async Task Create_ValidUser_AddsUserAndRedirects()
        {
            // Arrange
            var user = new IdentityUser { UserName = "newuser" };
            _userManagerMock.Setup(um => um.CreateAsync(user, "password"))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddToRoleAsync(user, "role"))
                .ReturnsAsync(IdentityResult.Success); // Исправлено здесь

            // Act
            var result = await _controller.Create(user, "password", "role");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _userManagerMock.Verify(um => um.CreateAsync(user, "password"), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(user, "role"), Times.Once);
        }

        [Fact]
        public async Task Edit_UpdatesUserAndRoles()
        {
            // Arrange
            var user = new IdentityUser { Id = "1", UserName = "user1" };
            _userManagerMock.Setup(um => um.FindByIdAsync("1")).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "admin" });

            // Act
            var result = await _controller.Edit("1", user, new List<string> { "user" });

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
            _userManagerMock.Verify(um => um.AddToRolesAsync(user, It.Is<IEnumerable<string>>(r => r.Contains("user"))), Times.Once);
        }

        [Fact]
        public async Task DeleteConfirmed_DeletesUserAndRedirects()
        {
            // Arrange
            var user = new IdentityUser { Id = "1", UserName = "user1" };
            _userManagerMock.Setup(um => um.FindByIdAsync("1")).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.DeleteConfirmed("1");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _userManagerMock.Verify(um => um.DeleteAsync(user), Times.Once);
        }
    }
}
