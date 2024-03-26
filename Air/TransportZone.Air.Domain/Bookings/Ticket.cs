using ErrorOr;
using TransportZone.Air.Domain.Abstractions;
using TransportZone.Air.Domain.Aircrafts;
using TransportZone.Air.Domain.Events;
using TransportZone.Air.Domain.Flights;

namespace TransportZone.Air.Domain.Bookings;

public sealed class Ticket : BaseEntity<int>
{
	public int BookingId { get; private set; }
	public Booking Booking { get; private set; }

	public string SeatId { get; private set; }
	public Seat Seat { get; private set; }

	public Passenger Passenger { get; private set; }
	public ContactData ContactData { get; private set; }
	public decimal Amount { get; private set; }
	
	public IReadOnlyCollection<Flight> Flights => _flights.AsReadOnly();
	private readonly List<Flight> _flights = new();
	private Ticket()
	{
	}

	public static ErrorOr<Ticket> Create(int bookingId,
		string seatId,
		decimal amount,
		Passenger passenger,
		ContactData contactData)
	{
		var entity = new Ticket()
		{
			BookingId = bookingId,
			Passenger = passenger,
			ContactData = contactData,
			Amount = amount,
			SeatId = seatId
		};
		entity.AddEvent(EntityCreatedEvent.WithEntity(entity));
		return entity;
	}
}