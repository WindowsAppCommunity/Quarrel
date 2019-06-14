using Windows.UI.Composition;

namespace Quarrel.Controls
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