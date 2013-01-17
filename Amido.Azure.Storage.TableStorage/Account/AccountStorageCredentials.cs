using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage.Account
{
    public class AccountStorageCredentials<TEntity> where TEntity : TableServiceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountConnection{TEntity}" /> class.
        /// </summary>
        /// <param name="accountName">The table storage account name.</param>
        /// <param name="accountKey">The table storage account key</param>
        /// <param name="useHttps">A value to indicate whether to use https.</param>
        public AccountStorageCredentials(string accountName, string accountKey, bool useHttps): this(accountName, accountKey, useHttps, typeof(TEntity).Name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountConnection{TEntity}" /> class.
        /// </summary>
        /// <param name="accountName">The table storage account name.</param>
        /// <param name="accountKey">The table storage account key</param>
        /// <param name="useHttps">A value to indicate whether to use https.</param>
        /// <param name="tableName">Name of the table.</param>
        public AccountStorageCredentials(string accountName, string accountKey, bool useHttps, string tableName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(accountName), "accountName is null or empty.");
            Contract.Requires(!string.IsNullOrWhiteSpace(accountKey), "accountKey is null or empty.");
            AccountName = accountName;
            AccountKey = accountKey;
            UseHttps = useHttps;
            TableName = tableName;
        }

        /// <summary>
        /// Gets the account name.
        /// </summary>
        /// <value>The account name.</value>
        public string AccountName { get; private set; }

        /// <summary>
        /// Gets the account key.
        /// </summary>
        /// <value>The account key.</value>
        public string AccountKey { get; private set; }

        /// <summary>
        /// A value to indicate whether to use https.
        /// </summary>
        /// <value>A value to indicate whether to use https.</value>
        public bool UseHttps { get; private set; }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; private set; }
    }
}
