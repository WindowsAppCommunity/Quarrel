// Quarrel © 2022

using Quarrel.Bindables.Messages;
using Quarrel.Bindables.Users;
using Quarrel.Markdown.Parsing;
using Windows.UI.Xaml;

namespace Quarrel.Markdown
{
    public class UserMentionElement : MarkdownElement
    {
        public static readonly DependencyProperty UserProperty = DependencyProperty.Register(
            nameof(User), typeof(BindableUser), typeof(UserMentionElement), new PropertyMetadata(null));

        public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register(
            nameof(UserName), typeof(string), typeof(UserMentionElement), new PropertyMetadata(null));

        internal UserMentionElement(UserMention mention, BindableMessage? context) : base(mention)
        {
            this.DefaultStyleKey = typeof(UserMentionElement);
            if (context is null)
            {
                return;
            }

            if (context.Users.ContainsKey(mention.UserID))
            {
                User = context.Users[mention.UserID];
                UserName = User.User.Username;
            }

        }

        public BindableUser User
        {
            get => (BindableUser)GetValue(UserProperty);
            set => SetValue(UserProperty, value);
        }

        public string UserName
        {
            get => (string)GetValue(UserNameProperty);
            set => SetValue(UserNameProperty, value);
        }
    }
}
