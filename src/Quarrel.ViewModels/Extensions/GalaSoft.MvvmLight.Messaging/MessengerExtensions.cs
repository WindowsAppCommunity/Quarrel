// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using JetBrains.Annotations;
using Quarrel.ViewModels.Messages.Abstract;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GalaSoft.MvvmLight.Messaging
{
    /// <summary>
    /// An class with some extension methods for the <see cref="IMessenger"/> type.
    /// </summary>
    public static class MessengerExtensions
    {
        /// <summary>
        /// Requests a result of a given type, using a specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to send.</typeparam>
        /// <typeparam name="TResult">The returned type.</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use.</param>
        /// <returns>The result of the <typeparamref name="TMessage"/> message.</returns>
        [Pure]
        [CanBeNull]
        public static TResult Request<TMessage, TResult>([NotNull] this IMessenger messenger)
            where TMessage : RequestMessageBase<TResult>, new()
        {
            TMessage message = new TMessage();
            messenger.Send(message);
            if (!message.ResponseReceived)
            {
                throw new InvalidOperationException("No response was received for the message");
            }

            return message.Result;
        }

        /// <summary>
        /// Requests a result of a given type, using a specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to send.</typeparam>
        /// <typeparam name="TResult">The returned type.</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use.</param>
        /// <param name="message">The request message to send.</param>
        /// <returns>The result of the <paramref name="message"/>.</returns>
        [Pure]
        [CanBeNull]
        public static TResult Request<TMessage, TResult>([NotNull] this IMessenger messenger, TMessage message)
            where TMessage : RequestMessageBase<TResult>
        {
            messenger.Send(message);
            if (!message.ResponseReceived)
            {
                throw new InvalidOperationException("No response was received for the message");
            }

            return message.Result;
        }

        /// <summary>
        /// Requests a result of a given type, for the specified request message.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to send.</typeparam>
        /// <typeparam name="TRequest">The the type of the request info.</typeparam>
        /// <typeparam name="TResult">The returned type.</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use.</param>
        /// <param name="message">The actual message to send.</param>
        /// <param name="token">The optional cancellation token to receive the response.</param>
        /// <returns>The result of the <paramref name="message"/>.</returns>
        [Pure]
        [ItemCanBeNull]
        public static Task<TResult> RequestAsync<TMessage, TRequest, TResult>([NotNull] this IMessenger messenger, TMessage message, CancellationToken token = default)
            where TMessage : AsyncRequestMessageBase<TRequest, TResult>
        {
            messenger.Send(message);
            return token == default ? message.Task : message.Task.WithToken(token);
        }

        /// <summary>
        /// Waits for a given message to be broadcast, and returns it.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to wait for.</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use.</param>
        /// <param name="recipient">The recipient that will receive the message.</param>
        /// <param name="token">The optional cancellation token to receive the response.</param>
        /// <returns>The recieved message.</returns>
        [ItemCanBeNull]
        public static async Task<TMessage> WaitAsync<TMessage>([NotNull] this IMessenger messenger, [NotNull] object recipient, CancellationToken token = default)
            where TMessage : class
        {
            TaskCompletionSource<TMessage> tcs = new TaskCompletionSource<TMessage>();
            messenger.Register<TMessage>(recipient, m => tcs.TrySetResult(m));
            TMessage result = await tcs.Task.WithToken(token);
            messenger.Unregister<TMessage>(recipient);
            return result;
        }
    }
}
