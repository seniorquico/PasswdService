using System.Collections.Generic;
using PasswdService.Models;

namespace PasswdService
{
    /// <summary>Represents a store of users parsed from the <c>/etc/passwd</c> file.</summary>
    public interface IUserStore
    {
        /// <summary>Gets the user defined with the specified user identifier.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The user or <c>null</c> if a matching user could not be found.</returns>
        /// <exception cref="StoreException">
        ///     If the <c>/etc/passwd</c> file has not yet been parsed, does not exist, or is malformed.
        /// </exception>
        User GetUser(uint userId);

        /// <summary>Gets an enumerable of all defined users.</summary>
        /// <returns>The enumerable of all defined users.</returns>
        /// <exception cref="StoreException">
        ///     If the <c>/etc/passwd</c> file has not yet been parsed, does not exist, or is malformed.
        /// </exception>
        IEnumerable<User> GetUsers();
    }
}
