using System;

namespace SharpVNC.Client.Core
{
    /// <summary>
    /// Apply this attribute to a property to indicate that the property represents a field in an object-relational mapping.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Initialise a new instance of <see cref="ColumnAttribute"/>.
        /// </summary>
        /// <param name="name">Value indicating the column name.</param>
        public ColumnAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Get a value indicating the column name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value indicating whether this column's value should be added to the database.
        /// </summary>
        public bool Add { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value indicating whether this column's value should be edited in the database.
        /// </summary>
        public bool Edit { get; set; }
    }
}