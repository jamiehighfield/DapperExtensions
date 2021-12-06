using DapperExtensions.Entities;
using SharpVNC.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SharpVNC.Client.DataAccess
{
    /// <summary>
    /// Extensions for automated repositories.
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Get the first item from the repository matching the where expression. If there isn't any items in the repository matching the where expression, an exception will be thrown.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="repository">The <see cref="IReadRepository{TEntityType}"/>.</param>
        /// <param name="where">An array of where expressions to be applied against matching items from the repository.</param>
        /// <returns>The first item from the repository.</returns>
        public static async Task<TEntityType> SingleAsync<TEntityType>(this IReadRepository<TEntityType> repository, params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (where == null)
            {
                throw new ArgumentNullException(nameof(where));
            }

            return await repository.ExpressionProvider.SingleAsync(where);
        }

        /// <summary>
        /// Get the first item from the repository matching the where expression. If there isn't any items in the repository matching the where expression, null will be returned.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="repository">The <see cref="IReadRepository{TEntityType}"/>.</param>
        /// <param name="where">An array of where expressions to be applied against matching items from the repository.</param>
        /// <returns>The first item from the repository or null if there are no matching items.</returns>
        public static async Task<TEntityType> SingleOrDefaultAsync<TEntityType>(this IReadRepository<TEntityType> repository, params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (where == null)
            {
                throw new ArgumentNullException(nameof(where));
            }

            return await repository.ExpressionProvider.SingleOrDefaultAsync(where);
        }

        /// <summary>
        /// Get the last item from the repository matching the where expression. If there isn't any items in the repository matching the where expression, an exception will be thrown.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="repository">The <see cref="IReadRepository{TEntityType}"/>.</param>
        /// <param name="where">An array of where expressions to be applied against matching items from the repository.</param>
        /// <returns>The first item from the repository.</returns>
        public static async Task<TEntityType> LastAsync<TEntityType>(this IReadRepository<TEntityType> repository, params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (where == null)
            {
                throw new ArgumentNullException(nameof(where));
            }

            return await repository.ExpressionProvider.LastAsync(where);
        }

        /// <summary>
        /// Get the last item from the repository matching the where expression. If there isn't any items in the repository matching the where expression, null will be returned.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="repository">The <see cref="IReadRepository{TEntityType}"/>.</param>
        /// <param name="where">An array of where expressions to be applied against matching items from the repository.</param>
        /// <returns>The first item from the repository or null if there are no matching items.</returns>
        public static async Task<TEntityType> LastOrDefaultAsync<TEntityType>(this IReadRepository<TEntityType> repository, params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (where == null)
            {
                throw new ArgumentNullException(nameof(where));
            }

            return await repository.ExpressionProvider.LastOrDefaultAsync(where);
        }

        /// <summary>
        /// Get all the items from the repository matching the where expression.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="repository">The <see cref="IReadRepository{TEntityType}"/>.</param>
        /// <param name="where">An array of where expressions to be applied against matching items from the repository.</param>
        /// <returns>An collection containing all items that match the where expression.</returns>
        public static async Task<IEnumerable<TEntityType>> ToListAsync<TEntityType>(this IReadRepository<TEntityType> repository, params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            return await repository.ExpressionProvider.ToListAsync(where);
        }

        /// <summary>
        /// Insert an item into the repository.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="repository">The <see cref="IWriteRepository{TEntityType}"/>.</param>
        /// <param name="entity">The entity to insert into the repository.</param>
        /// <returns>The entity after it has been processed by the database provider.</returns>
        public static async Task<TEntityType> AddAsync<TEntityType>(this IWriteRepository<TEntityType> repository, TEntityType entity)
            where TEntityType : class, IEntity
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await repository.ExpressionProvider.AddAsync(entity);
        }

        /// <summary>
        /// Edit all existing items in the repository.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="repository">The <see cref="IWriteRepository{TEntityType}"/>.</param>
        /// <param name="entity">The entity to update in the repository.</param>
        /// <returns>The entities after it has been processed by the database provider.</returns>
        public static async Task<IEnumerable<TEntityType>> EditAsync<TEntityType>(this IWriteRepository<TEntityType> repository, TEntityType entity)
            where TEntityType : class, IEntity
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await repository.ExpressionProvider.EditAsync(entity);
        }

        /// <summary>
        /// Edit an existing item in the repository.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="repository">The <see cref="IWriteRepository{TEntityType}"/>.</param>
        /// <param name="entity">The entity to update in the repository.</param>
        /// <param name="where">An array of where expressions to be applied against matching items from the repository.</param>
        /// <returns>The entity after it has been processed by the database provider.</returns>
        public static async Task<TEntityType> EditSingleAsync<TEntityType>(this IWriteRepository<TEntityType> repository, TEntityType entity, params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await repository.ExpressionProvider.EditSingleAsync(entity, where);
        }
    }
}