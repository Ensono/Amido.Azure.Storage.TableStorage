using System.Xml;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage.Serialization
{
    public interface IContinuationTokenSerializer
    {
        TableContinuationToken DeserializeToken(string token);
        TableContinuationToken DeserializeToken(XmlReader reader);
        void WriteXml(XmlWriter writer, TableContinuationToken continuationToken);
        string SerializeToken(TableContinuationToken continuationToken);
    }
}