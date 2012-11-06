using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage.Serialization
{
    public static class ResultContinuationSerializer
    {
        public static string SerializeToken(XmlSerializer serializer, ResultContinuation token)
        {
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
