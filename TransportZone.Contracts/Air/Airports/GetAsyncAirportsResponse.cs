namespace TransportZone.Contracts.Air.Airports;

public record GetAsyncAirportsResponse(string City, string Country, double Longitude, double Latitude, string Timezone);