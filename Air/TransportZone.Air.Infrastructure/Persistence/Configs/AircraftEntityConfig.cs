using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportZone.Air.Domain.Aircrafts;

namespace TransportZone.Air.Infrastructure.Persistence.Configs;

public class AircraftEntityConfig : IEntityTypeConfiguration<Aircraft>
{
	public void Configure(EntityTypeBuilder<Aircraft> builder)
	{
		builder.Property(x => x.Id).HasMaxLength(3);
	}
}