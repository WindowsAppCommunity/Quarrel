using Quarrel.ViewModels.Models.Bindables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
