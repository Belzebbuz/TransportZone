using System.Security.Cryptography;
using ErrorOr;
using TransportZone.Air.Domain.Abstractions;
using TransportZone.Air.Domain.Bookings.Enums;
using TransportZone.Air.Domain.Common;
using TransportZone.Air.Domain.Events;

namespace TransportZone.Air.Domain.Bookings;

public sealed class Booking : BaseEntity<int>
{
	public DateTime BookDate { get; private set; }
	public BookingState BookingState { get; private set; }
	public decimal TotalAmount { get; private set; }
	public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();
	private readonly List<Ticket> _tickets = new();
	private Booking()
	{
	}

	public static ErrorOr<Booking> Create(decimal totalAmount)
	{
		var entity = new Booking()
		{
			BookDate = DateTime.UtcNow,
			TotalAmount = totalAmount,
			BookingState = BookingState.Created
		};
		entity.AddEvent(EntityCreatedEvent.WithEntity(entity));
		return entity;
	}

	public ErrorOr<Success> Confirm()
	{
		if(BookingState != BookingState.Created)
			return Error.Validation(description: "Невозможно подтвердить бронь, бронь уже утверждена");
		if (!_tickets.Any())
			return Error.Validation(description: "Невозможно подтвердить бронь, не добавлен ни один билет");
		BookingState = BookingState.Created;
		AddEvent(EntityUpdatedEvent.WithEntity(this));
		return Result.Success;
	}
	
	public ErrorOr<Success> Payed()
	{
		if(BookingState != BookingState.Confirmed)
			return Error.Validation(description: "Невозможно подтвердить бронь, бронь еще не подтверждена");
		BookingState = BookingState.Payed;
		AddEvent(EntityUpdatedEvent.WithEntity(this));
		return Result.Success;
	}
	
	public ErrorOr<Success> Cancel()
	{
		BookingState = BookingState.Canceled;
		AddEvent(EntityUpdatedEvent.WithEntity(this));
		return Result.Success;
	}
	
	public ErrorOr<Success> AddTickets(IEnumerable<Ticket> tickets)
	{
		var errors = new List<Error>();
		foreach (var ticket in tickets)
		{
			if (_tickets.Any(x => x.Id == ticket.Id))
			{
				errors.Add(Error.Validation(description: ValidationMessages.AlreadyExist(nameof(Ticket), ticket.Id.ToString())));
				continue;
			}
			_tickets.Add(ticket);
		}
		if(errors.Any())
			return ErrorOr<Success>.From(errors);
		return Result.Success;
	}
}