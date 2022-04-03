// SubPage frame created by Sergio Pedri for BrainF*ck and Legere
// View Code in BrainF*ck
// https://github.com/Sergio0694/Brainf_ckSharp

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.Host
{
    public class SubPageFrame : ContentControl
    {
        /// <summary>
        /// The dependency property for <see cref="Title"/>.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(SubPageFrame),
            new(string.Empty));

        /// <summary>
        /// Gets or sets the title for the current frame.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
    }
}
