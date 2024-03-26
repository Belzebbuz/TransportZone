using ErrorOr;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using TransportZone.Air.Application.Abstractions;
using TransportZone.Air.Domain.Flights;

namespace TransportZone.Air.Infrastructure.Services.Grps;

public sealed class GrpsFlightService(
	IRepository<Flight> repository,
	ILogger<GrpsFlightService> logger) : FlightsService.FlightsServiceBase
{
	public override async Task<GetFlightResponse> GetFlight(GetFlightRequest request, ServerCallContext context)
	{
		var flight = await repository.FirstOrDefaultAsync(
			x => x.Id == request.FlightId, 
			flight => new GetFlightResponse()
			{
				FlightId = flight.Id,
				ArrivalAirportId = flight.ArrivalAirportId,
				DepartureAirportId = flight.DepartureAirportId,
				ScheduledDeparture = Timestamp.FromDateTime(flight.ScheduledDeparture)
			},context.CancellationToken);
		
		if (flight is null)
			throw new ArgumentException(nameof(request.FlightId));
		return flight;
	}

	public override async Task<CommandResponse> UpdateStatus(UpdateStatusRequest request, ServerCallContext context)
	{
		logger.LogInformation($"Рейс №{request.FlightId}: Получена команда на обновление статуса.");
		var flight = await repository.FindAsync(request.FlightId);
		if (flight is null)
			return GetError(Error.NotFound(), request.FlightId);
		var oldStatus = flight.Status;
		var result = request.Status switch
		{
			FlightStatus.OnTime => flight.UpdateStatus(Domain.Flights.Enums.FlightStatus.OnTime),
			FlightStatus.Delayed => flight.UpdateStatus(Domain.Flights.Enums.FlightStatus.Delayed),
			_ => throw new ArgumentOutOfRangeException()
		};

		if (result.IsError) 
			return GetError(result, flight.Id);

		var updateResult = await repository.UpdateAsync(flight);
		if (updateResult.IsError)
			return GetError(updateResult, flight.Id);
		
		logger.LogInformation($"Рейс №{request.FlightId}: Статус изменен. {oldStatus} -> {flight.Status}.");
		return new()
		{
			Status = CommandStatus.Success
		};
	}

	private CommandResponse GetError(ErrorOr<Success> result, int flightId)
	{
		logger.LogError($"Рейс №{flightId} Не удалось изменить статус рейса. по причине:\n {result.FirstError.Description}");
		return new()
		{
			Status = CommandStatus.Error,
			ErrorReason = result.FirstError.Description
		};
	}
}