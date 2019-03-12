using System.ComponentModel.DataAnnotations;

namespace PasswdService.Models
{
    /// <summary>
    ///     Represents a query string used to find one or more users defined in the <c>/etc/passwd</c> file.
    /// </summary>
    public sealed class UserQuery
    {
#pragma warning disable IDE1006 // Naming Styles

        /// <summary>The comment. If specified, may be an empty string.</summary>
        public string comment { get; set; }

        /// <summary>The primary group identifier (or "gid").</summary>
        public uint? gid { get; set; }

        /// <summary>
        ///     The absolute path to the directory the user will be in when they login. If specified, must not be an
        ///     empty string.
        /// </summary>
        [StringLength(int.MaxValue, MinimumLength = 1)]
        public string home { get; set; }

        /// <summary>The unique name. If specified, must not be an empty string.</summary>
        [StringLength(int.MaxValue, MinimumLength = 1)]
        public string name { get; set; }

        /// <summary>The absolute path of a command or shell. If specified, must not be an empty string.</summary>
        [StringLength(int.MaxValue, MinimumLength = 1)]
        public string shell { get; set; }

        /// <summary>The unique user identifier (or "uid").</summary>
        public uint? uid { get; set; }

#pragma warning restore IDE1006 // Naming Styles
    }
}
