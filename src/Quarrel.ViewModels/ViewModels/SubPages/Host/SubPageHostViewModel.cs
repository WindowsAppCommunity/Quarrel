// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Messages.Navigation.SubPages;
using System;
using System.Collections.Generic;

namespace Quarrel.ViewModels.SubPages.Host
{
    /// <summary>
    /// ViewModel for the app's SubPageHost.
    /// </summary>
    public partial class SubPageHostViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;
        private readonly Stack<object> _navStack;

        private object? _contentViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubPageHostViewModel"/> class.
        /// </summary>
        public SubPageHostViewModel(IMessenger messenger)
        {
            _messenger = messenger;
            _navStack = new Stack<object>();

            _messenger.Register<NavigateToSubPageMessage>(this, (s, e) => NavigateToSubPage(e.TargetViewModelType));
            _messenger.Register<GoBackSubPageMessage>(this, (s, e) => HandleGoBackSubPage());
        }

        /// <summary>
        /// Gets or sets the view model for the current content.
        /// </summary>
        public object? ContentViewModel
        {
            get => _contentViewModel;
            set
            {
                if (SetProperty(ref _contentViewModel, value))
                {
                    OnPropertyChanged(nameof(IsVisible));
                    OnPropertyChanged(nameof(IsStacked));
                }
            }
        }

        /// <summary>
        /// Gets whether or not the SubPageHost is visible.
        /// </summary>
        public bool IsVisible => ContentViewModel is not null;

        /// <summary>
        /// Gets whether or not the sub pages are currently stacked.
        /// </summary>
        public bool IsStacked => _navStack.Count > 0;
        
        [ICommand]
        private void NavigateToSubPage(Type viewModelType)
        {
            // TODO: Investigate alternative to using Ioc
            object viewModel = Ioc.Default.GetRequiredService(viewModelType);
            if (ContentViewModel is not null)
            {
                _navStack.Push(ContentViewModel);
            }

            ContentViewModel = viewModel;
        }
        
        [ICommand]
        private void GoBackSubPage()
        {
            _messenger.Send(new GoBackSubPageMessage());
        }

        private void HandleGoBackSubPage()
        {
            if (_navStack.Count > 0)
            {
                object viewModel = _navStack.Pop();
                ContentViewModel = viewModel;
                return;
            }

            ContentViewModel = null;
        }
    }
}
