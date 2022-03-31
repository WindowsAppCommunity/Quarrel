// Adam Dernis © 2022

using Quarrel.Services.Storage.Accounts;

namespace Quarrel.Services.Storage
{
    public interface IStorageService
    {
        IAccountInfoStorage AccountInfoStorage { get; }
    }
}
