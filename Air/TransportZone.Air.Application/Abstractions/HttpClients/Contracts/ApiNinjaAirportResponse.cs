namespace TransportZone.Air.Application.Abstractions.HttpClients.Contracts;

public record ApiNinjaAirportResponse(
	string Iata,
	string Name,
	string City,
	string Region,
	string Country,
	string Latitude,
	string Longitude,
	string Timezone);


