using Windows.UI.Composition;

namespace Discord_UWP.Controls
{
    internal class CompositionImageFactory
    {
        private Compositor _compositor;

        public CompositionImageFactory(Compositor compositor)
        {
            _compositor = compositor;
        }
    }
}