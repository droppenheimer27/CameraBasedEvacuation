using CentralServer.Domain.Shared;

namespace CentralServer.Infrastructure.Persistence;

public interface IPersistence<TEntity> where TEntity : IEntity 
{
    void Save(TEntity entity);
    
    TEntity? GetOrDefault(Guid id);
    
    IEnumerable<TEntity> GetAll();
}