using CentralServer.Domain.Shared;

namespace CentralServer.Infrastructure.Persistence;


/// <summary>
/// Represents an in-memory implementation of a persistence for demonstration purposes.
/// 
/// Intended solely for use in the initial development/demo phase of the project. 
/// Not suitable for production use due to lack of actual persistence, scalability, and real db communication.
/// </summary>
public sealed class InMemoryPersistence<TEntity> : IPersistence<TEntity> where TEntity : IEntity
{
    private readonly List<TEntity> _entities = new();

    public void Save(TEntity entity) => _entities.Add(entity);

    public TEntity? GetOrDefault(Guid id) => _entities.FirstOrDefault(entity => entity.Id == id);

    public IEnumerable<TEntity> GetAll() => _entities;
}