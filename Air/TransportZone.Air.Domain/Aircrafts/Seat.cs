using ErrorOr;
using TransportZone.Air.Domain.Abstractions;
using TransportZone.Air.Domain.Aircrafts.Enums;
using TransportZone.Air.Domain.Common;
using TransportZone.Air.Domain.Events;

namespace TransportZone.Air.Domain.Aircrafts;

public sealed class Seat : BaseEntity<string>
{
	public string AircraftId { get; private set; }
	public Aircraft Aircraft { get; private set; }
	public FareCondition FareCondition { get; private set; }

	private Seat()
	{
	}

	public static ErrorOr<Seat> Create(string aircraftId, FareCondition fareCondition)
	{
		if(string.IsNullOrEmpty(aircraftId))
			return Error.Validation(description: ValidationMessages.Required(nameof(aircraftId)));
		var entity = new Seat()
		{
			AircraftId = aircraftId,
			FareCondition = fareCondition
		};
		entity.AddEvent(EntityCreatedEvent.WithEntity(entity));
		return entity;
	}
}