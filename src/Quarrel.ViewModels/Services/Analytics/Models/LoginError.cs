// Quarrel © 2022

using System;

namespace Quarrel.Services.Analytics.Models
{
    /// <summary>
    /// A model for an error.
    /// </summary>
    public class LoginError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginError"/> class.
        /// </summary>
        /// <param name="exception">The exception thrown on login.</param>
        public LoginError(Exception exception)
        {
            ExceptionType = exception.GetType();
            ExceptionMessage = exception.Message;
        }

        /// <summary>
        /// The type of exception thrown.
        /// </summary>
        public Type ExceptionType { get; set; }

        /// <summary>
        /// The message of Exception.
        /// </summary>
        public string? ExceptionMessage { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{ExceptionType.Name}: {ExceptionMessage}";
        }
    }
}
