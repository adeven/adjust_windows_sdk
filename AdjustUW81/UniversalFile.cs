﻿using PCLStorage;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace AdjustSdk
{
    /// <summary>
    /// Represents a file in the <see cref="UniversalFileSystem"/>
    /// </summary>
    public class UniversalFile : IFile
    {
        /// <summary>
        /// The HRESULT on a System.Exception thrown when a file collision occurs.
        /// </summary>
        internal const int FILE_ALREADY_EXISTS = unchecked((int)0x800700B7);

        private readonly IStorageFile _WrappedFile;

        /// <summary>
        /// Creates a new <see cref="WinRTFile"/>
        /// </summary>
        /// <param name="wrappedFile">The WinRT <see cref="IStorageFile"/> to wrap</param>
        public UniversalFile(IStorageFile wrappedFile)
        {
            _WrappedFile = wrappedFile;
        }

        /// <summary>
        /// The name of the file
        /// </summary>
        public string Name
        {
            get { return _WrappedFile.Name; }
        }

        /// <summary>
        /// The "full path" of the file, which should uniquely identify it within a given <see cref="IFileSystem"/>
        /// </summary>
        public string Path
        {
            get { return _WrappedFile.Path; }
        }

        /// <summary>
        /// Opens the file
        /// </summary>
        /// <param name="fileAccess">Specifies whether the file should be opened in read-only or read/write mode</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Stream"/> which can be used to read from or write to the file</returns>
        public async Task<Stream> OpenAsync(PCLStorage.FileAccess fileAccess, CancellationToken cancellationToken)
        {
            FileAccessMode fileAccessMode;
            if (fileAccess == PCLStorage.FileAccess.Read)
            {
                fileAccessMode = FileAccessMode.Read;
            }
            else if (fileAccess == PCLStorage.FileAccess.ReadAndWrite)
            {
                fileAccessMode = FileAccessMode.ReadWrite;
            }
            else
            {
                throw new ArgumentException("Unrecognized FileAccess value: " + fileAccess);
            }

            var wrtStream = await _WrappedFile.OpenAsync(fileAccessMode).AsTask(cancellationToken).ConfigureAwait(false);
            return wrtStream.AsStream();
        }

        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <returns>A task which will complete after the file is deleted.</returns>
        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _WrappedFile.DeleteAsync().AsTask(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Renames a file without changing its location.
        /// </summary>
        /// <param name="newName">The new leaf name of the file.</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task which will complete after the file is renamed.
        /// </returns>
        public async Task RenameAsync(string newName, PCLStorage.NameCollisionOption collisionOption, CancellationToken cancellationToken)
        {            
            if (string.IsNullOrEmpty(newName))
            {
                return;
            }

            try
            {
                await _WrappedFile.RenameAsync(newName, (Windows.Storage.NameCollisionOption)collisionOption).AsTask(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex.HResult == FILE_ALREADY_EXISTS)
                {
                    throw new IOException("File already exists.", ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <param name="newPath">The new full path of the file.</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task which will complete after the file is moved.
        /// </returns>
        public async Task MoveAsync(string newPath, PCLStorage.NameCollisionOption collisionOption, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(newPath))
            {
                return;
            }

            var newFolder = await StorageFolder.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(newPath)).AsTask(cancellationToken).ConfigureAwait(false);
            string newName = System.IO.Path.GetFileName(newPath);

            try
            {
                await _WrappedFile.MoveAsync(newFolder, newName, (Windows.Storage.NameCollisionOption)collisionOption).AsTask(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex.HResult == FILE_ALREADY_EXISTS)
                {
                    throw new IOException("File already exists.", ex);
                }

                throw;
            }
        }        
    }
}
