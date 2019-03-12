namespace PasswdService.Options
{
    /// <summary>The options used to configure the service.</summary>
    public sealed class ServiceOptions
    {
        /// <summary>
        ///     Gets or sets the file name of the <c>/etc/group</c> file to watch. Defaults to "group".
        /// </summary>
        public string GroupFileName { get; set; } = "group";

        /// <summary>
        ///     Gets or sets the absolute path of the <c>/etc/group</c> file to watch. Default to "/etc".
        /// </summary>
        public string GroupFilePath { get; set; } = "/etc";

        /// <summary>
        ///     Gets or sets the file name of the <c>/etc/passwd</c> file to watch. Defaults to "passwd".
        /// </summary>
        public string PasswordFileName { get; set; } = "passwd";

        /// <summary>
        ///     Gets or sets the absolute path of the <c>/etc/passwd</c> file to watch. Default to "/etc".
        /// </summary>
        public string PasswordFilePath { get; set; } = "/etc";
    }
}
