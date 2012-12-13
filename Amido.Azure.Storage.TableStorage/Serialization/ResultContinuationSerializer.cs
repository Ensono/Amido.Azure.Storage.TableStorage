using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage.Serialization
{
    /// <summary>
    /// Class for serializing and deserializing a Continuation to and from a <see cref="ResultContinuation"/>.
    /// </summary>
    public static class ResultContinuationSerializer
    {
        /// <summary>
        /// Serializes the token.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="token">The token.</param>
        /// <returns>System.String.</returns>
        public static string SerializeToken(XmlSerializer serializer, ResultContinuation token)
        {
            if (token == null)
            {
                return null;
            }

            using (var writer = new StringWriter())
            {
                var writerSettings = new XmlWriterSettings { OmitXmlDeclaration = true, NewLineChars = String.Empty };

                using (var xmlWriter = XmlWriter.Create(writer, writerSettings))
                {
                    serializer.Serialize(xmlWriter, token);
                }

                return writer.ToString();
            }
        }

        /// <summary>
        /// Deserializes the token.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="token">The token.</param>
        /// <returns>ResultContinuation.</returns>
        public static ResultContinuation DeserializeToken(XmlSerializer serializer, string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                using (var stringReader = new StringReader(token))
                {
                    return (ResultContinuation)serializer.Deserialize(stringReader);
                }
            }

            return null;
        }
    }
}
