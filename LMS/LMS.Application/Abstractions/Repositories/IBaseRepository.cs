using System.Linq.Expressions;
using AutoMapper;
using LMS.Domain.Abstractions.Specifications;

namespace LMS.Application.Abstractions.Repositories
{
    /// <summary>
    /// Defines a generic base repository interface for common data access operations.
    /// This interface supports retrieving, adding, updating, and deleting entities,
    /// offering flexible query options via Specifications and LINQ Expressions,
    /// as well as projection capabilities for DTOs.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity this repository handles.</typeparam>
    public interface IBaseRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Retrieves a collection of entities and their total count based on the provided specification.
        /// This method is ideal for complex queries involving filtering, ordering, includes, and pagination.
        /// </summary>
        /// <param name="specification">The specification defining the query criteria, includes, ordering, and pagination.</param>
        /// <returns>A tuple containing a collection of entities and the total count of entities matching the specification without pagination.</returns>
        Task<(ICollection<TEntity> items, int count)> GetAllAsync(ISpecification<TEntity> specification);

        /// <summary>
        /// Retrieves a single entity that matches the provided specification.
        /// This method is suitable for queries that are expected to return zero or one result.
        /// </summary>
        /// <param name="specification">The specification defining the query criteria and includes.</param>
        /// <returns>The entity that matches the specification, or null if no entity is found.</returns>
        Task<TEntity?> GetBySpecificationAsync(ISpecification<TEntity> specification);

        /// <summary>
        /// Retrieves a collection of projected results based on a specialized projection specification.
        /// This is useful for complex projections that might involve grouping or specific shaping beyond simple entity-to-DTO mapping.
        /// </summary>
        /// <typeparam name="TResult">The type of the projected result (DTO).</typeparam>
        /// <typeparam name="TKey">The type of the key used in projection, if applicable (e.g., for grouping).</typeparam>
        /// <param name="projectedSpecification">The projection-specific specification defining how to shape the results.</param>
        /// <returns>A collection of projected results.</returns>
        Task<ICollection<TResult>> GetAllProjectedAsync<TResult, TKey>(IProjectedSpecification<TEntity, TResult, TKey> projectedSpecification);

        /// <summary>
        /// Retrieves a single entity based on a simple LINQ expression.
        /// This method is suitable for straightforward filtering without needing complex includes or ordering.
        /// </summary>
        /// <param name="expression">A LINQ expression to filter the entities.</param>
        /// <returns>The entity that matches the expression, or null if no entity is found.</returns>
        Task<TEntity?> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, bool isTrackingEnable);

        /// <summary>
        /// Retrieves a collection of entities based on a simple LINQ expression.
        /// This method is suitable for straightforward filtering of multiple entities without needing complex includes or ordering.
        /// </summary>
        /// <param name="expression">A LINQ expression to filter the entities.</param>
        /// <returns>A collection of entities that match the expression.</returns>
        Task<ICollection<TEntity>> GetAllByExpressionAsync(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The entity with the specified ID, or null if not found.</returns>
        Task<TEntity?> GetByIdAsync(Guid id);

        /// <summary>
        /// Adds a collection of entities to the repository (e.g., for batch insertion).
        /// Changes are typically persisted when the unit of work is committed.
        /// </summary>
        /// <param name="items">The collection of entities to add.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task AddRangeAsync(ICollection<TEntity> items);

        /// <summary>
        /// Adds a single entity to the repository.
        /// Changes are typically persisted when the unit of work is committed.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity (often with updated ID or default values after persistence).</returns>
        Task<TEntity> AddAsync(TEntity entity);


        /// <summary>
        /// Permanently deletes an entity from the repository by its unique identifier.
        /// Use with caution, as this operation is irreversible. Consider soft-deletes if data retention is important.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task HardDeleteAsync(Guid id);

        /// <summary>
        /// Counts the number of entities that match a given LINQ expression.
        /// This is useful for simple count operations without needing a full specification.
        /// </summary>
        /// <param name="expression">A LINQ expression to filter the entities for counting.</param>
        /// <returns>The total number of entities matching the expression.</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// Projects a list of entities into a list of DTOs (TResult) based on a specification
        /// and an AutoMapper configuration provider. This leverages AutoMapper's ProjectTo
        /// to optimize database queries by selecting only necessary data.
        /// </summary>
        /// <typeparam name="TResult">The type of the DTO to project to.</typeparam>
        /// <param name="spec">The specification defining the filtering and ordering for the entities to be projected.</param>
        /// <param name="configuration">The AutoMapper configuration provider, used to define the mapping from entity to DTO.</param>
        /// <returns>A list of projected DTOs.</returns>
        Task<List<TResult>> ProjectToListAsync<TResult>(ISpecification<TEntity> spec, IConfigurationProvider configuration);

        Task SaveChangeAsunc();



        /// <summary>
        /// Projects a list of entities into a list of DTOs (TResult) based on a specification
        /// and an AutoMapper configuration provider. This leverages AutoMapper's ProjectTo
        /// to optimize database queries by selecting only necessary data.
        /// </summary>
        /// <typeparam name="TResult">The type of the DTO to project to.</typeparam>
        /// <param name="spec">The specification defining the filtering and ordering for the entities to be projected.</param>
        /// <param name="configuration">The AutoMapper configuration provider, used to define the mapping from entity to DTO.</param>
        /// <returns>A list of projected DTOs.</returns>
        Task<TResult?> ProjectToEntityAsync<TResult>(ISpecification<TEntity> spec, IConfigurationProvider configuration);

    }
}