using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PasswdService.Models;
using Xunit;

namespace PasswdService.Controllers
{
    public sealed class GroupsControllerTests
    {
        public static Group DaemonGroup = new Group("daemon", 1, new string[0]);
        public static Group FloppyGroup = new Group("floppy", 25, new string[] { "kyle" });
        public static Group RootGroup = new Group("root", 0, new string[] { "root" });
        public static Group UserGroup = new Group("kyle", 1000, new string[] { "kyle" });

        public static IEnumerable<object[]> FindGroups_ReturnsValue_WithMatchingGroups_Data =>
            new List<object[]>
            {
                new object[] { new GroupQuery {}, new Group[] { RootGroup, DaemonGroup, FloppyGroup, UserGroup } },
                new object[] { new GroupQuery { gid = 65535 }, new Group[0] },
                new object[] { new GroupQuery { gid = 1 }, new Group[] { DaemonGroup } },
                new object[] { new GroupQuery { members = new List<string> { "john" } }, new Group[0] },
                new object[] { new GroupQuery { members = new List<string> { "kyle" } }, new Group[] { FloppyGroup, UserGroup } },
                new object[] { new GroupQuery { name = "john" }, new Group[0] },
                new object[] { new GroupQuery { name = "root" }, new Group[] { RootGroup } },
                new object[] { new GroupQuery { gid = 1, name = "root" }, new Group[0] },
                new object[] { new GroupQuery { gid = 1000, members = new List<string> { "kyle" } }, new Group[] { UserGroup } },
            };

        [Theory]
        [MemberData(nameof(FindGroups_ReturnsValue_WithMatchingGroups_Data))]
        public void FindGroups_ReturnsValue_WithMatchingGroups(GroupQuery queryString, IEnumerable<Group> expectedGroups)
        {
            // Arrange
            var mockGroupService = new Mock<IGroupStore>(MockBehavior.Strict);
            mockGroupService.Setup(o => o.GetGroups()).Returns(new Group[] { RootGroup, DaemonGroup, FloppyGroup, UserGroup });

            var controller = new GroupsController(mockGroupService.Object);

            // Act
            var result = controller.FindGroups(queryString);

            // Assert
            mockGroupService.Verify(o => o.GetGroups(), Times.Once());

            Assert.NotNull(result);

            Assert.Null(result.Result);
            Assert.Equal(expectedGroups, result.Value);
        }

        [Fact]
        public void GetAllGroups_ReturnsValue_WithEmptyListOfGroups()
        {
            // Arrange
            var mockGroupService = new Mock<IGroupStore>(MockBehavior.Strict);
            mockGroupService.Setup(o => o.GetGroups()).Returns(new Group[0]);

            var controller = new GroupsController(mockGroupService.Object);

            // Act
            var result = controller.GetAllGroups();

            // Assert
            mockGroupService.Verify(o => o.GetGroups(), Times.Once());

            Assert.NotNull(result);

            Assert.Null(result.Result);
            Assert.Empty(result.Value);
        }

        [Fact]
        public void GetAllGroups_ReturnsValue_WithNonEmptyListOfGroups()
        {
            // Arrange
            var mockGroupService = new Mock<IGroupStore>(MockBehavior.Strict);
            mockGroupService.Setup(o => o.GetGroups()).Returns(new Group[] { UserGroup });

            var controller = new GroupsController(mockGroupService.Object);

            // Act
            var result = controller.GetAllGroups();

            // Assert
            mockGroupService.Verify(o => o.GetGroups(), Times.Once());

            Assert.NotNull(result);

            Assert.Null(result.Result);
            Assert.Equal(new Group[] { UserGroup }, result.Value);
        }

        [Fact]
        public void GetGroupById_ReturnsResult_NotFound()
        {
            // Arrange
            var mockGroupService = new Mock<IGroupStore>(MockBehavior.Strict);
            mockGroupService.Setup(o => o.GetGroup(1000)).Returns<Group>(null);

            var controller = new GroupsController(mockGroupService.Object);

            // Act
            var result = controller.GetGroupById(1000);

            // Assert
            mockGroupService.Verify(o => o.GetGroup(1000), Times.Once());

            Assert.NotNull(result);

            Assert.IsType<NotFoundResult>(result.Result);
            Assert.Null(result.Value);
        }

        [Fact]
        public void GetGroupById_ReturnsValue_WithGroup()
        {
            // Arrange
            var mockGroupService = new Mock<IGroupStore>(MockBehavior.Strict);
            mockGroupService.Setup(o => o.GetGroup(1000)).Returns(UserGroup);

            var controller = new GroupsController(mockGroupService.Object);

            // Act
            var result = controller.GetGroupById(1000);

            // Assert
            mockGroupService.Verify(o => o.GetGroup(1000), Times.Once());

            Assert.NotNull(result);

            Assert.Null(result.Result);
            Assert.Equal(UserGroup, result.Value);
        }

        [Fact]
        public void GetUserGroups_ReturnsResult_NotFound()
        {
            // Arrange
            var mockGroupService = new Mock<IGroupStore>(MockBehavior.Strict);
            mockGroupService.Setup(o => o.GetGroupsContainingUser(1000)).Returns<Group>(null);

            var controller = new GroupsController(mockGroupService.Object);

            // Act
            var result = controller.GetUserGroups(1000);

            // Assert
            mockGroupService.Verify(o => o.GetGroupsContainingUser(1000), Times.Once());

            Assert.NotNull(result);

            Assert.IsType<NotFoundResult>(result.Result);
            Assert.Null(result.Value);
        }

        [Fact]
        public void GetUserGroups_ReturnsValue_WithNonEmptyListOfGroups()
        {
            // Arrange
            var mockGroupService = new Mock<IGroupStore>(MockBehavior.Strict);
            mockGroupService.Setup(o => o.GetGroupsContainingUser(1000)).Returns(new Group[] { FloppyGroup, UserGroup });

            var controller = new GroupsController(mockGroupService.Object);

            // Act
            var result = controller.GetUserGroups(1000);

            // Assert
            mockGroupService.Verify(o => o.GetGroupsContainingUser(1000), Times.Once());

            Assert.NotNull(result);

            Assert.Null(result.Result);
            Assert.Equal(new Group[] { FloppyGroup, UserGroup }, result.Value);
        }
    }
}
