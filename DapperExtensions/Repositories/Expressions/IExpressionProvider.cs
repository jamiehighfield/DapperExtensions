using DapperExtensions.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SharpVNC.Client.Core
{
    /// <summary>
    /// Implement this interface to create an expression provider.
    /// </summary>
    public interface IExpressionProvider
    {
        /// <summary>
        /// Get the first item from the repository matching the where expression. If there isn't any items in the repository matching the where expression, an exception will be thrown.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="where">An array of where expressions to be applied against matching items from the repository.</param>
        /// <returns>The first item from the repository.</returns>
        Task<TEntityType> SingleAsync<TEntityType>(params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity;

        /// <summary>
        /// Get the first item from the repository matching the where expression. If there isn't any items in the repository matching the where expression, null will be returned.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="where">An array of where expressions to be applied against matching items from the repository.</param>
        /// <returns>The first item from the repository.</returns>
        Task<TEntityType> SingleOrDefaultAsync<TEntityType>(params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity;

        /// <summary>
        /// Get the last item from the repository matching the where expression. If there isn't any items in the repository matching the where expression, an exception will be thrown.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="where">An array of where expressions to be applied against matching items from the repository.</param>
        /// <returns>The first item from the repository.</returns>
        Task<TEntityType> LastAsync<TEntityType>(params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity;

        /// <summary>
        /// Get the last item from the repository matching the where expression. If there isn't any items in the repository matching the where expression, null will be returned.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="where">An array of where expressions to be applied against matching items from the repository.</param>
        /// <returns>The first item from the repository.</returns>
        Task<TEntityType> LastOrDefaultAsync<TEntityType>(params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity;

        /// <summary>
        /// Get all the items from the repository matching the where expression.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="where">An array of where expressions to be applied against matching items from the repository.</param>
        /// <returns>A collection containing all items that match the where expression.</returns>
        Task<IEnumerable<TEntityType>> ToListAsync<TEntityType>(params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity;

        /// <summary>
        /// Insert an item into the repository.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="entity">The entity to insert into the repository.</param>
        /// <returns>The entity after it has been processed by the database provider.</returns>
        Task<TEntityType> AddAsync<TEntityType>(TEntityType entity)
            where TEntityType : class, IEntity;

        /// <summary>
        /// Edit all existing items in the repository.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="entity">The entity to edit in the repository.</param>
        /// <returns>The entities after it has been processed by the database provider.</returns>
        Task<IEnumerable<TEntityType>> EditAsync<TEntityType>(TEntityType entity)
            where TEntityType : class, IEntity;

        /// <summary>
        /// Edit an existing item in the repository.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="entity">The entity to edit in the repository.</param>
        /// <param name="where">An array of where expressions to be applied against matching items from the repository.</param>
        /// <returns>The entity after it has been processed by the database provider.</returns>
        Task<TEntityType> EditSingleAsync<TEntityType>(TEntityType entity, params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity;
    }
}