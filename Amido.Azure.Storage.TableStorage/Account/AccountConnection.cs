using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage.Account
{
    public class AccountConnection<TEntity> where TEntity : TableServiceEntity
    {
        public AccountConnection(string connectionString) : this(connectionString, typeof(TEntity).Name)
        {
        }

        public AccountConnection(string connectionString, string tableName)
        {
            ConnectionString = connectionString;
            TableName = tableName;
        }

        public string ConnectionString { get; private set; }

        public string TableName { get; private set; }
    }
}