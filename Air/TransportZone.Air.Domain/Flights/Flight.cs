using ErrorOr;
using TransportZone.Air.Domain.Abstractions;
using TransportZone.Air.Domain.Aircrafts;
using TransportZone.Air.Domain.Airports;
using TransportZone.Air.Domain.Bookings;
using TransportZone.Air.Domain.Events;
using TransportZone.Air.Domain.Flights.Enums;
using TransportZone.Contracts.Messaging;

namespace TransportZone.Air.Domain.Flights;

public sealed class Flight : BaseEntity<int>
{
	public DateTime ScheduledDeparture { get; private set; }
	public DateTime ScheduledArrival  { get; private set; }
	public DateTime? ActualDeparture { get; private set; }
	public DateTime? ActualArrival  { get; private set; }
	public string DepartureAirportId  { get; private set; }
	public Airport DepartureAirport { get; private set; }
	public string ArrivalAirportId  { get; private set; }
	public Airport ArrivalAirport { get; private set; }
	public FlightStatus Status { get; private set; }
	public string AircraftId { get; private set; }
	public Aircraft Aircraft { get; private set; }

	public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();
	private readonly List<Ticket> _tickets = new();
	private Flight()
	{
	}

	public static ErrorOr<Flight> Create(
		DateTime scheduledDeparture,
		DateTime scheduledArrival, 
		string departureAirportId,
		string arrivalAirportId,
		string aircraftId
	)
	{
		var entity = new Flight()
		{
			ScheduledDeparture = scheduledDeparture,
			ScheduledArrival = scheduledArrival,
			DepartureAirportId = departureAirportId,
			ArrivalAirportId = arrivalAirportId,
			AircraftId = aircraftId,
			Status = FlightStatus.Scheduled
		};
		entity.AddEvent(EntityCreatedEvent.WithEntity(entity));
		return entity;
	}

	public ErrorOr<Success> UpdateStatus(FlightStatus status)
	{
		Status = status;
		AddEvent(EntityUpdatedEvent.WithEntity(this));
		return Result.Success;
	}
}