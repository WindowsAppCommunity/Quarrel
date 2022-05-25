// Quarrel © 2022

using Quarrel.Services.Storage.Accounts;
using Quarrel.Services.Storage.Vault;
using System.IO;

namespace Quarrel.Services.Storage
{
    public class StorageService : IStorageService
    {
        public StorageService(IVaultService vaultService, IAccountInfoStorage accountInfoStorage)
        {
            VaultService = vaultService;
            AccountInfoStorage = accountInfoStorage;
        }

        public IAccountInfoStorage AccountInfoStorage { get; }

        public IVaultService VaultService { get; }
    }
}
