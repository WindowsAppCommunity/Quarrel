// Adam Dernis © 2022

using Discord.API.Models.Channels.Abstract;
using Quarrel.Bindables.Channels.Abstract;
using System.Collections.ObjectModel;

namespace Quarrel.Bindables.Channels
{
    public class BindableCategoryChannel : BindableChannel
    {
        public BindableCategoryChannel(Channel channel) : base(channel)
        {
            Children = new ObservableCollection<BindableChannel>();
        }

        public ObservableCollection<BindableChannel> Children { get; }

        public void AddChild(BindableChannel child)
        {
            Children.Add(child);
        }
    }
}
