using ErrorOr;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TransportZone.Contracts.Messaging;
using TransportZone.Air.Application.Abstractions;

namespace TransportZone.Air.Infrastructure.Services.Messaging;

public class KafkaProducer(IServiceProvider provider, ILogger<KafkaProducer> logger) : IProducer
{
	public async Task<ErrorOr<Success>> ProduceAsync<T>(T message, CancellationToken cancellationToken) 
		where T : class, IMessage
	{
		try
		{
			var producer = provider.GetRequiredService<ITopicProducer<T>>();
			await producer.Produce(message, cancellationToken);
			return Result.Success;
		}
		catch (Exception e)
		{
			logger.LogError(e.ToString());
			return Error.Failure(description: e.Message);
		}
		
	}
}