using System;

namespace SharpVNC.Client.DataAccess
{
    /// <summary>
    /// Used to represent the information and result of a where expression.
    /// </summary>
    public class EqualityPredicate
    {
        /// <summary>
        /// Initialise a new instance of <see cref="EqualityPredicate"/>.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="operation">The operation of the where expression.</param>
        /// <param name="result">The result of the where expression.</param>
        public EqualityPredicate(string tableName, string columnName, EqualityPredicateOperations operation, object result)
        {
            TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
            Operation = operation;
            Result = result;
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// Gets the operation of the where expression.
        /// </summary>
        public EqualityPredicateOperations Operation { get; }

        /// <summary>
        /// Gets the result of the where expression.
        /// </summary>
        public object Result { get; }
    }

    /// <summary>
    /// The different equality predicate operations.
    /// </summary>
    public enum EqualityPredicateOperations
    {
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual
    }
}