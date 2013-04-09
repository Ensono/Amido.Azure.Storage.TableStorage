using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage.Serialization
{
    public class ContinuationTokenSerializer : IContinuationTokenSerializer
    {
        public TableContinuationToken DeserializeToken(string token) 
        {
            TableContinuationToken continuationToken;
            
            var doc = XDocument.Parse(token,LoadOptions.None);
            using (var reader = doc.CreateReader())
            {
                continuationToken = DeserializeToken(reader);
            }
            
            return continuationToken;
        }
        
        public TableContinuationToken DeserializeToken(XmlReader reader)
        {
            var continuationToken = new TableContinuationToken();
            continuationToken.ReadXml(reader);
            return continuationToken;
        }

        public void WriteXml(XmlWriter writer, TableContinuationToken continuationToken) 
        {
            continuationToken.WriteXml(writer);
        }

        public string SerializeToken(TableContinuationToken continuationToken) 
        {

            using (var stringWriter = new StringWriter())
            {
                using (var writer = new XmlTextWriter(stringWriter))
                {
                    WriteXml(writer, continuationToken);
                }

                return stringWriter.ToString();
            }
        }
    }
}