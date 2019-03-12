using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PasswdService.Models
{
    /// <summary>Represents a parsed, immutable <c>/etc/group</c> file.</summary>
    public sealed class GroupFile
    {
        /// <summary>Initializes a new instance of the <see cref="GroupFile"/> class.</summary>
        /// <param name="groups">The list of groups pre-sorted by unique group identifier (or "gid").</param>
        /// <param name="groupsById">The dictionary that maps unique group identifiers (or "gid") to groups.</param>
        /// <param name="groupsByUser">
        ///     The dictionary that maps unique user names to lists of groups to which they are members.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="groups"/>, <paramref name="groupsById"/>, or <paramref name="groupsByUser"/> are
        ///     <c>null</c>.
        /// </exception>
        public GroupFile(List<Group> groups, Dictionary<uint, Group> groupsById, Dictionary<string, List<Group>> groupsByUser)
        {
            if (groups == null)
            {
                throw new ArgumentNullException(nameof(groups));
            }

            if (groupsById == null)
            {
                throw new ArgumentNullException(nameof(groupsById));
            }

            if (groupsByUser == null)
            {
                throw new ArgumentNullException(nameof(groupsByUser));
            }

            this.Groups = groups.AsReadOnly();
            this.GroupsById = new ReadOnlyDictionary<uint, Group>(groupsById);
            this.GroupsByUser = new ReadOnlyDictionary<string, IReadOnlyList<Group>>(
                groupsByUser.ToDictionary<KeyValuePair<string, List<Group>>, string, IReadOnlyList<Group>>(
                    o => o.Key,
                    o => o.Value.AsReadOnly()));
        }

        /// <summary>Gets a read-only list of groups pre-sorted by unique group identifier (or "gid").</summary>
        public IReadOnlyList<Group> Groups { get; }

        /// <summary>Gets a read-only dictionary that maps unique group identifiers (or "gid") to groups.</summary>
        public IReadOnlyDictionary<uint, Group> GroupsById { get; }

        /// <summary>
        ///     Gets a read-only dictionary that maps unique user names to lists of groups to which they are members.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyList<Group>> GroupsByUser { get; }
    }
}
