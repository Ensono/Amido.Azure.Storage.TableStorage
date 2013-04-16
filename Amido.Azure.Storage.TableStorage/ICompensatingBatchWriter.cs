using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage
{
    public interface ICompensatingBatchWriter<in TEntity>
        where TEntity : class, ITableEntity, new()
    {
        /// <summary>
        /// Insert entity into batch for execution.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        void Insert(TEntity entity);

        /// <summary>
        /// Insert entities into batch for execution.
        /// </summary>
        /// <param name="entities">Entities to insert.</param>
        void Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// Execute batch of operations by partition in order.
        /// </summary>
        /// <exception cref="BatchFailedException">Raised if the batch fails for any reason with IsConsisted property set to false if part of the batch has been committed.</exception>
        void Execute();
    }
}