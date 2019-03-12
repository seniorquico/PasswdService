using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PasswdService.Services
{
    /// <summary>A long-running background service that watches a single file for changes.</summary>
    internal abstract class FileWatcherService : BackgroundService
    {
        /// <summary>The name of the file to watch.</summary>
        private readonly string fileName;

        /// <summary>The file provider used to access the file to watch.</summary>
        private readonly IFileProvider fileProvider;

        /// <summary>The debug log.</summary>
        private readonly ILogger<FileWatcherService> logger;

        /// <summary>Initializes a new instance of the <see cref="FileWatcherService"/> class.</summary>
        /// <param name="fileProvider">The file provider used to access the file to watch.</param>
        /// <param name="fileName">The name of the file to watch.</param>
        /// <param name="logger">The debug log.</param>
        /// <exception cref="ArgumentNullException">
        ///     If <paramref name="fileProvider"/>, <paramref name="fileName"/>, or <paramref name="logger"/> are
        ///     <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="fileName"/> is empty.</exception>
        protected FileWatcherService(IFileProvider fileProvider, string fileName, ILogger<FileWatcherService> logger)
        {
            this.fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));

            this.fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            if (fileName.Length == 0)
            {
                throw new ArgumentException("The file name must not be empty.", nameof(fileName));
            }

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Asynchronously executes the background service. This calls
        ///     <see cref="ReadAsync(IFileInfo, CancellationToken)"/> on start of the background service (to enable
        ///     reading the initial contents of the file) and whenever the file changes (to enable reading the changed
        ///     contents of the file).
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // Read the initial contents of the file.
            try
            {
                this.logger.LogInformation("Reading initial contents of file {File}", this.fileName);
                var file = this.fileProvider.GetFileInfo(this.fileName);
                await this.ReadAsync(file, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                this.logger.LogInformation("Read initial contents of file {File}", this.fileName);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to read initial contents of file {File}", this.fileName);
            }

            // Loop until the cancellation token relays the request to shutdown the background service.
            while (true)
            {
                IDisposable cancellationTokenRegistration = null;
                IDisposable changeTokenRegistration = null;
                try
                {
                    // This token notifies us (only once!) when the contents of the file have changed.
                    var changeToken = this.fileProvider.Watch(this.fileName);

                    // The task completion source will be used to race the two tokens.
                    var taskCompletionSource = new TaskCompletionSource<object>();
                    cancellationTokenRegistration = cancellationToken.Register(
                        state => ((TaskCompletionSource<object>)state).TrySetCanceled(), taskCompletionSource);
                    changeTokenRegistration = changeToken.RegisterChangeCallback(
                        state => ((TaskCompletionSource<object>)state).TrySetResult(null), taskCompletionSource);

                    // Wait for either the contents of the file to change or the request to shutdown the background
                    // service.
                    await taskCompletionSource.Task.ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();

                    this.logger.LogInformation("Reading changed contents of file {File}", this.fileName);
                    var file = this.fileProvider.GetFileInfo(this.fileName);
                    await this.ReadAsync(file, cancellationToken).ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();
                    this.logger.LogInformation("Read changed contents of file {File}", this.fileName);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, "Failed to read changed contents of file {File}", this.fileName);
                }
                finally
                {
                    if (cancellationTokenRegistration != null)
                    {
                        cancellationTokenRegistration.Dispose();
                        cancellationTokenRegistration = null;
                    }

                    if (changeTokenRegistration != null)
                    {
                        changeTokenRegistration.Dispose();
                        changeTokenRegistration = null;
                    }
                }
            }
        }

        /// <summary>Called after reading the initial or changed contents of the file.</summary>
        /// <param name="content">The initial or changed contents of the file.</param>
        protected abstract void OnRead(string content);

        /// <summary>
        ///     Asynchronously reads the file. This is called on start of the background service (to enable reading the
        ///     initial contents of the file) and whenever the file changes (to enable reading the changed contents of
        ///     the file).
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task ReadAsync(IFileInfo file, CancellationToken cancellationToken)
        {
            var filePhysicalPath = file.PhysicalPath;
            string content;
            if (filePhysicalPath == null)
            {
                using (var stream = file.CreateReadStream())
                using (var streamReader = new StreamReader(stream, Encoding.ASCII))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    content = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                }
            }
            else
            {
                content = await File.ReadAllTextAsync(filePhysicalPath, Encoding.ASCII, cancellationToken).ConfigureAwait(false);
            }

            cancellationToken.ThrowIfCancellationRequested();
            this.OnRead(content);
        }
    }
}
