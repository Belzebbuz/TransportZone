using Grpc.Net.Client;
using MassTransit;
using Microsoft.Extensions.Options;
using TransportZone.Contracts.Messaging;
using TransportZone.Schedulers.Air.Api.Options;
using TransportZone.Schedulers.Air.Api.Services;

namespace TransportZone.Schedulers.Air.Api.Consumers;

public class FlightCreatedConsumer(
	ILogger<FlightCreatedConsumer> logger, 
	IMessageScheduler scheduler,
	IOptions<ServicesOptions> options) : IConsumer<FlightCreatedMessage>
{
	private readonly ServicesOptions _options = options.Value;
	public async Task Consume(ConsumeContext<FlightCreatedMessage> context)
	{
		logger.LogInformation($"Получено сообщение о создании рейса №{context.Message.Id}");
		using var channel = GrpcChannel.ForAddress(_options.AirGrpcUrl);
		var client = new FlightsService.FlightsServiceClient(channel);
		var flightEvent = context.Message;
		var flight = await client.GetFlightAsync(new (){FlightId = flightEvent.Id});
		if (flight is null)
		{
			logger.LogError($"Рейс {flightEvent.Id} не найден.");
			throw new ArgumentException(nameof(context.Message.Id));
		}

		var departureTime = flight.ScheduledDeparture.ToDateTime();
		var departureTimeLeft = departureTime - DateTime.UtcNow;
		switch (departureTimeLeft.TotalMinutes)
		{
			case > 15:
			{
				var timeSchedule = departureTime.AddMinutes(-15);
				await scheduler.SchedulePublish<FlightOnTimeSchedule>(timeSchedule, new(flight.FlightId));
				logger.LogInformation($"Рейс №{flight.FlightId} || {flight.DepartureAirportId} -> {flight.ArrivalAirportId} || посадка открывается в {timeSchedule}.");
				return;
			}
			case <= 15 and >= 0:
				await client.UpdateStatusAsync(new (){FlightId = flight.FlightId, Status = FlightStatus.OnTime});
				logger.LogInformation($"Рейс №{flight.FlightId} открыта посадка.");
				return;
			case < 0:
				await client.UpdateStatusAsync(new (){FlightId = flight.FlightId, Status = FlightStatus.Delayed});
				logger.LogInformation($"Рейс №{flight.FlightId} задерживается.");
				break;
		}
	}
}