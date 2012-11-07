using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage.Account
{
    public class AccountConfiguration<TEntity> where TEntity : TableServiceEntity
    {
        public AccountConfiguration(string accountName) : this(accountName, typeof(TEntity).Name)
        {
        }

        public AccountConfiguration(string accountName, string tableName)
        {
            AccountName = accountName;
            TableName = tableName;
        }

        public string AccountName { get; private set; }

        public string TableName { get; private set; }
    }
}