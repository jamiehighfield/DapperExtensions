using Dapper;
using System;

namespace SharpVNC.Client.DataAccess
{
    /// <summary>
    /// Represents the result of creating a where expression string.
    /// </summary>
    public class ComputedWhere
    {
        /// <summary>
        /// Initialise a new instance of <see cref="ComputedWhere"/>.
        /// </summary>
        /// <param name="where">The where string.</param>
        /// <param name="dynamicParameters">A collection of dynamic parameters.</param>
        public ComputedWhere(string where, DynamicParameters dynamicParameters)
        {
            Where = where ?? throw new ArgumentNullException(nameof(where));
            DynamicParameters = dynamicParameters ?? throw new ArgumentNullException(nameof(dynamicParameters));
        }

        /// <summary>
        /// Gets the where string.
        /// </summary>
        public string Where { get; }

        /// <summary>
        /// Gets a collection of dynamic parameters.
        /// </summary>
        public DynamicParameters DynamicParameters { get; }
    }
}