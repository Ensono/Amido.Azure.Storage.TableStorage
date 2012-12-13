namespace Amido.Azure.Storage.TableStorage
{
    /// <summary>
    /// Interface ITableStorageAdminRepository
    /// </summary>
    public interface ITableStorageAdminRepository
    {
        /// <summary>
        /// Creates the table if it does not exist.
        /// </summary>
        void CreateTableIfNotExists();

        /// <summary>
        /// Deletes a table from Table storage.
        /// </summary>
        void DeleteTable();
    }
}
