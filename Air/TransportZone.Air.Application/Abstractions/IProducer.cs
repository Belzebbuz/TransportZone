using ErrorOr;
using TransportZone.Contracts.Messaging;

namespace TransportZone.Air.Application.Abstractions;

public interface IProducer
{
	public Task<ErrorOr<Success>> ProduceAsync<T>(T message, CancellationToken cancellationToken) where T : class, IMessage;
}