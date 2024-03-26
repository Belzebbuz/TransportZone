using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportZone.Air.Domain.Bookings;

namespace TransportZone.Air.Infrastructure.Persistence.Configs;

public class TicketEntityConfig : IEntityTypeConfiguration<Ticket>
{
	public void Configure(EntityTypeBuilder<Ticket> builder)
	{
		builder.ComplexProperty(x => x.Passenger);
		builder.Property(x => x.ContactData)
			.HasColumnType("jsonb");
	}
}