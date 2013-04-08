using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage.Account
{
    /// <summary>
    /// /// Class that represents a connection to a Windows Azure table repository based upon an account name.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity, constrained to be of type <see cref="TableEntity"/>.</typeparam>
    public class AccountConfiguration<TEntity> where TEntity : class, ITableEntity, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountConfiguration{TEntity}" /> class.
        /// </summary>
        /// <param name="accountName">Name of the account.</param>
        public AccountConfiguration(string accountName) : this(accountName, typeof(TEntity).Name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountConfiguration{TEntity}" /> class.
        /// </summary>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="tableName">Name of the table.</param>
        public AccountConfiguration(string accountName, string tableName)
        {
            AccountName = accountName;
            TableName = tableName;
        }

        /// <summary>
        /// Gets the name of the account.
        /// </summary>
        /// <value>The name of the account.</value>
        public string AccountName { get; private set; }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; private set; }
    }
}