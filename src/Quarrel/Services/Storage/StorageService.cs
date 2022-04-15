// Quarrel © 2022

using OwlCore.AbstractStorage;
using OwlCore.Services;
using Quarrel.Services.Storage.Accounts;
using Quarrel.Services.Storage.Vault;
using System.IO;

namespace Quarrel.Services.Storage
{
    public class StorageService : IStorageService
    {
        public StorageService(IFolderData folder, IAsyncSerializer<Stream> serializer)
        {
            VaultService = new VaultService();
            AccountInfoStorage = new AccountInfoStorage(VaultService, folder, serializer);
        }

        IAccountInfoStorage IStorageService.AccountInfoStorage => AccountInfoStorage;

        IVaultService IStorageService.VaultService => VaultService;

        public AccountInfoStorage AccountInfoStorage { get; }

        public VaultService VaultService { get; }
    }
}
