using System.Collections.Generic;
using PasswdService.Models;

namespace PasswdService
{
    /// <summary>Represents a store of groups parsed from the <c>/etc/group</c> file.</summary>
    public interface IGroupStore
    {
        /// <summary>Gets the group defined with the specified group identifier.</summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>The group or <c>null</c> if a matching group could not be found.</returns>
        /// <exception cref="StoreException">
        ///     If the <c>/etc/group</c> file has not yet been parsed, does not exist, or is malformed.
        /// </exception>
        Group GetGroup(uint groupId);

        /// <summary>Gets an enumerable of all defined groups.</summary>
        /// <returns>The enumerable of all defined groups.</returns>
        /// <exception cref="StoreException">
        ///     If the <c>/etc/group</c> file has not yet been parsed, does not exist, or is malformed.
        /// </exception>
        IEnumerable<Group> GetGroups();

        /// <summary>
        ///     Gets an enumerable of all defined groups containing the user with the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        ///     The enumerable of all defined groups containing the user or <c>null</c> if a matching user could not be
        ///     found.
        /// </returns>
        /// <exception cref="StoreException">
        ///     If the <c>/etc/group</c> file has not yet been parsed, does not exist, or is malformed.
        /// </exception>
        IEnumerable<Group> GetGroupsContainingUser(uint userId);
    }
}
