using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.ViewModels.Settings.Pages
{
    public class MyAccountSettingsViewModel : ViewModelBase
    {
        private bool _EditingAccountInfo = false;
        public bool EditingAccountInfo
        {
            get => _EditingAccountInfo;
            set => Set(ref _EditingAccountInfo, value);
        }

        private bool _EditingPassword = false;
        public bool EditingPassword
        {
            get => _EditingPassword;
            set => Set(ref _EditingPassword, value);
        }
    }
}
