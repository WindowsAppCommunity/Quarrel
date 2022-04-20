// Quarrel © 2022

using System;

namespace Quarrel.Attributes
{
    /// <summary>
    /// An attribute used to give an enum a string value.
    /// </summary>
    public class StringValueAttribute : Attribute
    {
        public StringValueAttribute(string stringValue)
        {
            StringValue = stringValue;
        }

        public string StringValue { get; }
    }
}
