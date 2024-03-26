namespace TransportZone.Contracts.Air.Airports;

public sealed record AirportCreateRequest(string Code,
	string Name,
	string City,
	string Timezone,
	double CoordinateX,
	double CoordinateY,
	string Country);