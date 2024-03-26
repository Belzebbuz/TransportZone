namespace TransportZone.Air.Domain.Abstractions;

public interface IEntity<out T> : IEntity
{
	public T Id { get; }
}