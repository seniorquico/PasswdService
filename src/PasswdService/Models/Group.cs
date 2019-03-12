using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PasswdService.Models
{
    /// <summary>Represents a group defined in the <c>/etc/group</c> file.</summary>
    public sealed class Group
    {
        /// <summary>Initializes a new instance of the <see cref="Group"/> class.</summary>
        /// <param name="name">The unique group name.</param>
        /// <param name="groupId">The unique group identifier (or "gid").</param>
        /// <param name="members">The list of group members (as user names).</param>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="name"/> or <paramref name="members"/> is <c>null</c>.
        /// </exception>
        public Group(string name, uint groupId, IEnumerable<string> members)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.GroupId = groupId;
            this.Members = members ?? throw new ArgumentNullException(nameof(members));
        }

        /// <summary>The unique group name.</summary>
        [JsonProperty(PropertyName = "name")]
        [Required]
        public string Name { get; }

        /// <summary>The unique group identifier (or "gid").</summary>
        [JsonProperty(PropertyName = "gid")]
        [Required]
        public uint GroupId { get; }

        /// <summary>The list of group members (as user names).</summary>
        [JsonProperty(PropertyName = "members")]
        [Required]
        public IEnumerable<string> Members { get; }
    }
}
