using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using PasswdService.Models;
using PasswdService.Utilities;

namespace PasswdService.Services
{
    /// <summary>A long-running background service that watches and parses the <c>/etc/passwd</c> file.</summary>
    internal sealed class PasswordFileWatcherService : FileWatcherService
    {
        /// <summary>The field index of the comment.</summary>
        private const int COMMENT_FIELD_INDEX = 4;

        /// <summary>
        ///     Each user entry in the <c>/etc/passwd</c> file is stored on a separate line. Each user entry has fields
        ///     separated by a colon (":").
        /// </summary>
        private const int FIELD_COUNT = 7;

        /// <summary>
        ///     The field index of the primary group identifier (or "gid") as stored in the <c>/etc/group</c> file.
        /// </summary>
        private const int GROUP_ID_FIELD_INDEX = 3;

        /// <summary>
        ///     The field index of the absolute path to the directory the user will be in when they login.
        /// </summary>
        private const int HOME_FIELD_INDEX = 5;

        /// <summary>The field index of the unique name.</summary>
        private const int NAME_FIELD_INDEX = 0;

        /// <summary>The field index of the absolute path of a command or shell.</summary>
        private const int SHELL_FIELD_INDEX = 6;

        /// <summary>The field index of the unique user identifier (or "uid").</summary>
        private const int USER_ID_FIELD_INDEX = 2;

        /// <summary>The debug log.</summary>
        private readonly ILogger<PasswordFileWatcherService> logger;

        /// <summary>
        ///     The store used to share the parsed <c>/etc/passwd</c> file with other application components (e.g.
        ///     controllers).
        /// </summary>
        private readonly IStore store;

        /// <summary>Initializes a new instance of the <see cref="PasswordFileWatcherService"/> class.</summary>
        /// <param name="fileProvider">The file provider used to access the <c>/etc/passwd</c> file to watch.</param>
        /// <param name="store">
        ///     The store used to share the parsed <c>/etc/passwd</c> file with other application components (e.g.
        ///     controllers).
        /// </param>
        /// <param name="logger">The debug log.</param>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="fileProvider"/>, <paramref name="store"/>, or <paramref name="logger"/> are
        ///     <c>null</c>.
        /// </exception>
        public PasswordFileWatcherService(IPasswordFileProvider fileProvider, IStore store, ILogger<PasswordFileWatcherService> logger)
            : base(fileProvider.FileProvider, fileProvider.FileName, logger)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Called after reading the initial or changed contents of the <c>/etc/passwd</c> file. This parses the
        ///     contents of the <c>/etc/passwd</c> file and updates the store. If an error occurs parsing the contents of
        ///     the <c>/etc/passwd</c> file, the store is left unchanged.
        /// </summary>
        /// <param name="content">The initial or changed contents of the <c>/etc/passwd</c> file.</param>
        protected override void OnRead(string content)
        {
            var users = new List<User>();
            var usersById = new Dictionary<uint, User>();

            // Each user entry in the `/etc/passwd` file is stored on a separate line.
            foreach (var line in content.Split('\n').Where(line => line.Length != 0))
            {
                // Each user entry has fields separated by a colon (":").
                var fields = line.Split(':');
                if (fields.Length != FIELD_COUNT)
                {
                    this.logger.LogError("Failed to parse users: expected {FieldCount} fields in {User}", FIELD_COUNT, line);
                    return;
                }

                var name = fields[NAME_FIELD_INDEX];
                if (name.Length == 0)
                {
                    this.logger.LogError("Failed to parse users: expected non-empty name in {User}", line);
                    return;
                }

                if (!uint.TryParse(fields[USER_ID_FIELD_INDEX], NumberStyles.None, NumberFormatInfo.InvariantInfo, out var userId))
                {
                    this.logger.LogError("Failed to parse users: expected unsigned 32-bit integer user identifier (\"uid\") in {User}", line);
                    return;
                }

                if (!uint.TryParse(fields[GROUP_ID_FIELD_INDEX], NumberStyles.None, NumberFormatInfo.InvariantInfo, out var groupId))
                {
                    this.logger.LogError("Failed to parse users: expected unsigned 32-bit integer group identifier (\"gid\") in {User}", line);
                    return;
                }

                var user = new User(name, userId, groupId, fields[COMMENT_FIELD_INDEX], fields[HOME_FIELD_INDEX], fields[SHELL_FIELD_INDEX]);
                if (!usersById.TryAdd(userId, user))
                {
                    this.logger.LogError("Failed to parse users: expected unique user identifier (\"uid\") in {User}", line);
                    return;
                }

                users.Add(user);
            }

            // Sort the list of users to ensure stable API search results.
            users.Sort(UserIdSorter.Instance);

            var passwordFile = new PasswordFile(users, usersById);
            this.store.SetPasswordFile(passwordFile);
        }
    }
}
