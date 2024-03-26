namespace TransportZone.Air.Domain.Abstractions;

public interface IEntity
{
	public IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
};