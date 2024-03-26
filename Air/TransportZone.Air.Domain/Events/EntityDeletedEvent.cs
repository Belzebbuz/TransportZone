using TransportZone.Air.Domain.Abstractions;

namespace TransportZone.Air.Domain.Events;

public static class EntityDeletedEvent
{
    public static EntityDeletedEvent<TEntity> WithEntity<TEntity>(TEntity entity)
        where TEntity : IEntity
        => new(entity);
}

public class EntityDeletedEvent<TEntity> : IDomainEvent
    where TEntity : IEntity
{
    internal EntityDeletedEvent(TEntity entity) => Entity = entity;

    public TEntity Entity { get; }
}