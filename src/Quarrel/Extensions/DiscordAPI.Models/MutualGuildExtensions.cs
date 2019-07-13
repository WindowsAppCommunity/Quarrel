using CommonServiceLocator;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using Quarrel.Models.Bindables;
using Quarrel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Helpers;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Services.Cache;
using Quarrel.Services.Rest;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Posts.Requests;

namespace DiscordAPI.Models
{
    internal static class MutualGuildExtentions
    {
        public static BindableGuild Guild(this MutualGuild mg) =>  Messenger.Default.Request<BindableGuildRequestMessage, BindableGuild>(new BindableGuildRequestMessage(mg.Id));

        public static string GetName(this MutualGuild mg) => Guild(mg).Model.Name;

        public static string GetIconUrl(this MutualGuild mg)
        {
            return Guild(mg).IconUrl;
        }
    }
}