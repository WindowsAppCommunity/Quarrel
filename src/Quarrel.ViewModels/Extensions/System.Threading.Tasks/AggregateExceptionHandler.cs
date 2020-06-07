// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using JetBrains.Annotations;

namespace System.Threading.Tasks
{
    /// <summary>
    /// A <see langword="delegate"/> used to handle an <see cref="AggregateException"/> thrown by a <see cref="Task"/>.
    /// </summary>
    /// <param name="e">The generated <see cref="AggregateException"/>.</param>
    public delegate void AggregateExceptionHandler([NotNull] AggregateException e);
}
