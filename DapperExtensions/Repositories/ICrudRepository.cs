using DapperExtensions.Entities;

namespace SharpVNC.Client.Core
{
    /// <summary>
    /// Implement this interface to create a read and write repository.
    /// </summary>
    /// <typeparam name="TEntityType">The type of entity.</typeparam>
    public interface ICrudRepository<TEntityType> : IReadRepository<TEntityType>, IWriteRepository<TEntityType>, IAutomatedRepository<TEntityType>, IRepository
        where TEntityType : class, IEntity
    { }
}