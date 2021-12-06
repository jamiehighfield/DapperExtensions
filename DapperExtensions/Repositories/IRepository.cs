using SharpVNC.Client.Core.Connection;

namespace SharpVNC.Client.Core
{
    /// <summary>
    /// Implement this interface to create a repository.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Gets an instance of the database connection.
        /// </summary>
        IDatabaseConnection Database { get; }
    }
}