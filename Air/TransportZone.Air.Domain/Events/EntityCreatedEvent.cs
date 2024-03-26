using TransportZone.Air.Domain.Abstractions;
using TransportZone.Contracts.Messaging;

namespace TransportZone.Air.Domain.Events;

public static class EntityCreatedEvent
{
	public static EntityCreatedEvent<TEntity> WithEntity<TEntity>(TEntity entity)
		where TEntity : IEntity
		=> new(entity);

}

public class EntityCreatedEvent<TEntity> : IDomainEvent
	where TEntity : IEntity
{
	internal EntityCreatedEvent(TEntity entity) => Entity = entity;
	public TEntity Entity { get; }
}