using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PasswdService.Utilities;

namespace PasswdService.Models
{
    /// <summary>
    ///     Represents a query string used to find one or more groups defined in the <c>/etc/group</c> file.
    /// </summary>
    public sealed class GroupQuery
    {
#pragma warning disable IDE1006 // Naming Styles

        /// <summary>The group identifier.</summary>
        public uint? gid { get; set; }

        /// <summary>The list of group members (as user names).</summary>
        [ListStringLength(int.MaxValue, MinimumLength = 1)]
        public List<string> members { get; set; }

        /// <summary>The unique name. If specified, must not be an empty string.</summary>
        [StringLength(int.MaxValue, MinimumLength = 1)]
        public string name { get; set; }

#pragma warning restore IDE1006 // Naming Styles
    }
}
