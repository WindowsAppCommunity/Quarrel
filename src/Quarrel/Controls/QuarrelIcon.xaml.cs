using System;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls
{
    /// <summary>
    /// A control that displays an animated Quarrel icon.
    /// </summary>
    public sealed partial class QuarrelIcon : UserControl
    {
        private bool _isAnimationEnding;
        private bool _encounteredError;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuarrelIcon"/> class.
        /// </summary>
        public QuarrelIcon()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Fired when the ending animation completes.
        /// </summary>
        public event EventHandler? AnimationFinished;

        /// <summary>
        /// A function that tells to the control to begin the animation.
        /// </summary>
        public void BeginAnimation()
        {
            _isAnimationEnding = false;
            _encounteredError = false;
            BeginningAnimation.Begin();
        }

        /// <summary>
        /// A function that tells the control to begin ending the animation.
        /// </summary>
        public void FinishAnimation()
        {
            _isAnimationEnding = true;
        }

        /// <summary>
        /// TODO: A function that tells the control to begin entering the error state.
        /// </summary>
        private void ErrorAnimation()
        {
            _encounteredError = true;
            _isAnimationEnding = true;
        }

        private void BeginningAnimation_Completed(object sender, object e)
        {
            RepeatingAnimation.Begin();
        }

        private void RepeatingAnimation_Completed(object sender, object e)
        {
            if (_isAnimationEnding)
            {
                RepeatingAnimation.Stop();
                if (!_encounteredError)
                {
                    EndingAnimation.Stop();
                }

                // TODO: Animation that runs when an error is encountered
            } else
            {
                RepeatingAnimation.Begin();
            }
        }

        private void EndingAnimation_Completed(object sender, object e)
        {
            AnimationFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}
