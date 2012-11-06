namespace Amido.Azure.Storage.TableStorage
{
    public interface ITableStorageAdminRepository
    {
        void CreateTableIfNotExists();

        void DeleteTable();
    }
}
