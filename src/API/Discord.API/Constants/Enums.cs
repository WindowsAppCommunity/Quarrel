// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API
{
    public class Constants
    {
        public const JsonNumberHandling ReadWriteAsString = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString;
    }
}
