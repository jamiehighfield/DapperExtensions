using DapperExtensions.Entities;

namespace SharpVNC.Client.Core
{
    /// <summary>
    /// Implement this interface to create a read repository.
    /// </summary>
    /// <typeparam name="TEntityType">The type of entity.</typeparam>
    public interface IReadRepository<TEntityType> : IAutomatedRepository<TEntityType>, IRepository
        where TEntityType : class, IEntity
    { }
}