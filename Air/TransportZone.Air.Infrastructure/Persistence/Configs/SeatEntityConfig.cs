using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportZone.Air.Domain.Aircrafts;

namespace TransportZone.Air.Infrastructure.Persistence.Configs;

public class SeatEntityConfig : IEntityTypeConfiguration<Seat>
{
	public void Configure(EntityTypeBuilder<Seat> builder)
	{
		builder.Property(x => x.Id).HasMaxLength(4);
		builder.HasKey(nameof(Seat.AircraftId), nameof(Seat.Id));
		
	}
}