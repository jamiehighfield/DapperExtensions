using DapperExtensions.Entities;

namespace SharpVNC.Client.Core
{
    /// <summary>
    /// All automated interfaces must implement this interface. Automated interfaces extends the traditional <see cref="IRepository"/> interface, but allows for use of extension methods to read and write to the database.
    /// </summary>
    /// <typeparam name="TEntityType">The type of entity.</typeparam>
    public interface IAutomatedRepository<TEntityType> : IRepository
        where TEntityType : class, IEntity
    {
        /// <summary>
        /// Gets the expression provider as an instance of <see cref="IExpressionProvider"/>.
        /// </summary>
        IExpressionProvider ExpressionProvider { get; }
    }
}