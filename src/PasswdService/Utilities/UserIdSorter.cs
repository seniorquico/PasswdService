using System.Collections.Generic;
using System.Diagnostics;
using PasswdService.Models;

namespace PasswdService.Utilities
{
    /// <summary>
    ///     A custom comparer of <see cref="User"/> instances that may be used to sort a list by unique user identifier
    ///     (or "uid") in ascending order.
    /// </summary>
    [DebuggerStepThrough]
    public sealed class UserIdSorter : IComparer<User>
    {
        /// <summary>Prevents creating new instances.</summary>
        private UserIdSorter()
        {
        }

        /// <summary>Gets the singleton <see cref="UserIdSorter"/> instance.</summary>
        public static UserIdSorter Instance { get; } = new UserIdSorter();

        /// <summary>Compares two <see cref="User"/> instances</summary>
        /// <param name="x">The first <see cref="User"/> instance.</param>
        /// <param name="y">The second <see cref="User"/> instance.</param>
        /// <returns>
        ///     A signed integer that indicates the relative values of x and y. A value less than zero indicates
        ///     <paramref name="x"/> is less than <paramref name="y"/>. A value equal to zero indicates
        ///     <paramref name="x"/> equals <paramref name="y"/>. A value greater than zero indicates
        ///     <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public int Compare(User x, User y)
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
            else if (x.UserId < y.UserId)
            {
                return -1;
            }
            else if (x.UserId > y.UserId)
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
