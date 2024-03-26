using ErrorOr;
using MediatR;
using TransportZone.Air.Application.Abstractions;
using TransportZone.Contracts.Air.Flights;
using TransportZone.Air.Domain.Flights;

namespace TransportZone.Air.Application.Flights.Features;

public static class FlightCreate
{
	public sealed record Command(FlightCreateRequest Request) : IRequest<ErrorOr<Success>>;

	internal sealed class Handler(IRepository<Flight> repository) : IRequestHandler<Command, ErrorOr<Success>>
	{
		public async Task<ErrorOr<Success>> Handle(Command command, CancellationToken cancellationToken)
		{
			var request = command.Request;
			var entityResult = Flight.Create(request.ScheduledDeparture, request.ScheduledArrival, request.DepartureAirportId,
				request.ArrivalAirportId, request.AircraftId);
			if (entityResult.IsError)
				return entityResult.Errors;
			var saveResult = await repository.CreateAsync(entityResult.Value, cancellationToken);
			if (saveResult.IsError)
				return saveResult.Errors;
			return Result.Success;
		}
	}
}