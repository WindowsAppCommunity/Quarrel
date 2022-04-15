// Quarrel © 2022

using OwlCore.Services;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Quarrel.Services.Storage
{
    /// <summary>
    /// A seralizer using <see cref="JsonSerializer"/>.
    /// </summary>
    public class JsonAsyncSerializer : IAsyncSerializer<Stream>
    {
        /// <summary>
        /// A singular instance of the <see cref="JsonAsyncSerializer"/> provided.
        /// </summary>
        public static JsonAsyncSerializer Singleton { get; } = new JsonAsyncSerializer();

        /// <inheritdoc />
        public async Task<Stream> SerializeAsync<T>(T data, CancellationToken? cancellationToken = null)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, data, cancellationToken: cancellationToken ?? CancellationToken.None);
            return stream;
        }

        /// <inheritdoc />
        public async Task<Stream> SerializeAsync(Type inputType, object data, CancellationToken? cancellationToken = null)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, data, inputType, cancellationToken: cancellationToken ?? CancellationToken.None);
            return stream;
        }

        /// <inheritdoc />
        public Task<TResult> DeserializeAsync<TResult>(Stream serialized, CancellationToken? cancellationToken = null)
        {
            return JsonSerializer.DeserializeAsync<TResult>(serialized).AsTask()!;
        }

        /// <inheritdoc />
        public Task<object> DeserializeAsync(Type returnType, Stream serialized, CancellationToken? cancellationToken = null)
        {
            return JsonSerializer.DeserializeAsync(serialized, returnType).AsTask()!;
        }
    }
}
