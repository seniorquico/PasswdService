using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PasswdService.Models
{
    /// <summary>Represents a user defined in the <c>/etc/passwd</c> file.</summary>
    public sealed class User
    {
        /// <summary>Initializes a new instance of the <see cref="User"/> class.</summary>
        /// <param name="name">The unique group name.</param>
        /// <param name="userId">The unique user identifier (or "uid").</param>
        /// <param name="groupId">
        ///     The primary group identifier (or "gid") as stored in the <c>/etc/group</c> file.
        /// </param>
        /// <param name="comment">The comment.</param>
        /// <param name="home">The absolute path to the directory the user will be in when they login.</param>
        /// <param name="shell">The absolute path of a command or shell.</param>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="name"/>, <paramref name="comment"/>, <paramref name="home"/>, or
        ///     <paramref name="shell"/> are <c>null</c>.
        /// </exception>
        public User(string name, uint userId, uint groupId, string comment, string home, string shell)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.UserId = userId;
            this.GroupId = groupId;
            this.Comment = comment ?? throw new ArgumentNullException(nameof(comment));
            this.Home = home ?? throw new ArgumentNullException(nameof(home));
            this.Shell = shell ?? throw new ArgumentNullException(nameof(shell));
        }

        /// <summary>The unique name.</summary>
        [JsonProperty(PropertyName = "name")]
        [Required]
        public string Name { get; }

        /// <summary>The unique user identifier (or "uid").</summary>
        [JsonProperty(PropertyName = "uid")]
        [Required]
        public uint UserId { get; }

        /// <summary>The primary group identifier (or "gid") as stored in the <c>/etc/group</c> file.</summary>
        [JsonProperty(PropertyName = "gid")]
        [Required]
        public uint GroupId { get; }

        /// <summary>The comment.</summary>
        [JsonProperty(PropertyName = "comment")]
        [Required]
        public string Comment { get; }

        /// <summary>The absolute path to the directory the user will be in when they login.</summary>
        [JsonProperty(PropertyName = "home")]
        [Required]
        public string Home { get; }

        /// <summary>The absolute path of a command or shell.</summary>
        [JsonProperty(PropertyName = "shell")]
        [Required]
        public string Shell { get; }
    }
}
