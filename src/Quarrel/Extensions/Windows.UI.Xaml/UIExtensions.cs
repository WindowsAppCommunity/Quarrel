// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Windows.UI.Xaml
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for UI-related types.
    /// </summary>
    public static class UIExtensions
    {
        /// <summary>
        /// Get the first parent item of a given type for the input object.
        /// </summary>
        /// <typeparam name="T">The type of the parent to look for.</typeparam>
        /// <param name="target">The source item.</param>
        /// <param name="forceLevelUp">If true and the input item is valid, it will be skipped.</param>
        /// <returns>The first parent item of a given type for the input object.</returns>
        [Pure]
        [CanBeNull]
        public static T FindParent<T>([NotNull] this DependencyObject target, bool forceLevelUp = false)
            where T : UIElement
        {
            if (target is T same && !forceLevelUp)
            {
                return same;
            }

            DependencyObject parent;
            while ((parent = VisualTreeHelper.GetParent(target)) != null)
            {
                if (parent is T found)
                {
                    return found;
                }

                target = parent;
            }

            return null;
        }

        /// <summary>
        /// Get the first element of a specific type in the visual tree of a DependencyObject.
        /// </summary>
        /// <typeparam name="T">The type of the element to find.</typeparam>
        /// <param name="parent">The object that contains the UIElement to find.</param>
        /// <returns>The sfirst element of a specific type in the visual tree of a DependencyObject.</returns>
        [Pure]
        [CanBeNull]
        public static T FindChild<T>([NotNull] this DependencyObject parent)
            where T : class
        {
            if (parent is T same)
            {
                return same;
            }

            int children = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < children; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!(child is T))
                {
                    T tChild = FindChild<T>(child);
                    if (tChild != null)
                    {
                        return tChild;
                    }
                }
                else
                {
                    return child as T;
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to find the target resource starting from the given element.
        /// </summary>
        /// <typeparam name="T">The type of the resource to retrieve.</typeparam>
        /// <param name="element">The starting element.</param>
        /// <param name="name">The name of the target resource.</param>
        /// <returns><paramref name="name"/> as a resource of type <typeparamref name="T"/>.</returns>
        [Pure]
        [CanBeNull]
        public static T FindResource<T>([NotNull] this FrameworkElement element, [NotNull] string name)
        {
            while (element != null)
            {
                if (element.Resources.TryGetValue(name, out object result))
                {
                    return (T)result;
                }

                element = element.FindParent<FrameworkElement>(true);
            }

            return (T)Application.Current.Resources[name];
        }

        /// <summary>
        /// Returns the desired <see cref="Transform"/> instance after assigning it to the <see cref="UIElement.RenderTransform"/> property of the target <see cref="UIElement"/>.
        /// </summary>
        /// <typeparam name="T">The desired <see cref="Transform"/> type.</typeparam>
        /// <param name="element">The target <see cref="UIElement"/> to modify.</param>
        /// <param name="reset">If <see langword="true"/>, a new <see cref="Transform"/> instance will always be created and assigned to the <see cref="UIElement"/>.</param>
        /// <returns>Gets the desired <see cref="Transform"/> instance after assigning it to the <see cref="UIElement.RenderTransform"/> property of the target <see cref="UIElement"/>.</returns>
        [MustUseReturnValue]
        [NotNull]
        public static T GetTransform<T>([NotNull] this UIElement element, bool reset = true)
            where T : Transform, new()
        {
            // Return the existing transform object, if it exists
            if (element.RenderTransform is T && !reset)
            {
                return element.RenderTransform as T;
            }

            // Create a new transform
            T transform = new T();
            element.RenderTransform = transform;
            return transform;
        }

        /// <summary>
        /// Gets the rectangle that contains the input <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="element">The target <see cref="FrameworkElement"/>.</param>
        /// <returns>The rectangle that contains the input <see cref="FrameworkElement"/>.</returns>
        [Pure]
        public static Rect GetContainerRectangle([NotNull] this FrameworkElement element)
        {
            return element.TransformToVisual(Window.Current.Content).TransformBounds(LayoutInformation.GetLayoutSlot(element));
        }

        /// <summary>
        /// Returns a <see cref="Visibility"/> value that reèresents the input <see langword="bool"/>.
        /// </summary>
        /// <param name="value">The <see langword="bool"/> value to convert.</param>
        /// <returns><see cref="Visibility"/> value that reèresents the input <see langword="bool"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static Visibility ToVisibility(this bool value) => value ? Visibility.Visible : Visibility.Collapsed;
    }
}
