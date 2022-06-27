// Quarrel © 2022

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls
{
    public sealed class DraftBlock : ContentControl
    {
        public static readonly DependencyProperty IsDraftedProperty = DependencyProperty.Register(
            nameof(IsDrafted), typeof(bool), typeof(DraftBlock), new PropertyMetadata(false, OnIsDraftedUpdated));

        public DraftBlock()
        {
            this.DefaultStyleKey = typeof(DraftBlock);
        }

        public bool IsDrafted
        {
            get => (bool)GetValue(IsDraftedProperty);
            set => SetValue(IsDraftedProperty, value);
        }

        private static void OnIsDraftedUpdated(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            DraftBlock draftBlock = (DraftBlock)sender;
            if ((bool)args.NewValue)
            {
                VisualStateManager.GoToState(draftBlock, "Drafted", true);
            }
            else
            {
                VisualStateManager.GoToState(draftBlock, "Normal", true);
            }
        }
    }
}
