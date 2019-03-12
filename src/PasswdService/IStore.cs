using System;
using PasswdService.Models;

namespace PasswdService
{
    /// <summary>
    ///     Represents a store of groups and users parsed from the <c>/etc/group</c> and <c>/etc/passwd</c> files,
    ///     respectively. Application components (e.g. controllers) should prefer to depend on <see cref="IGroupStore"/>
    ///     and <see cref="IUserStore"/> directly. The <see cref="IStore"/> abstraction should be reserved for setting
    ///     the parsed <c>/etc/group</c> and <c>/etc/passwd</c> files.
    /// </summary>
    public interface IStore : IGroupStore, IUserStore
    {
        /// <summary>Sets the parsed <c>/etc/group</c> file.</summary>
        /// <param name="groupFile">The parsed <c>/etc/group</c> file.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="groupFile"/> is <c>null</c>.</exception>
        void SetGroupFile(GroupFile groupFile);

        /// <summary>Sets the parsed <c>/etc/passwd</c> file.</summary>
        /// <param name="passwordFile">The parsed <c>/etc/passwd</c> file.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="passwordFile"/> is <c>null</c>.</exception>
        void SetPasswordFile(PasswordFile passwordFile);
    }
}
