using System.ComponentModel.DataAnnotations.Schema;

namespace TransportZone.Air.Domain.Abstractions;

public abstract class BaseEntity<T> : IEntity<T>
{
	public T Id { get; protected set; }

	[NotMapped]
	public IReadOnlyCollection<IDomainEvent> DomainEvents => _events;
	private readonly List<IDomainEvent> _events = new();

	protected void AddEvent(IDomainEvent @event)
	{
		_events.Add(@event);
	}
}
public abstract class BaseEntity : IEntity
{
	[NotMapped]
	public IReadOnlyCollection<IDomainEvent> DomainEvents => _events;
	private readonly List<IDomainEvent> _events = new();

	protected void AddEvent(IDomainEvent @event)
	{
		_events.Add(@event);
	}
}