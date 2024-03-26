using ErrorOr;
using TransportZone.Air.Domain.Abstractions;
using TransportZone.Air.Domain.Bookings;
using TransportZone.Air.Domain.Events;
using TransportZone.Air.Domain.Flights;

namespace TransportZone.Air.Domain.BoardingPasses;

public sealed class BoardingPass: BaseEntity
{
	public int TicketId { get; private set; }
	public Ticket Ticket { get; private set; }
	public int FlightId { get; private set; }
	public Flight Flight { get; private set; }
	public int Number { get; private set; }

	private BoardingPass()
	{
	}

	public static ErrorOr<BoardingPass> Create(int ticketId, int flightId, int number)
	{
		var entity = new BoardingPass()
		{
			FlightId = flightId,
			TicketId = ticketId,
			Number = number
		};
		entity.AddEvent(EntityCreatedEvent.WithEntity(entity));
		return entity;
	}
}