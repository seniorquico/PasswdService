using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PasswdService.Models
{
    /// <summary>Represents a parsed, immutable <c>/etc/passwd</c> file.</summary>
    public sealed class PasswordFile
    {
        /// <summary>Initializes a new instance of the <see cref="PasswordFile"/> class.</summary>
        /// <param name="users">The list of users pre-sorted by unique user identifier (or "uid").</param>
        /// <param name="usersById">The dictionary that maps unique user identifiers (or "uid") to users.</param>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="users"/> or <paramref name="usersById"/> are <c>null</c>.
        /// </exception>
        public PasswordFile(List<User> users, Dictionary<uint, User> usersById)
        {
            if (users == null)
            {
                throw new ArgumentNullException(nameof(users));
            }

            if (usersById == null)
            {
                throw new ArgumentNullException(nameof(usersById));
            }

            this.Users = users.AsReadOnly();
            this.UsersById = new ReadOnlyDictionary<uint, User>(usersById);
        }

        /// <summary>Gets a read-only list of users pre-sorted by unique user identifier (or "uid").</summary>
        public IReadOnlyList<User> Users { get; }

        /// <summary>Gets a read-only dictionary that maps unique user identifiers (or "uid") to users.</summary>
        public IReadOnlyDictionary<uint, User> UsersById { get; }
    }
}
