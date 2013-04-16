using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage
{
    internal class TableEntityOperation
    {
        public TableEntityOperation(int operationNumber, ITableEntity entity, TableOperation operation)
        {
            OperationNumber = operationNumber;
            Entity = entity;
            Operation = operation;
        }

        public int OperationNumber { get; private set; }
        public ITableEntity Entity { get; private set; }
        public TableOperation Operation { get; private set; }
    }
}