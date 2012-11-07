using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage.Account
{
    public class AccountConnection<TEntity> where TEntity : TableServiceEntity
    {
        private string tableName;

        public string ConnectionString { get; set; }

        public string TableName
        {
            get
            {
                if (string.IsNullOrEmpty(tableName))
                {
                    return typeof (TEntity).Name;
                }

                return tableName;
            }
            set { tableName = value; }
        }
    }
}