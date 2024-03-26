using ErrorOr;
using MediatR;
using NetTopologySuite.Geometries;
using TransportZone.Air.Application.Abstractions;
using TransportZone.Contracts.Air.Airports;
using TransportZone.Air.Domain.Airports;

namespace TransportZone.Air.Application.Airports.Features;

public static class AirportCreate
{
	public sealed record Command(AirportCreateRequest Request) : IRequest<ErrorOr<Success>>;

	internal sealed class Handler(IRepository<Airport> repository) : IRequestHandler<Command, ErrorOr<Success>>
	{
		public async Task<ErrorOr<Success>> Handle(Command command, CancellationToken cancellationToken)
		{
			var point = new Point(new(command.Request.CoordinateX, command.Request.CoordinateY));
			var entity = Airport.Create(command.Request.Code,
				command.Request.Name,
				command.Request.City,
				point,
				command.Request.Timezone,
				command.Request.Country);
			if (entity.IsError)
				return entity.Errors;
			var saveResult = await repository.CreateAsync(entity.Value, cancellationToken);
			if (saveResult.IsError)
				return saveResult.Errors;
			return Result.Success;
		}
	}
}