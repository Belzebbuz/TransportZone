using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportZone.Air.Domain.BoardingPasses;

namespace TransportZone.Air.Infrastructure.Persistence.Configs;

public class BoardingPassEntityConfig : IEntityTypeConfiguration<BoardingPass>
{
	public void Configure(EntityTypeBuilder<BoardingPass> builder)
	{
		builder.HasKey(x => new { x.FlightId, x.TicketId });
		builder
			.HasIndex(x => new { x.FlightId, x.Number})
			.IsUnique();
	}
}