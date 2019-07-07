using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DiscordAPI.API.User;
using Quarrel.Models.Bindables;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Posts.Requests;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using GalaSoft.MvvmLight.Threading;
using Quarrel.Services.Users;

namespace Quarrel.ViewModels
{
    public class MemberViewModel : ViewModelBase
    {
        public ICurrentUsersService UserService => SimpleIoc.Default.GetInstance<ICurrentUsersService>();

    }
}
