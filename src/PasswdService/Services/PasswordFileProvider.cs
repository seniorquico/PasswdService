using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using PasswdService.Options;

namespace PasswdService.Services
{
    /// <summary>
    ///     The dependency injection utility that provides the configured name of the <c>/etc/passwd</c> file to watch
    ///     and an <see cref="IFileProvider"/> for a physical file system used to access the <c>/etc/passwd</c> file to
    ///     watch.
    /// </summary>
    internal sealed class PasswordFileProvider : IDisposable, IPasswordFileProvider
    {
        /// <summary>The <see cref="IFileProvider"/> used to access the <c>/etc/passwd</c> file to watch.</summary>
        private readonly PhysicalFileProvider fileProvider;

        /// <summary>Initializes a new instance of the <see cref="PasswordFileProvider"/> class.</summary>
        /// <param name="options">The service options.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="options"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="options"/> is invalid.</exception>
        public PasswordFileProvider(IOptions<ServiceOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var serviceOptions = options.Value;
            if (serviceOptions == null)
            {
                throw new ArgumentException("The value must not be null.", nameof(options));
            }

            this.FileName = serviceOptions.PasswordFileName ?? throw new ArgumentException("The password file name must not be null.", nameof(options));
            if (this.FileName.Length == 0)
            {
                throw new ArgumentException("The password file name must not be empty.", nameof(options));
            }

            this.fileProvider = new PhysicalFileProvider(serviceOptions.PasswordFilePath ?? throw new ArgumentException("The password file path must not be null.", nameof(options)));
        }

        /// <summary>Gets the configured name of the <c>/etc/passwd</c> file to watch.</summary>
        public string FileName { get; }

        /// <summary>
        ///     Gets the <see cref="IFileProvider"/> used to access the <c>/etc/passwd</c> file to watch.
        /// </summary>
        public IFileProvider FileProvider => this.fileProvider;

        /// <summary>Disposes the underlying <see cref="IFileProvider"/>.</summary>
        public void Dispose() =>
            this.fileProvider.Dispose();
    }
}
