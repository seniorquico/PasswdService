﻿using Microsoft.Extensions.FileProviders;

namespace PasswdService
{
    /// <summary>
    ///     A dependency injection utility that provides the configured name of the <c>/etc/passwd</c> file to watch and
    ///     an <see cref="IFileProvider"/> used to access the <c>/etc/passwd</c> file to watch.
    /// </summary>
    public interface IPasswordFileProvider
    {
        /// <summary>Gets the name of the <c>/etc/passwd</c> file to watch.</summary>
        string FileName { get; }

        /// <summary>Gets the file provider used to access the <c>/etc/passwd</c> file to watch.</summary>
        IFileProvider FileProvider { get; }
    }
}
