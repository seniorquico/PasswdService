using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PasswdService.Models;
using Xunit;

namespace PasswdService.Controllers
{
    public sealed class UsersControllerTests
    {
        public static User KyleUser = new User("kyle", 1000, 1000, "", "/home/kyle", "/bin/bash");
        public static User RootUser = new User("root", 0, 0, "", "/root", "/bin/bash");

        public static IEnumerable<object[]> FindUsers_ReturnsValue_WithMatchingUsers_Data =>
            new List<object[]>
            {
                new object[] { new UserQuery {}, new User[] { RootUser, KyleUser } },
                new object[] { new UserQuery { uid = 65535 }, new User[0] },
                new object[] { new UserQuery { uid = 1000 }, new User[] { KyleUser } },
                new object[] { new UserQuery { gid = 65535 }, new User[0] },
                new object[] { new UserQuery { gid = 0 }, new User[] { RootUser } },
                new object[] { new UserQuery { comment = "Hello, world!" }, new User[0] },
                new object[] { new UserQuery { comment = "" }, new User[] { RootUser, KyleUser } },
                new object[] { new UserQuery { name = "john" }, new User[0] },
                new object[] { new UserQuery { name = "root" }, new User[] { RootUser } },
                new object[] { new UserQuery { gid = 1, name = "root" }, new User[0] },
                new object[] { new UserQuery { gid = 0, home = "/root" }, new User[] { RootUser } },
                new object[] { new UserQuery { gid = 1000, shell = "/bin/bash" }, new User[] { KyleUser } },
            };

        [Theory]
        [MemberData(nameof(FindUsers_ReturnsValue_WithMatchingUsers_Data))]
        public void FindUsers_ReturnsValue_WithMatchingUsers(UserQuery queryString, IEnumerable<User> expectedUsers)
        {
            // Arrange
            var mockUserService = new Mock<IUserStore>(MockBehavior.Strict);
            mockUserService.Setup(o => o.GetUsers()).Returns(new User[] { RootUser, KyleUser });

            var controller = new UsersController(mockUserService.Object);

            // Act
            var result = controller.FindUsers(queryString);

            // Assert
            mockUserService.Verify(o => o.GetUsers(), Times.Once());

            Assert.NotNull(result);

            Assert.Null(result.Result);
            Assert.Equal(expectedUsers, result.Value);
        }

        [Fact]
        public void GetAllUsers_ReturnsValue_WithEmptyListOfUsers()
        {
            // Arrange
            var mockUserService = new Mock<IUserStore>(MockBehavior.Strict);
            mockUserService.Setup(o => o.GetUsers()).Returns(new User[0]);

            var controller = new UsersController(mockUserService.Object);

            // Act
            var result = controller.GetAllUsers();

            // Assert
            mockUserService.Verify(o => o.GetUsers(), Times.Once());

            Assert.NotNull(result);

            Assert.Null(result.Result);
            Assert.Empty(result.Value);
        }

        [Fact]
        public void GetAllUsers_ReturnsValue_WithNonEmptyListOfUsers()
        {
            // Arrange
            var mockUserService = new Mock<IUserStore>(MockBehavior.Strict);
            mockUserService.Setup(o => o.GetUsers()).Returns(new User[] { KyleUser });

            var controller = new UsersController(mockUserService.Object);

            // Act
            var result = controller.GetAllUsers();

            // Assert
            mockUserService.Verify(o => o.GetUsers(), Times.Once());

            Assert.NotNull(result);

            Assert.Null(result.Result);
            Assert.Equal(new User[] { KyleUser }, result.Value);
        }

        [Fact]
        public void GetUserById_ReturnsResult_NotFound()
        {
            // Arrange
            var mockUserService = new Mock<IUserStore>(MockBehavior.Strict);
            mockUserService.Setup(o => o.GetUser(1000)).Returns<User>(null);

            var controller = new UsersController(mockUserService.Object);

            // Act
            var result = controller.GetUserById(1000);

            // Assert
            mockUserService.Verify(o => o.GetUser(1000), Times.Once());

            Assert.NotNull(result);

            Assert.IsType<NotFoundResult>(result.Result);
            Assert.Null(result.Value);
        }

        [Fact]
        public void GetUserById_ReturnsValue_WithUser()
        {
            // Arrange
            var mockUserService = new Mock<IUserStore>(MockBehavior.Strict);
            mockUserService.Setup(o => o.GetUser(1000)).Returns(KyleUser);

            var controller = new UsersController(mockUserService.Object);

            // Act
            var result = controller.GetUserById(1000);

            // Assert
            mockUserService.Verify(o => o.GetUser(1000), Times.Once());

            Assert.NotNull(result);

            Assert.Null(result.Result);
            Assert.Equal(KyleUser, result.Value);
        }
    }
}
