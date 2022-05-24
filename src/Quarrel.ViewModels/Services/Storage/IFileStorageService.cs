// Quarrel © 2022

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Services.Storage
{
    public interface IFileStorageService
    {
        Task<string> GetFileAsync(string name);

        Task WriteFileAsync(string name, string contents);
    }
}
