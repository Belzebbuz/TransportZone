using ErrorOr;
using MediatR;
using TransportZone.Air.Application.Abstractions;
using TransportZone.Contracts.Air.Aircrafts;
using TransportZone.Air.Domain.Aircrafts;

namespace TransportZone.Air.Application.Aircrafts.Features;

public static class AircraftCreate
{
	public sealed record Command(AircraftCreateRequest Request) : IRequest<ErrorOr<Success>>;

	internal sealed class Handler(IRepository<Aircraft> repository) : IRequestHandler<Command, ErrorOr<Success>>
	{
		public async Task<ErrorOr<Success>> Handle(Command command, CancellationToken cancellationToken)
		{
			var entity = Aircraft.Create(command.Request.Code, command.Request.Model, command.Request.Range);
			if (entity.IsError)
				return entity.Errors;
			var saveResult = await repository.CreateAsync(entity.Value, cancellationToken);
			if (saveResult.IsError)
				return entity.Errors;
			return Result.Success;
		}
	}
}