using System.Collections.Generic;
using PasswdService.Models;
using Xunit;

namespace PasswdService.Utilities
{
    public sealed class UserIdSorterTests
    {
        public static User KyleUser = new User("kyle", 1000, 1000, "", "/home/kyle", "/bin/bash");
        public static User RootUser = new User("root", 0, 0, "", "/root", "/bin/bash");

        public static IEnumerable<object[]> UserIdSorter_Compare_Data =>
            new List<object[]>
            {
                new object[] { null, null, 0 },
                new object[] { null, RootUser, -1 },
                new object[] { RootUser, null, 1 },
                new object[] { RootUser, RootUser, 0 },
                new object[] { KyleUser, KyleUser, 0 },
                new object[] { RootUser, KyleUser, -1 },
                new object[] { KyleUser, RootUser, 1 },
            };

        [Theory]
        [MemberData(nameof(UserIdSorter_Compare_Data))]
        public void UserIdSorter_Compare(User x, User y, int expectedResult)
        {
            // Act
            var result = UserIdSorter.Instance.Compare(x, y);

            if (expectedResult < 0)
            {
                Assert.True(result < 0);
            }
            else if (expectedResult > 0)
            {
                Assert.True(result > 0);
            }
            else
            {
                Assert.Equal(0, result);
            }
        }
    }
}
