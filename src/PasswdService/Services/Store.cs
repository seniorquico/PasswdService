using System;
using System.Collections.Generic;
using System.Linq;
using PasswdService.Models;
using PasswdService.Utilities;

namespace PasswdService.Services
{
    /// <summary>
    ///     A store of groups and users parsed from the <c>/etc/group</c> and <c>/etc/passwd</c> files, respectively.
    /// </summary>
    internal sealed class Store : IStore
    {
        /// <summary>The parsed <c>/etc/group</c> file or <c>null</c> if it does not exist or is malformed.</summary>
        private GroupFile groupFile;

        /// <summary>
        ///     The parsed <c>/etc/passwd</c> file or <c>null</c> if it does not exist or is malformed.
        /// </summary>
        private PasswordFile passwordFile;

        /// <summary>Gets the group defined with the specified group identifier.</summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>The group or <c>null</c> if a matching group could not be found.</returns>
        public Group GetGroup(uint groupId)
        {
            var groupFile = this.groupFile;
            if (groupFile == null)
            {
                throw new StoreException();
            }

            return groupFile.GroupsById.TryGetValue(groupId, out var group)
                ? group
                : null;
        }

        /// <summary>Gets an enumerable of all defined groups.</summary>
        /// <returns>The enumerable of all defined groups.</returns>
        public IEnumerable<Group> GetGroups()
        {
            var groupFile = this.groupFile;
            if (groupFile == null)
            {
                throw new StoreException();
            }

            return groupFile.Groups;
        }

        /// <summary>
        ///     Gets an enumerable of all defined groups containing the user with the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///     The enumerable of all defined groups containing the user or <c>null</c> if a matching user could not be
        ///     found.
        /// </returns>
        public IEnumerable<Group> GetGroupsContainingUser(uint userId)
        {
            var passwordFile = this.passwordFile;
            if (passwordFile == null)
            {
                throw new StoreException();
            }

            if (!passwordFile.UsersById.TryGetValue(userId, out var user))
            {
                return null;
            }

            var groupFile = this.groupFile;
            if (groupFile == null)
            {
                throw new StoreException();
            }

            if (!groupFile.GroupsByUser.TryGetValue(user.Name, out var groups))
            {
                // The user is not a member of any secondary groups--return a collection that only includes their primary
                // group. Note: A race condition after updating a user's primary group is possible (the updated
                // `/etc/group` file might not have been processed at this point in time). We simply return an empty
                // collection if we cannot find the user's primary group.
                return groupFile.GroupsById.TryGetValue(user.GroupId, out var primaryGroup)
                    ? new Group[] { primaryGroup }
                    : Enumerable.Empty<Group>();
            }

            if (!groups.Any(group => group.GroupId == user.GroupId))
            {
                // The user is a member of secondary groups--return a collection that includes their primary and
                // secondary groups. Note: A race condition after updating a user's primary group is possible (the
                // updated `/etc/group` file might not have been processed at this point in time). We simply return the
                // collection of secondary groups if we cannot find the user's primary group.
                if (!groupFile.GroupsById.TryGetValue(user.GroupId, out var primaryGroup))
                {
                    return groups;
                }

                var list = new List<Group>(groups)
                {
                    primaryGroup
                };
                list.Sort(GroupIdSorter.Instance);
                return list;
            }

            return groups;
        }

        /// <summary>Gets the user defined with the specified user identifier.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The user or <c>null</c> if a matching user could not be found.</returns>
        public User GetUser(uint userId)
        {
            var passwordFile = this.passwordFile;
            if (passwordFile == null)
            {
                throw new StoreException();
            }

            return passwordFile.UsersById.TryGetValue(userId, out var user)
                ? user
                : null;
        }

        /// <summary>Gets an enumerable of all defined users.</summary>
        /// <returns>The enumerable of all defined users.</returns>
        public IEnumerable<User> GetUsers()
        {
            var passwordFile = this.passwordFile;
            if (passwordFile == null)
            {
                throw new StoreException();
            }

            return passwordFile.Users;
        }

        /// <summary>Sets the parsed <c>/etc/group</c> file (initially or after a change).</summary>
        /// <param name="groupFile">The parsed <c>/etc/group</c> file.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="groupFile"/> is <c>null</c>.</exception>
        public void SetGroupFile(GroupFile groupFile) =>
            this.groupFile = groupFile ?? throw new ArgumentNullException(nameof(groupFile));

        /// <summary>Sets the <c>/etc/passwd</c> file (initially or after a change).</summary>
        /// <param name="passwordFile">The parsed <c>/etc/passwd</c> file.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="passwordFile"/> is <c>null</c>.</exception>
        public void SetPasswordFile(PasswordFile passwordFile) =>
            this.passwordFile = passwordFile ?? throw new ArgumentNullException(nameof(passwordFile));
    }
}
