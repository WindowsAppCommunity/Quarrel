// Quarrel © 2022

using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using OwlFileAccessMode = OwlCore.AbstractStorage.FileAccessMode;
using OwlThumbnailMode = OwlCore.AbstractStorage.ThumbnailMode;
using WindowsFileAccessMode = Windows.Storage.FileAccessMode;
using WindowsThumbnailMode = Windows.Storage.FileProperties.ThumbnailMode;

namespace Quarrel.Services.Storage.Models
{
    internal class FileData : IFileData
    {
        private StorageFile _storageFile;

        public FileData(StorageFile storageFile)
        {
            _storageFile = storageFile;
            Properties = new FileDataProperties(storageFile);
        }

        /// <inheritdoc/>
        public string Id => Path;

        /// <inheritdoc/>
        public string Path => _storageFile.Path;

        /// <inheritdoc/>
        public string Name => _storageFile.Name;

        /// <inheritdoc/>
        public string DisplayName => _storageFile.DisplayName;

        /// <inheritdoc/>
        public string FileExtension => _storageFile.FileType;

        /// <inheritdoc/>
        public IFileDataProperties Properties { get; set; }

        /// <inheritdoc/>
        public async Task<IFolderData> GetParentAsync()
        {
            var storageFile = await _storageFile.GetParentAsync();

            Guard.IsNotNull(storageFile, nameof(storageFile));

            return new FolderData(storageFile);
        }

        /// <inheritdoc/>
        public Task Delete()
        {
            return _storageFile.DeleteAsync().AsTask();
        }

        /// <inheritdoc />
        public async Task<Stream> GetStreamAsync(OwlFileAccessMode accessMode = OwlFileAccessMode.Read)
        {
            var stream = await _storageFile.OpenAsync((WindowsFileAccessMode)accessMode);

            return stream.AsStream();
        }

        /// <inheritdoc />
        public Task WriteAllBytesAsync(byte[] bytes)
        {
            return FileIO.WriteBytesAsync(_storageFile, bytes).AsTask();
        }

        /// <inheritdoc />
        public async Task<Stream> GetThumbnailAsync(OwlThumbnailMode thumbnailMode, uint requiredSize)
        {
            var thumbnail = await _storageFile.GetThumbnailAsync((WindowsThumbnailMode)thumbnailMode, requiredSize);

            return thumbnail.AsStream();
        }
    }
}
