// Quarrel © 2022

using System;

namespace Quarrel.Attributes
{
    /// <summary>
    /// An attribute used to give an enum a string value.
    /// </summary>
    public class StringValueAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringValueAttribute"/> class.
        /// </summary>
        public StringValueAttribute(string stringValue)
        {
            StringValue = stringValue;
        }

        /// <summary>
        /// Gets the string value for the enum.
        /// </summary>
        public string StringValue { get; }
    }
}
