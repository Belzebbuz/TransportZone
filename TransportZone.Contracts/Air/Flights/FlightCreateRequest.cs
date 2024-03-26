namespace TransportZone.Contracts.Air.Flights;

public sealed record FlightCreateRequest(
	DateTime ScheduledDeparture,
	DateTime ScheduledArrival,
	string DepartureAirportId,
	string ArrivalAirportId,
	string AircraftId);