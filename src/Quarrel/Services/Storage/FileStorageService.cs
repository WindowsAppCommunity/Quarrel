// Quarrel © 2022

using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace Quarrel.Services.Storage
{
    public class FileStorageService : IFileStorageService
    {
        private readonly StorageFolder _storageFolder;

        public FileStorageService(StorageFolder storageFolder)
        {
            _storageFolder = storageFolder;
        }

        public async Task<string> GetFileAsync(string name)
        {
            var file = await _storageFolder.CreateFileAsync(name, CreationCollisionOption.OpenIfExists);
            return await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8);
        }

        public async Task WriteFileAsync(string name, string contents)
        {
            var file = await _storageFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, contents, UnicodeEncoding.Utf8);
        }
    }
}
