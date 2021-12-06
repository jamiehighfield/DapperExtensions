using Dapper;
using DapperExtensions.Entities;
using SharpVNC.Client.Core;
using SharpVNC.Client.Core.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SharpVNC.Client.DataAccess
{
    /// <summary>
    /// Default implementation of <see cref="IExpressionProvider"/> for SQLite.
    /// </summary>
    public class DefaultSQLiteExpressionProvider : IExpressionProvider
    {
        /// <summary>
        /// Initialise a new instance of <see cref="DefaultSQLiteExpressionProvider"/>.
        /// </summary>
        /// <param name="database">An instance of <see cref="IDatabaseConnection"/>.</param>
        /// <param name="repositoryNamingProvider">An instance of the repository naming provider.</param>
        public DefaultSQLiteExpressionProvider(IDatabaseConnection database, RepositoryNamingProvider repositoryNamingProvider)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
            RepositoryNamingProvider = repositoryNamingProvider ?? throw new ArgumentNullException(nameof(repositoryNamingProvider));
        }

        /// <summary>
        /// Gets an instance of <see cref="IDatabaseConnection"/>.
        /// </summary>
        private IDatabaseConnection Database { get; }

        /// <summary>
        /// Gets an instance of the repository naming provider.
        /// </summary>
        public RepositoryNamingProvider RepositoryNamingProvider { get; }

        /// <summary>
        /// Run a query against the database, exporting the results to an instance of <see cref="IListResult{TObjectType}"/>.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="queryStringLeft">The SQL query string to run, left of the where expression.</param>
        /// <param name="queryStringRight">The SQL query string to run, right of the where expression.</param>
        /// <param name="limit">The limit of results. 0 indicates no limit.</param>
        /// <param name="where">An array of where expressions to be applied against matching items from the repository.</param>
        /// <returns>An instance of <see cref="IEnumerable{TObjectType}"/>.</returns>
        private async Task<IEnumerable<TEntityType>> Query<TEntityType>(string queryStringLeft, string queryStringRight, int limit, params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity
        {
            if (queryStringLeft is null)
            {
                throw new ArgumentNullException(nameof(queryStringLeft));
            }

            if (queryStringRight is null)
            {
                throw new ArgumentNullException(nameof(queryStringRight));
            }

            ComputedWhere whereExpression = ComputeWhere(where);

            if (queryStringLeft.EndsWith(";"))
            {
                queryStringLeft = queryStringLeft.Substring(0, queryStringLeft.LastIndexOf(";"));
            }

            if (queryStringRight.EndsWith(";"))
            {
                queryStringRight = queryStringRight.Substring(0, queryStringRight.LastIndexOf(";"));
            }

            string computedQueryString = default;

            if (limit > 0)
            {
                computedQueryString = queryStringLeft + " WHERE " + whereExpression.Where + " " + queryStringRight + " LIMIT " + limit + ";";
            }
            else
            {
                computedQueryString = queryStringLeft + " WHERE " + whereExpression.Where + " " + queryStringRight + ";";
            }

            IEnumerable<TEntityType> result = await Database.QueryAsync<TEntityType>(computedQueryString, whereExpression.DynamicParameters);

            return result;
        }

        /// <summary>
        /// Builds an instance of <see cref="ComputedWhere"/> from one or more where expressions as instances of <see cref="Expression{TDelegate}"/>.
        /// </summary>
        /// <typeparam name="TEntityType">The entity type.</typeparam>
        /// <param name="where">An array of where expressions.</param>
        /// <returns>An instance of <see cref="ComputedWhere"/>.</returns>
        private ComputedWhere ComputeWhere<TEntityType>(params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity
        {
            if (where is null)
            {
                throw new ArgumentNullException(nameof(where));
            }

            DynamicParameters dynamicParameters = new DynamicParameters();

            List<EqualityPredicate> predicates = new List<EqualityPredicate>();

            foreach (Expression<Func<TEntityType, bool>> expression in where)
            {
                if (expression.Body is BinaryExpression binaryExpression)
                {
                    if (binaryExpression.Type != typeof(bool))
                    {
                        continue;
                    }

                    TableIdentifier table = RepositoryNamingProvider.GetTable<TEntityType>();

                    if (table is null)
                    {
                        continue;
                    }

                    string propertyName = ((MemberExpression)binaryExpression.Left).Member.Name;

                    ColumnIdentifier column = table.GetColumnByPropertyName(propertyName);

                    if (column is null)
                    {
                        continue;
                    }

                    object result = Expression.Lambda(binaryExpression.Right).Compile().DynamicInvoke();

                    if (binaryExpression.NodeType == ExpressionType.Equal)
                    {
                        predicates.Add(new EqualityPredicate(table.Name, column.Name, EqualityPredicateOperations.Equal, result));
                    }
                    else if (binaryExpression.NodeType == ExpressionType.NotEqual)
                    {
                        predicates.Add(new EqualityPredicate(table.Name, column.Name, EqualityPredicateOperations.Equal, result));
                    }
                    else if (binaryExpression.NodeType == ExpressionType.LessThan)
                    {
                        predicates.Add(new EqualityPredicate(table.Name, column.Name, EqualityPredicateOperations.Equal, result));
                    }
                    else if (binaryExpression.NodeType == ExpressionType.LessThanOrEqual)
                    {
                        predicates.Add(new EqualityPredicate(table.Name, column.Name, EqualityPredicateOperations.Equal, result));
                    }
                    else if (binaryExpression.NodeType == ExpressionType.GreaterThan)
                    {
                        predicates.Add(new EqualityPredicate(table.Name, column.Name, EqualityPredicateOperations.Equal, result));
                    }
                    else if (binaryExpression.NodeType == ExpressionType.GreaterThanOrEqual)
                    {
                        predicates.Add(new EqualityPredicate(table.Name, column.Name, EqualityPredicateOperations.Equal, result));
                    }
                }
            }

            if (predicates.Count == 0)
            {
                return new ComputedWhere(" (TRUE = TRUE)", dynamicParameters);
            }

            string computedWhere = "";

            int count = 0;

            foreach (EqualityPredicate predicate in predicates)
            {
                if (predicate.Operation == EqualityPredicateOperations.Equal)
                {
                    computedWhere += (string.IsNullOrEmpty(computedWhere) ? string.Empty : " AND") + " (" + predicate.TableName + "." + predicate.ColumnName + " = " + ":whereExp" + Convert.ToString(count) + ")";
                }
                else if (predicate.Operation == EqualityPredicateOperations.NotEqual)
                {
                    computedWhere += (string.IsNullOrEmpty(computedWhere) ? string.Empty : " AND") + " (" + predicate.TableName + "." + predicate.ColumnName + " <> " + ":whereExp" + Convert.ToString(count) + ")";
                }
                else if (predicate.Operation == EqualityPredicateOperations.LessThan)
                {
                    computedWhere += (string.IsNullOrEmpty(computedWhere) ? string.Empty : " AND") + " (" + predicate.TableName + "." + predicate.ColumnName + " < " + ":whereExp" + Convert.ToString(count) + ")";
                }
                else if (predicate.Operation == EqualityPredicateOperations.LessThanOrEqual)
                {
                    computedWhere += (string.IsNullOrEmpty(computedWhere) ? string.Empty : " AND") + " (" + predicate.TableName + "." + predicate.ColumnName + " <= " + ":whereExp" + Convert.ToString(count) + ")";
                }
                else if (predicate.Operation == EqualityPredicateOperations.GreaterThan)
                {
                    computedWhere += (string.IsNullOrEmpty(computedWhere) ? string.Empty : " AND") + " (" + predicate.TableName + "." + predicate.ColumnName + " > " + ":whereExp" + Convert.ToString(count) + ")";
                }
                else if (predicate.Operation == EqualityPredicateOperations.GreaterThanOrEqual)
                {
                    computedWhere += (string.IsNullOrEmpty(computedWhere) ? string.Empty : " AND") + " (" + predicate.TableName + "." + predicate.ColumnName + " >= " + ":whereExp" + Convert.ToString(count) + ")";
                }

                dynamicParameters.Add("@whereExp" + Convert.ToString(count), predicate.Result);

                count += 1;
            }

            return new ComputedWhere(computedWhere, dynamicParameters);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TEntityType"><inheritdoc/></typeparam>
        /// <param name="entity"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public async Task<TEntityType> AddAsync<TEntityType>(TEntityType entity)
            where TEntityType : class, IEntity
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            TableIdentifier table = RepositoryNamingProvider.GetTable<TEntityType>();

            string columns = string.Empty;
            string values = string.Empty;

            if (table.GetColumns().Count() == 0)
            {
                return entity;
            }

            foreach (ColumnIdentifier column in table.GetColumns())
            {
                if (column.Add)
                {
                    if (string.IsNullOrEmpty(columns))
                    {
                        columns = column.Name;
                    }
                    else
                    {
                        columns += "," + column.Name;
                    }

                    if (string.IsNullOrEmpty(values))
                    {
                        values = ":" + column.Property.Name;
                    }
                    else
                    {
                        values += "," + ":" + column.Property.Name;
                    }
                }
            }

            long id = await Database.QuerySingleAsync<long>(@"INSERT INTO " + table.Name + " (" + columns + ") VALUES (" + values + "); SELECT last_insert_rowid();", entity);

            return await SingleAsync<TEntityType>((existingEntity) => existingEntity.Id == id);
        }

        public async Task<IEnumerable<TEntityType>> EditAsync<TEntityType>(TEntityType entity)
            where TEntityType : class, IEntity
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            TableIdentifier table = RepositoryNamingProvider.GetTable<TEntityType>();

            string query = string.Empty;

            if (table.GetColumns().Count() == 0)
            {
                return new List<TEntityType> { }.AsReadOnly();
            }

            foreach (ColumnIdentifier column in table.GetColumns())
            {
                if (column.Edit)
                {
                    if (string.IsNullOrEmpty(query))
                    {
                        query = column.Name + "=" + ":" + column.Property.Name;
                    }
                    else
                    {
                        query += "," + column.Name + "=" + ":" + column.Property.Name;
                    }
                }
            }

            DynamicParameters dynamicParameters = new DynamicParameters(entity);

            await Database.ExecuteAsync(@"UPDATE " + table.Name + " SET " + query + ";", dynamicParameters);

            return null;
        }

        public async Task<TEntityType> EditSingleAsync<TEntityType>(TEntityType entity, params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity
        {
            if (where is null)
            {
                throw new ArgumentNullException(nameof(where));
            }

            TableIdentifier table = RepositoryNamingProvider.GetTable<TEntityType>();

            string query = string.Empty;

            if (table.GetColumns().Count() == 0)
            {
                return entity;
            }

            foreach (ColumnIdentifier column in table.GetColumns())
            {
                if (column.Edit)
                {
                    if (string.IsNullOrEmpty(query))
                    {
                        query = column.Name + "=" + ":" + column.Property.Name;
                    }
                    else
                    {
                        query += "," + column.Name + "=" + ":" + column.Property.Name;
                    }
                }
            }
            
            ComputedWhere whereExpression = ComputeWhere(where);

            whereExpression.DynamicParameters.AddDynamicParams(entity);

            await Database.ExecuteAsync(@"UPDATE " + table.Name + " SET " + query + " WHERE" + whereExpression.Where + ";", whereExpression.DynamicParameters);

            return null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TEntityType"><inheritdoc/></typeparam>
        /// <param name="where"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public async Task<TEntityType> LastAsync<TEntityType>(params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity
        {
            if (where is null)
            {
                throw new ArgumentNullException(nameof(where));
            }

            IEnumerable<TEntityType> result = await Query("SELECT * FROM " + RepositoryNamingProvider.GetTable<TEntityType>().Name, "ORDER BY id DESC;", 1, where);

            return result.First();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TEntityType"><inheritdoc/></typeparam>
        /// <param name="where"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public async Task<TEntityType> LastOrDefaultAsync<TEntityType>(params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity
        {
            if (where is null)
            {
                throw new ArgumentNullException(nameof(where));
            }

            IEnumerable<TEntityType> result = await Query("SELECT * FROM " + RepositoryNamingProvider.GetTable<TEntityType>().Name, "ORDER BY id DESC;", 1, where);

            return result.FirstOrDefault();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TEntityType"><inheritdoc/></typeparam>
        /// <param name="where"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public async Task<TEntityType> SingleAsync<TEntityType>(params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity
        {
            if (where is null)
            {
                throw new ArgumentNullException(nameof(where));
            }

            IEnumerable<TEntityType> result = await Query("SELECT * FROM " + RepositoryNamingProvider.GetTable<TEntityType>().Name + ";", string.Empty, 1, where);

            return result.Single();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TEntityType"><inheritdoc/></typeparam>
        /// <param name="where"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public async Task<TEntityType> SingleOrDefaultAsync<TEntityType>(params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity
        {
            if (where is null)
            {
                throw new ArgumentNullException(nameof(where));
            }

            IEnumerable<TEntityType> result = await Query("SELECT * FROM " + RepositoryNamingProvider.GetTable<TEntityType>().Name + ";", string.Empty, 1, where);

            return result.SingleOrDefault();
        }

        public async Task<IEnumerable<TEntityType>> ToListAsync<TEntityType>(params Expression<Func<TEntityType, bool>>[] where)
            where TEntityType : class, IEntity
        {
            if (where is null)
            {
                throw new ArgumentNullException(nameof(where));
            }

            IEnumerable<TEntityType> result = await Query("SELECT * FROM " + RepositoryNamingProvider.GetTable<TEntityType>().Name, string.Empty, 0, where);

            return result;
        }
    }
}