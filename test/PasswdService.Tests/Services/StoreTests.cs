using System.Collections.Generic;
using PasswdService.Models;
using Xunit;

namespace PasswdService.Services
{
    public sealed class StoreTests
    {
        public static Group RootGroup = new Group("root", 0, new string[0]);
        public static Group DaemonGroup = new Group("daemon", 1, new string[0]);
        public static Group FloppyGroup = new Group("floppy", 25, new string[] { "kyle" });
        public static Group UserGroup = new Group("kyle", 1000, new string[0]);
        public static GroupFile GroupFile = new GroupFile(
            new List<Group> { RootGroup, DaemonGroup, FloppyGroup, UserGroup },
            new Dictionary<uint, Group>
            {
                { 0, RootGroup },
                { 1, DaemonGroup },
                { 25, FloppyGroup },
                { 1000, UserGroup },
            },
            new Dictionary<string, List<Group>>
            {
                { "kyle", new List<Group> { FloppyGroup } },
            });

        public static User RootUser = new User("root", 0, 0, "", "/root", "/bin/bash");
        public static User KyleUser = new User("kyle", 1000, 1000, "", "/home/kyle", "/bin/bash");
        public static PasswordFile PasswordFile = new PasswordFile(
            new List<User> { RootUser, KyleUser },
            new Dictionary<uint, User>
            {
                { 0, RootUser },
                { 1000, KyleUser },
            });

        [Fact]
        public void GetGroup_ReturnsNull_WithUndefinedGroup()
        {
            // Arrange
            var store = new Store();
            store.SetGroupFile(GroupFile);
            store.SetPasswordFile(PasswordFile);

            // Act
            var group = store.GetGroup(65535);

            // Assert
            Assert.Null(group);
        }

        [Fact]
        public void GetGroup_ReturnsValue_WithDefinedGroup()
        {
            // Arrange
            var store = new Store();
            store.SetGroupFile(GroupFile);
            store.SetPasswordFile(PasswordFile);

            // Act
            var group = store.GetGroup(1);

            // Assert
            Assert.Equal(DaemonGroup, group);
        }

        [Fact]
        public void GetGroup_ThrowsStoreException_WithNullGroupFile()
        {
            // Arrange
            var store = new Store();
            store.SetPasswordFile(PasswordFile);

            // Act+Assert
            Assert.Throws<StoreException>(() => store.GetGroup(1000));
        }

        [Fact]
        public void GetGroups_ReturnsValue_WithDefinedGroups()
        {
            // Arrange
            var store = new Store();
            store.SetGroupFile(GroupFile);
            store.SetPasswordFile(PasswordFile);

            // Act
            var groups = store.GetGroups();

            // Assert
            Assert.Equal(new Group[] { RootGroup, DaemonGroup, FloppyGroup, UserGroup }, groups);
        }

        [Fact]
        public void GetGroups_ThrowsStoreException_WithNullGroupFile()
        {
            // Arrange
            var store = new Store();
            store.SetPasswordFile(PasswordFile);

            // Act+Assert
            Assert.Throws<StoreException>(() => store.GetGroups());
        }

        [Fact]
        public void GetGroupsContainingUser_ReturnsNull_WithUndefinedUser()
        {
            // Arrange
            var store = new Store();
            store.SetGroupFile(GroupFile);
            store.SetPasswordFile(PasswordFile);

            // Act
            var groups = store.GetGroupsContainingUser(65535);

            // Assert
            Assert.Null(groups);
        }

        [Fact]
        public void GetGroupsContainingUser_ReturnsValue_WithDefinedUser()
        {
            // Arrange
            var store = new Store();
            store.SetGroupFile(GroupFile);
            store.SetPasswordFile(PasswordFile);

            // Act
            var groups = store.GetGroupsContainingUser(1000);

            // Assert
            Assert.Equal(new Group[] { FloppyGroup, UserGroup }, groups);
        }

        [Fact]
        public void GetGroupsContainingUser_ThrowsStoreException_WithNullGroupFile()
        {
            // Arrange
            var store = new Store();
            store.SetPasswordFile(PasswordFile);

            // Act+Assert
            Assert.Throws<StoreException>(() => store.GetGroupsContainingUser(1000));
        }

        [Fact]
        public void GetGroupsContainingUser_ThrowsStoreException_WithNullPasswordFile()
        {
            // Arrange
            var store = new Store();
            store.SetGroupFile(GroupFile);

            // Act+Assert
            Assert.Throws<StoreException>(() => store.GetGroupsContainingUser(1000));
        }

        [Fact]
        public void GetUser_ReturnsNull_WithUndefinedUser()
        {
            // Arrange
            var store = new Store();
            store.SetGroupFile(GroupFile);
            store.SetPasswordFile(PasswordFile);

            // Act
            var user = store.GetUser(65535);

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public void GetUser_ReturnsValue_WithDefinedUser()
        {
            // Arrange
            var store = new Store();
            store.SetGroupFile(GroupFile);
            store.SetPasswordFile(PasswordFile);

            // Act
            var user = store.GetUser(1000);

            // Assert
            Assert.Equal(KyleUser, user);
        }

        [Fact]
        public void GetUser_ThrowsStoreException_WithNullPasswordFile()
        {
            // Arrange
            var store = new Store();
            store.SetGroupFile(GroupFile);

            // Act+Assert
            Assert.Throws<StoreException>(() => store.GetUser(1000));
        }

        [Fact]
        public void GetUsers_ReturnsValue_WithDefinedUsers()
        {
            // Arrange
            var store = new Store();
            store.SetGroupFile(GroupFile);
            store.SetPasswordFile(PasswordFile);

            // Act
            var users = store.GetUsers();

            // Assert
            Assert.Equal(new User[] { RootUser, KyleUser }, users);
        }

        [Fact]
        public void GetUsers_ThrowsStoreException_WithNullPasswordFile()
        {
            // Arrange
            var store = new Store();
            store.SetGroupFile(GroupFile);

            // Act+Assert
            Assert.Throws<StoreException>(() => store.GetUsers());
        }
    }
}
