using System.Linq.Expressions;
using MediatR;
using TransportZone.Air.Application.Abstractions;
using TransportZone.Contracts.Air.Airports;
using TransportZone.Air.Domain.Airports;

namespace TransportZone.Air.Application.Airports.Features;

public static class GetAsyncAirports
{
	public sealed record Query(string? City) : IStreamRequest<GetAsyncAirportsResponse>;

	public class Handler(IRepository<Airport> repository) : IStreamRequestHandler<Query,GetAsyncAirportsResponse>
	{
		public IAsyncEnumerable<GetAsyncAirportsResponse> Handle(Query request, CancellationToken cancellationToken)
		{
			Expression<Func<Airport, GetAsyncAirportsResponse>> selector = airport => new GetAsyncAirportsResponse(
				airport.City,
				airport.Country,
				airport.Coordinates.Coordinate.X,
				airport.Coordinates.Coordinate.Y, airport.Timezone);
			
			if(string.IsNullOrEmpty(request.City))
				return repository.GetAsync(selector);
			
			return repository.GetAsync(x => x.City.ToLower().Contains(request.City.ToLower()),selector);
		}
	}
}