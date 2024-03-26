using ErrorOr;
using TransportZone.Air.Domain.Abstractions;
using TransportZone.Air.Domain.Common;
using TransportZone.Air.Domain.Events;

namespace TransportZone.Air.Domain.Aircrafts;

public class Aircraft : BaseEntity<string>
{
	public string Model { get; private set; }
	public int Range { get; private set; }
	public IReadOnlyCollection<Seat> Seats =>_seats.AsReadOnly();
	private readonly List<Seat> _seats = new();
	private Aircraft()
	{
	}

	public static ErrorOr<Aircraft> Create(string id, string model, int range)
	{
		var entity = new Aircraft()
		{
			Id = id,
			Model = model,
			Range = range
		};
		var validation = Validate(entity);
		if (validation.IsError)
			return validation.Errors;
		entity.AddEvent(EntityCreatedEvent.WithEntity(entity));
		return entity;
	}
	
	private static ErrorOr<Success> Validate(Aircraft entity)
	{
		var result = new List<Error>();
		if (string.IsNullOrEmpty(entity.Id))
		{
			result.Add(Error.Validation(description: ValidationMessages.Required(nameof(entity.Id))));
		}
		if (string.IsNullOrEmpty(entity.Model))
		{
			result.Add(Error.Validation(description: ValidationMessages.Required(nameof(entity.Model))));
		}
		if (entity.Range <= 0)
		{
			result.Add(Error.Validation(description: ValidationMessages.MoreThan(nameof(entity.Range),0)));
		}
		return result.Any() ? ErrorOr<Success>.From(result) : Result.Success;
	}
}