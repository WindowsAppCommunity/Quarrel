// Copyright (c) Quarrel. All rights reserved.

using System;
using System.IO;
using System.Threading.Tasks;

namespace Quarrel.Helpers
{
    /// <summary>
    /// Methods for parsing image data.
    /// </summary>
    public static class ImageParsing
    {
        /// <summary>
        /// Gets a <see cref="Windows.Storage.StorageFile"/> as a byte array.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <returns><paramref name="file"/> as a byte array.</returns>
        public static async Task<byte[]> FileToBytes(Windows.Storage.StorageFile file)
        {
            using (var inputStream = await file.OpenSequentialReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();
                var byteArray = new byte[readStream.Length];
                await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                return byteArray;
            }
        }
    }
}
