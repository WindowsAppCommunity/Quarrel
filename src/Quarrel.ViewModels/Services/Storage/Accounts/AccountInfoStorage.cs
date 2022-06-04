// Quarrel © 2022

using Quarrel.Services.Storage.Accounts.Models;
using Quarrel.Services.Storage.Vault;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Quarrel.Services.Storage.Accounts
{
    public record struct AccountStorageState(IEnumerable<AccountInfo>? Accounts, ulong? AccountId);

    /// <summary>
    /// A service that stores info about the user's account.
    /// </summary>
    public class AccountInfoStorage : IAccountInfoStorage
    {
        private readonly IVaultService _vaultService;
        private readonly IFileStorageService _fileStorageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountInfoStorage"/> class.
        /// </summary>
        public AccountInfoStorage(IVaultService vaultService, IFileStorageService fileStorageService)
        {
            _vaultService = vaultService;
            _fileStorageService = fileStorageService;
        }

        /// <inheritdoc/>
        public AccountInfo? ActiveAccount
        {
            get => ActiveAccountId != null ? _accounts[ActiveAccountId.Value] : null;
            set => _accountId = value?.Id;
        }

        private readonly Dictionary<ulong, AccountInfo> _accounts = new();
        private ulong? _accountId = null;

        /// <summary>
        /// Gets list of accounts in storage.
        /// </summary>
        public IReadOnlyDictionary<ulong, AccountInfo> Accounts => _accounts;

        private ulong? ActiveAccountId => _accountId;

        /// <inheritdoc/>
        public bool SelectAccount(ulong id)
        {
            if (_accounts.ContainsKey(id))
            {
                _accountId = id;
                return true;
            }

            return false;
        }
        
        /// <inheritdoc/>
        public bool RegisterAccount(AccountInfo accountInfo)
        {
            if (_accounts.ContainsKey(accountInfo.Id)) return false;

            _accounts.Add(accountInfo.Id, accountInfo with { Token = string.Empty });
            _vaultService.RegisterUserToken(accountInfo.Id, accountInfo.Token);
            return true;

        }
        
        /// <inheritdoc/>
        public bool UnregisterAccount(ulong id)
        {
            // TODO: Handle active account 
            _vaultService.UnregisterToken(id);
            return _accounts.Remove(id);
        }

        /// <inheritdoc/>
        public async Task LoadAsync()
        {
            string contents = await _fileStorageService.GetFileAsync("accounts.json");
            if (string.IsNullOrEmpty(contents)) return;
            try
            {
                var state = await JsonSerializer.DeserializeAsync<AccountStorageState>(new MemoryStream(Encoding.UTF8.GetBytes(contents)));
                _accounts.Clear();
                if (state.Accounts != null)
                {
                    foreach (var account in state.Accounts)
                    {
                        _accounts.Add(account.Id, account with { Token = _vaultService.GetUserToken(account.Id) ?? string.Empty });
                    }
                }

                _accountId = state.AccountId;
            }
            catch (JsonException e)
            {

            }
        }
        
        /// <inheritdoc/>
        public async Task SaveAsync()
        {
            var state = new AccountStorageState(_accounts.Values.Select(x => x with { Token = string.Empty }), _accountId);

            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, state);
            stream.Position = 0;
            string contents = await new StreamReader(stream, Encoding.UTF8).ReadToEndAsync();

            await _fileStorageService.WriteFileAsync("accounts.json", contents);
        }
    }
}
