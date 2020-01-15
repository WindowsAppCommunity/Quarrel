using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DiscordAPI.Models;
using Quarrel.Models.Bindables;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Models.Interfaces;

namespace Quarrel.TemplateSelectors
{
    public sealed class GuildMemberListSelector : DataTemplateSelector
    {
        public DataTemplate MemberTemplate { get; set; }
        public DataTemplate MemberGroupTemplate { get; set; }
        public DataTemplate PlaceholderTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement)
            {
                switch (item)
                {
                    case BindableGuildMember member:
                        return MemberTemplate;

                    case BindableGuildMemberGroup group:
                        return MemberGroupTemplate;

                    default:
                        return PlaceholderTemplate;
                }
            }

            return null;
        }
    }
}
