// Quarrel © 2022

using Quarrel.ViewModels.SubPages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors
{
    public class SubPageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? AboutTemplate { get; set; }
        
        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return item switch
            {
                AboutPageViewModel => AboutTemplate,
                _ => null,
            };
        }
    }
}
