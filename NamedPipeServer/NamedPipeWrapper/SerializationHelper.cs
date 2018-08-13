using System;
using System.IO;
using Common.Logging;
using Wire;

namespace NamedPipeWrapper
{
    public static class SerializationHelper
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SerializationHelper));

        public static byte[] Serialize(this object obj)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    new Serializer().Serialize(obj, ms);
                    return ms.ToArray();
                }
            }
            catch (Exception e)
            {
                _log.Warn("Failed to serialize the message. It won't be sent.");
            }

            return null;
        }
    }
}