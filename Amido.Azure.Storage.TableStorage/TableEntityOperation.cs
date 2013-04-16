using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage
{
    internal class TableEntityOperation
    {
        public TableEntityOperation(ITableEntity entity, TableOperation operation)
        {
            Entity = entity;
            Operation = operation;
        }

        public ITableEntity Entity { get; set; }
        public TableOperation Operation { get; set; }
    }
}