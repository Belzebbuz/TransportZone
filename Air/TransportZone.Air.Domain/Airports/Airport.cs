using ErrorOr;
using NetTopologySuite.Geometries;
using TransportZone.Air.Domain.Abstractions;
using TransportZone.Air.Domain.Common;
using TransportZone.Air.Domain.Events;

namespace TransportZone.Air.Domain.Airports;

public sealed class Airport : BaseEntity<string>
{
	public string Country { get; init; }
	public string Name { get; init; }
	public string City { get; init; }
	public Point Coordinates { get; init; }
	public string Timezone { get; init; }

	private Airport()
	{
	}

	public static ErrorOr<Airport> Create(string id, string name, string city, Point coordinates, string timezone, string country)
	{
		var entity = new Airport()
		{
			Id = id,
			Name = name,
			City = city,
			Coordinates = coordinates,
			Timezone = timezone,
			Country = country
		};
		var validation = Validate(entity);
		if (validation.IsError) 
			return validation.Errors;

		entity.AddEvent(EntityCreatedEvent.WithEntity(entity));
		return entity;
	}

	private static ErrorOr<Success> Validate(Airport entity)
	{
		var result = new List<Error>();
		if (string.IsNullOrEmpty(entity.Id))
		{
			result.Add(Error.Validation(description: ValidationMessages.Required(nameof(entity.Id))));
		}
		if (string.IsNullOrEmpty(entity.Name))
		{
			result.Add(Error.Validation(description: ValidationMessages.Required(nameof(entity.Name))));
		}
		if (string.IsNullOrEmpty(entity.City))
		{
			result.Add(Error.Validation(description: ValidationMessages.Required(nameof(entity.City))));
		}
		if (string.IsNullOrEmpty(entity.Country))
		{
			result.Add(Error.Validation(description: ValidationMessages.Required(nameof(entity.Country))));
		}
		if (entity.Coordinates is null)
		{
			result.Add(Error.Validation(description: ValidationMessages.Required(nameof(entity.Coordinates))));
		}
		if (string.IsNullOrEmpty(entity.Timezone))
		{
			result.Add(Error.Validation(description: ValidationMessages.Required(nameof(entity.Timezone))));
		}
		if(result.Any())
			return ErrorOr<Success>.From(result);
		return Result.Success;
	}
}