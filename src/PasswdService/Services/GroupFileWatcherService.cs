using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using PasswdService.Models;
using PasswdService.Utilities;

namespace PasswdService.Services
{
    /// <summary>A long-running background service that watches and parses the <c>/etc/group</c> file.</summary>
    internal sealed class GroupFileWatcherService : FileWatcherService
    {
        /// <summary>
        ///     Each group entry in the <c>/etc/group</c> file is stored on a separate line. Each group entry has fields
        ///     separated by a colon (":").
        /// </summary>
        private const int FIELD_COUNT = 4;

        /// <summary>The field index of the unique group identifier (or "gid").</summary>
        private const int GROUP_ID_FIELD_INDEX = 2;

        /// <summary>The field index of the list of members.</summary>
        private const int MEMBERS_FIELD_INDEX = 3;

        /// <summary>The field index of the unique name.</summary>
        private const int NAME_FIELD_INDEX = 0;

        /// <summary>The debug log.</summary>
        private readonly ILogger<GroupFileWatcherService> logger;

        /// <summary>
        ///     The store used to share the parsed <c>/etc/group</c> file with other application components (e.g.
        ///     controllers).
        /// </summary>
        private readonly IStore store;

        /// <summary>Initializes a new instance of the <see cref="GroupFileWatcherService"/> class.</summary>
        /// <param name="fileProvider">The file provider used to access the <c>/etc/group</c> file to watch.</param>
        /// <param name="store">
        ///     The store used to share the parsed <c>/etc/group</c> file with other application components (e.g.
        ///     controllers).
        /// </param>
        /// <param name="logger">The debug log.</param>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="fileProvider"/>, <paramref name="store"/>, or <paramref name="logger"/> are
        ///     <c>null</c>.
        /// </exception>
        public GroupFileWatcherService(IGroupFileProvider fileProvider, IStore store, ILogger<GroupFileWatcherService> logger)
            : base(fileProvider.FileProvider, fileProvider.FileName, logger)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Called after reading the initial or changed contents of the <c>/etc/group</c> file. This parses the
        ///     contents of the <c>/etc/group</c> file and updates the store. If an error occurs parsing the contents of
        ///     the <c>/etc/group</c> file, the store is left unchanged.
        /// </summary>
        /// <param name="content">The initial or changed contents of the <c>/etc/group</c> file.</param>
        protected override void OnRead(string content)
        {
            var groups = new List<Group>();
            var groupsById = new Dictionary<uint, Group>();
            var groupsByUser = new Dictionary<string, List<Group>>();

            // Each group entry in the `/etc/group` file is stored on a separate line.
            foreach (var line in content.Split('\n').Where(line => line.Length != 0))
            {
                // Each group entry has fields separated by a colon (":").
                var fields = line.Split(':');
                if (fields.Length != FIELD_COUNT)
                {
                    this.logger.LogError("Failed to parse groups: expected {FieldCount} fields in {Group}", FIELD_COUNT, line);
                    return;
                }

                var name = fields[NAME_FIELD_INDEX];
                if (name.Length == 0)
                {
                    this.logger.LogError("Failed to parse groups: expected non-empty name in {Group}", line);
                    return;
                }

                if (!uint.TryParse(fields[GROUP_ID_FIELD_INDEX], NumberStyles.None, NumberFormatInfo.InvariantInfo, out var groupId))
                {
                    this.logger.LogError("Failed to parse groups: expected unsigned 32-bit integer group identifier (\"gid\") in {Group}", line);
                    return;
                }

                var rawMembers = fields[MEMBERS_FIELD_INDEX];
                List<string> members;
                if (rawMembers.Length == 0)
                {
                    members = new List<string>(0);
                }
                else
                {
                    members = rawMembers.Split(',').Where(member => member.Length != 0).ToList();
                }

                var group = new Group(name, groupId, members);
                if (!groupsById.TryAdd(groupId, group))
                {
                    this.logger.LogError("Failed to parse groups: expected unique group identifier (\"gid\") in {Group}", line);
                    return;
                }

                groups.Add(group);
                foreach (var member in members)
                {
                    if (!groupsByUser.TryGetValue(member, out var memberGroups))
                    {
                        memberGroups = groupsByUser[member] = new List<Group>();
                    }

                    memberGroups.Add(group);
                }
            }

            // Sort the lists of groups to ensure stable API search results.
            groups.Sort(GroupIdSorter.Instance);
            foreach (var groupsForUser in groupsByUser.Values)
            {
                groupsForUser.Sort(GroupIdSorter.Instance);
            }

            var groupFile = new GroupFile(groups, groupsById, groupsByUser);
            this.store.SetGroupFile(groupFile);
        }
    }
}
