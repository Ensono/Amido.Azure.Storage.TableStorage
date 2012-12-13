using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage.Account
{
    /// <summary>
    /// Class that represents a connection to a Windows Azure table repository based upon a connection string.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity, constrained to be of type <see cref="TableServiceEntity"/>.</typeparam>
    public class AccountConnection<TEntity> where TEntity : TableServiceEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountConnection{TEntity}" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AccountConnection(string connectionString) : this(connectionString, typeof(TEntity).Name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountConnection{TEntity}" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="tableName">Name of the table.</param>
        public AccountConnection(string connectionString, string tableName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(connectionString), "connectionString is null or empty.");
            ConnectionString = connectionString;
            TableName = tableName;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; private set; }
    }
}