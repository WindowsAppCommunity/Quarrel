// Quarrel © 2022

using OwlCore.AbstractStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using CreationCollisionOption = OwlCore.AbstractStorage.CreationCollisionOption;

namespace Quarrel.Services.Storage.Models
{
    internal class FolderData : IFolderData
    {
        public StorageFolder _storageFolder { get; private set; }

        public FolderData(StorageFolder folder)
        {
            _storageFolder = folder;
        }

        /// <inheritdoc/>
        public string Name => _storageFolder.Name;

        /// <inheritdoc/>
        public string Path => _storageFolder.Path;

        /// <inheritdoc/>
        public string? Id { get; set; }

        /// <inheritdoc/>
        public async Task<IEnumerable<IFileData>> GetFilesAsync()
        {
            var files = await _storageFolder.GetFilesAsync();

            return files.Select(x => new FileData(x)).ToArray();
        }

        /// <inheritdoc />
        public Task DeleteAsync() => _storageFolder.DeleteAsync().AsTask();

        /// <inheritdoc/>
        public async Task<IFolderData?> GetParentAsync()
        {
            var storageFolder = await _storageFolder.GetParentAsync();

            return new FolderData(storageFolder);
        }

        /// <inheritdoc/>
        public async Task<IFolderData> CreateFolderAsync(string desiredName)
        {
            var storageFolder = await _storageFolder.CreateFolderAsync(desiredName);

            return new FolderData(storageFolder);
        }

        /// <inheritdoc/>
        public async Task<IFolderData> CreateFolderAsync(string desiredName, CreationCollisionOption options)
        {
            var collisionOptions = (Windows.Storage.CreationCollisionOption)Enum.Parse(typeof(Windows.Storage.CreationCollisionOption), options.ToString());

            var storageFolder = await _storageFolder.CreateFolderAsync(desiredName, collisionOptions);

            return new FolderData(storageFolder);
        }

        /// <inheritdoc/>
        public async Task<IFileData> CreateFileAsync(string desiredName)
        {
            var storageFile = await _storageFolder.CreateFileAsync(desiredName);

            return new FileData(storageFile);
        }

        /// <inheritdoc/>
        public async Task<IFileData> CreateFileAsync(string desiredName, CreationCollisionOption options)
        {
            var collisionOptions = (Windows.Storage.CreationCollisionOption)Enum.Parse(typeof(Windows.Storage.CreationCollisionOption), options.ToString());
            var storageFile = await _storageFolder.CreateFileAsync(desiredName, collisionOptions);

            return new FileData(storageFile);
        }

        /// <inheritdoc/>
        public async Task<IFolderData?> GetFolderAsync(string name)
        {
            var folderData = await _storageFolder.GetFolderAsync(name);

            return new FolderData(folderData);
        }

        /// <inheritdoc/>
        public async Task<IFileData?> GetFileAsync(string name)
        {
            var fileData = await _storageFolder.GetFileAsync(name);

            return new FileData(fileData);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IFolderData>> GetFoldersAsync()
        {
            var foldersData = await _storageFolder.GetFoldersAsync();

            return foldersData.Select(x => new FolderData(x));
        }

        /// <inheritdoc />
        public async Task EnsureExists()
        {
            try
            {
                _ = StorageFolder.GetFolderFromPathAsync(_storageFolder.Path);
            }
            catch
            {
                _storageFolder = await _storageFolder.CreateFolderAsync(_storageFolder.Name);
            }
        }
    }
}
