// Credit to Sergio Pedri for extensions

using JetBrains.Annotations;

namespace System.Threading.Tasks
{
    /// <summary>
    /// A <see langword="delegate"/> used to handle an <see cref="AggregateException"/> thrown by a <see cref="Task"/>
    /// </summary>
    /// <param name="e">The generated <see cref="AggregateException"/></param>
    public delegate void AggregateExceptionHandler([NotNull] AggregateException e);
}
