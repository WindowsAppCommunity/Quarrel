// Adam Dernis © 2022

using OwlCore.AbstractStorage;
using OwlCore.Services;
using Quarrel.Services.Storage.Accounts;
using System.IO;

namespace Quarrel.Services.Storage
{
    public class StorageService : SettingsBase, IStorageService
    {
        public StorageService(IFolderData folder, IAsyncSerializer<Stream> serializer) : base(folder, serializer)
        {
            AccountInfoStorage = new AccountInfoStorage(folder, serializer);
        }

        public IAccountInfoStorage AccountInfoStorage { get; }
    }
}
