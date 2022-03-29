// Adam Dernis © 2022

using OwlCore.AbstractStorage;
using OwlCore.Services;
using Quarrel.ViewModels.Services.Storage.Models;
using System.IO;

namespace Quarrel.ViewModels.Services.Storage
{
    /// <summary>
    /// A storage class that stores info about the user's account.
    /// </summary>
    public class AccountInfoStorage : SettingsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountInfoStorage"/> class.
        /// </summary>
        public AccountInfoStorage(IFolderData folder, IAsyncSerializer<Stream> settingSerializer) : base(folder, settingSerializer)
        {
        }

        /// <summary>
        /// Gets or sets the account info in storage.
        /// </summary>
        AccountInfo AccountInfo
        {
            get => GetSetting(() => new AccountInfo());
            set => SetSetting(value);
        }
    }
}
