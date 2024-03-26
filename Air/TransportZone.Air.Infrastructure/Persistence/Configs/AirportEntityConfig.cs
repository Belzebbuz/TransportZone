using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportZone.Air.Domain.Airports;

namespace TransportZone.Air.Infrastructure.Persistence.Configs;

public class AirportEntityConfig : IEntityTypeConfiguration<Airport>
{
	public void Configure(EntityTypeBuilder<Airport> builder)
	{
		builder.Property(x => x.Id).HasMaxLength(3);
		builder.Property(b => b.Coordinates).HasColumnType("geography (point)");
	}
}