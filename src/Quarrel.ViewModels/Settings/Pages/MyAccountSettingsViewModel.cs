using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace Quarrel.ViewModels.Settings.Pages
{
    public class MyAccountSettingsViewModel : ViewModelBase
    {
        private bool editingAccountInfo;
        public bool EditingAccountInfo
        {
            get => editingAccountInfo;
            set => Set(ref editingAccountInfo, value);
        }

        private bool editingPassword;
        public bool EditingPassword
        {
            get => editingPassword;
            set => Set(ref editingPassword, value);
        }


        private RelayCommand enterAccountEditCommand;
        public RelayCommand EnterAccountEditCommand => enterAccountEditCommand = new RelayCommand(() =>
        {
            EditingAccountInfo = true;
        });

        private RelayCommand finalizeAccountEditCommand;
        public RelayCommand FinalizeAccountEditCommand => finalizeAccountEditCommand = new RelayCommand(() =>
        {
            // TODO:
        });

        private RelayCommand cancelAccountEditCommand;
        public RelayCommand CancelAccountEditCommand => cancelAccountEditCommand = new RelayCommand(() =>
        {
            EditingAccountInfo = false;
            EditingPassword = false;
        });


        private RelayCommand enterPasswordEditCommand;
        public RelayCommand EnterPasswordEditCommand => enterPasswordEditCommand = new RelayCommand(() =>
        {
            EditingPassword = true;
        });

    }
}
