using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage
{
    public interface IBatchWriter<TEntity>
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
        /// Delete entity into batch for execution.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Delete entities into batch for execution.
        /// </summary>
        /// <param name="entities">Entities to delete.</param>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// Insert or merge entity into batch for execution.
        /// </summary>
        /// <param name="entity">Entity to insert or merge.</param>
        void InsertOrMerge(TEntity entity);

        /// <summary>
        /// Insert or merge entities into batch for execution.
        /// </summary>
        /// <param name="entities">Entities to insert or merge.</param>
        void InsertOrMerge(IEnumerable<TEntity> entities);
        
        /// <summary>
        /// Insert or replace entity into batch for execution.
        /// </summary>
        /// <param name="entity">Entity to insert or replace.</param>
        void InsertOrReplace(TEntity entity);

        /// <summary>
        /// Insert or replace entities into batch for execution.
        /// </summary>
        /// <param name="entities">Entities to insert or replace.</param>
        void InsertOrReplace(IEnumerable<TEntity> entities);
        
        /// <summary>
        /// Merge entity into batch for execution.
        /// </summary>
        /// <param name="entity">Entity to merge.</param>
        void Merge(TEntity entity);

        /// <summary>
        /// Merge entities into batch for execution.
        /// </summary>
        /// <param name="entities">Entities to merge.</param>
        void Merge(IEnumerable<TEntity> entities);
        
        /// <summary>
        /// Replace entity into batch for execution.
        /// </summary>
        /// <param name="entity">Entity to replace.</param>
        void Replace(TEntity entity);

        /// <summary>
        /// Replace entities into batch for execution.
        /// </summary>
        /// <param name="entities">Entities to replace.</param>
        void Replace(IEnumerable<TEntity> entities);
        
        /// <summary>
        /// Execute batch of operations by partition in order.
        /// </summary>
        /// <exception cref="BatchFailedException">Raised if the batch fails for any reason with IsConsisted property set to true as no compensation attempted.</exception>
        void Execute();
    }
}