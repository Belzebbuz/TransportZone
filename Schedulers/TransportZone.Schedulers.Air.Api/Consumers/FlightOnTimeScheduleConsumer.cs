using Grpc.Net.Client;
using MassTransit;
using Microsoft.Extensions.Options;
using TransportZone.Contracts.Messaging;
using TransportZone.Schedulers.Air.Api.Options;
using TransportZone.Schedulers.Air.Api.Services;

namespace TransportZone.Schedulers.Air.Api.Consumers;

public class FlightOnTimeScheduleConsumer(
	ILogger<FlightOnTimeScheduleConsumer> logger,
	IOptions<ServicesOptions> options) 
	: IConsumer<FlightOnTimeSchedule>
{
	private readonly ServicesOptions _options = options.Value;
	public async Task Consume(ConsumeContext<FlightOnTimeSchedule> context)
	{
		var id = context.Message.FlightId;
		logger.LogInformation($"Рейс №{id}: начинается обработка сообщения об открытии посадки.");
		using var channel = GrpcChannel.ForAddress(_options.AirGrpcUrl);
		var client = new FlightsService.FlightsServiceClient(channel);
		
		var flight = await client.GetFlightAsync(new (){FlightId = id});
		if (flight is null)
		{
			logger.LogError($"Рейс {id} не найден.");
			throw new ArgumentException(nameof(id));
		}

		if(flight is null)
			throw new ArgumentException(nameof(flight));
		var updateResult = await client.UpdateStatusAsync(new (){FlightId = flight.FlightId, Status = FlightStatus.OnTime});

		if(updateResult is not null && updateResult.Status == CommandStatus.Error)
			throw new InvalidOperationException(updateResult.ErrorReason);
		logger.LogInformation($"Рейс №{id}: открыта посадка.");
	}
}

public class FlightDeliveredScheduleConsumer(
	ILogger<FlightOnTimeScheduleConsumer> logger,
	IOptions<ServicesOptions> options) 
	: IConsumer<FlightOnTimeSchedule>
{
	private readonly ServicesOptions _options = options.Value;
	public async Task Consume(ConsumeContext<FlightOnTimeSchedule> context)
	{
		var id = context.Message.FlightId;
		logger.LogInformation($"Рейс №{id}: начинается обработка сообщения об открытии посадки.");
		using var channel = GrpcChannel.ForAddress(_options.AirGrpcUrl);
		var client = new FlightsService.FlightsServiceClient(channel);
		
		var flight = await client.GetFlightAsync(new ()
		{
			FlightId = id
		});
		if (flight is null)
		{
			logger.LogError($"Рейс {id} не найден.");
			throw new ArgumentException(nameof(id));
		}

		if(flight is null)
			throw new ArgumentException(nameof(flight));
		var updateResult = await client.UpdateStatusAsync(new ()
		{
			FlightId = flight.FlightId, 
			Status = FlightStatus.OnTime
		});

		if(updateResult is not null && updateResult.Status == CommandStatus.Error)
			throw new InvalidOperationException(updateResult.ErrorReason);
		logger.LogInformation($"Рейс №{id}: открыта посадка.");
	}
}