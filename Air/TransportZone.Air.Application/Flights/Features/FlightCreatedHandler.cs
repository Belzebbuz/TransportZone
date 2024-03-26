using MediatR;
using Microsoft.Extensions.Logging;
using TransportZone.Air.Application.Abstractions;
using TransportZone.Contracts.Messaging;
using TransportZone.Air.Domain.Events;
using TransportZone.Air.Domain.Flights;

namespace TransportZone.Air.Application.Flights.Features;

public class FlightCreatedHandler(IProducer producer, ILogger<FlightCreatedHandler> logger) 
	: INotificationHandler<EntityCreatedEvent<Flight>>,
	  INotificationHandler<EntityUpdatedEvent<Flight>>
{
	public async Task Handle(EntityCreatedEvent<Flight> notification, CancellationToken cancellationToken)
	{
		var flight = notification.Entity;
		var result = await producer.ProduceAsync(new FlightCreatedMessage(flight.Id), cancellationToken);
		if(result.IsError)
			logger.LogError($"Произошла ошибка при отправке сообщения о создании рейса №{flight.Id}");
		else
			logger.LogInformation($"Информация о созданном рейсе {flight.Id} отправлена в очередь.");
	}

	public Task Handle(EntityUpdatedEvent<Flight> notification, CancellationToken cancellationToken)
		=> Task.CompletedTask;
}