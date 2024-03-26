using ErrorOr;
using TransportZone.Air.Application.Abstractions.HttpClients.Contracts;

namespace TransportZone.Air.Application.Abstractions.HttpClients;

public interface IApiNinjaClient
{
	public Task<ErrorOr<IEnumerable<ApiNinjaAirportResponse>>> GetAirportsAsync(string country);
}