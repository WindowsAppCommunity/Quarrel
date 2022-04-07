// Adam Dernis © 2022

using Quarrel.Bindables.Channels.Abstract;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.Bindables.Channels
{
    public class BindableChannelGroup : IGrouping<BindableCategoryChannel?, BindableChannel?>
    {
        public BindableChannelGroup(BindableCategoryChannel? key)
        {
            Key = key;

            Children = new ObservableCollection<BindableChannel>();
        }

        /// <inheritdoc/>
        public BindableCategoryChannel? Key { get; }

        public ObservableCollection<BindableChannel> Children { get; }

        public IEnumerator<BindableChannel> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddChild(BindableChannel child)
        {
            Children.Add(child);
        }
    }
}
