using System.Collections.Generic;
using PasswdService.Models;
using Xunit;

namespace PasswdService.Utilities
{
    public sealed class GroupIdSorterTests
    {
        public static Group RootGroup = new Group("root", 0, new string[] { "root" });
        public static Group UserGroup = new Group("kyle", 1000, new string[] { "kyle" });

        public static IEnumerable<object[]> GroupIdSorter_Compare_Data =>
            new List<object[]>
            {
                new object[] { null, null, 0 },
                new object[] { null, RootGroup, -1 },
                new object[] { RootGroup, null, 1 },
                new object[] { RootGroup, RootGroup, 0 },
                new object[] { UserGroup, UserGroup, 0 },
                new object[] { RootGroup, UserGroup, -1 },
                new object[] { UserGroup, RootGroup, 1 },
            };

        [Theory]
        [MemberData(nameof(GroupIdSorter_Compare_Data))]
        public void GroupIdSorter_Compare(Group x, Group y, int expectedResult)
        {
            // Act
            var result = GroupIdSorter.Instance.Compare(x, y);

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
