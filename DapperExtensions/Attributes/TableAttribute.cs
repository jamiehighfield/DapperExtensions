using System;

namespace SharpVNC.Client.Core
{
    /// <summary>
    /// Apply this attribute to a class to indicate that the class represents a table in an object-relational mapping.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TableAttribute : Attribute
    {
        /// <summary>
        /// Initialise a new instance of <see cref="TableAttribute"/>
        /// </summary>
        /// <param name="name">A value indicating the table name.</param>
        public TableAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Get a value indicating the table name.
        /// </summary>
        public string Name { get; }
    }
}