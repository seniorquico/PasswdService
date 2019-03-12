using System.Collections.Generic;
using System.Diagnostics;
using PasswdService.Models;

namespace PasswdService.Utilities
{
    /// <summary>
    ///     A custom comparer of <see cref="Group"/> instances that may be used to sort a list by unique group identifier
    ///     (or "gid") in ascending order.
    /// </summary>
    [DebuggerStepThrough]
    public sealed class GroupIdSorter : IComparer<Group>
    {
        /// <summary>Prevents creating new instances.</summary>
        private GroupIdSorter()
        {
        }

        /// <summary>Gets the singleton <see cref="GroupIdSorter"/> instance.</summary>
        public static GroupIdSorter Instance { get; } = new GroupIdSorter();

        /// <summary>Compares two <see cref="Group"/> instances</summary>
        /// <param name="x">The first <see cref="Group"/> instance.</param>
        /// <param name="y">The second <see cref="Group"/> instance.</param>
        /// <returns>
        ///     A signed integer that indicates the relative values of x and y. A value less than zero indicates
        ///     <paramref name="x"/> is less than <paramref name="y"/>. A value equal to zero indicates
        ///     <paramref name="x"/> equals <paramref name="y"/>. A value greater than zero indicates
        ///     <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public int Compare(Group x, Group y)
        {
            if (x is null)
            {
                if (y is null)
                {
                    return 0;
                }

                return -1;
            }
            else if (y is null)
            {
                return 1;
            }
            else if (x.GroupId < y.GroupId)
            {
                return -1;
            }
            else if (x.GroupId > y.GroupId)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
