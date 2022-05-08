// Quarrel © 2022

using Quarrel.Converters.Common.Text;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Quarrel.Attached
{
    /// <summary>
    /// <see href="https://stackoverflow.com/a/15615736/"/>
    /// </summary>
    public partial class TextHelpers : DependencyObject
    {
        private static bool _mutex; // This is nasty

        /// <summary>
        /// Gets the <see cref="CharacterCasing"/> value for this dependency object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CharacterCasing GetCharacterCasing(DependencyObject obj) => (CharacterCasing)obj.GetValue(CharacterCasingProperty);

        /// <summary>
        /// Sets the <see cref="CharacterCasing"/> value for this dependency object.
        /// </summary>
        public static void SetCharacterCasing(DependencyObject obj, CharacterCasing value) => obj.SetValue(CharacterCasingProperty, value);

        /// <summary>
        /// Backing dependency property for CharacterCasing attached property.
        /// </summary>
        public static readonly DependencyProperty CharacterCasingProperty =
            DependencyProperty.RegisterAttached("CharacterCasing",
                typeof(TextHelpers),
                typeof(CharacterCasing),
                new PropertyMetadata(CharacterCasing.Normal, OnCharacterCasingChanged));

        private static void OnCharacterCasingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var val = (CharacterCasing)e.NewValue;

            DependencyProperty prop = d switch
            {
                TextBlock txt => TextBlock.TextProperty,
                TextBox tbox => TextBox.PlaceholderTextProperty,
                ButtonBase hBtn => ButtonBase.ContentProperty,
                PivotItem pvi => PivotItem.HeaderProperty,
                _ => throw new ArgumentException(),
            };

            d.RegisterPropertyChangedCallback(prop, (s, e) =>
            {
                if (_mutex)
                    return;

                _mutex = true;
                d.SetValue(prop, CaseConverter.Convert((string)d.GetValue(prop), val));
                _mutex = false;
            });
        }
    }
}
