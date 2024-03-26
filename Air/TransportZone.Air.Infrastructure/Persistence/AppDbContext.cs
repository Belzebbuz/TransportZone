using MediatR;
using Microsoft.EntityFrameworkCore;
using TransportZone.Air.Domain.Abstractions;
using TransportZone.Air.Domain.Aircrafts;
using TransportZone.Air.Domain.Airports;
using TransportZone.Air.Domain.Bookings;
using TransportZone.Air.Domain.Flights;

namespace TransportZone.Air.Infrastructure.Persistence;

internal sealed class AppDbContext : DbContext
{
	private readonly IPublisher _publisher;

	public AppDbContext(DbContextOptions<AppDbContext> options, IPublisher publisher)
		: base(options)
	{
		_publisher = publisher;
	}

	public DbSet<Airport> Airports => Set<Airport>();
	public DbSet<Aircraft> Aircrafts => Set<Aircraft>();
	public DbSet<Seat> Seats => Set<Seat>();
	public DbSet<Flight> Flights => Set<Flight>();
	public DbSet<Booking> Bookings => Set<Booking>();
	public DbSet<Ticket> Tickets => Set<Ticket>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
		base.OnModelCreating(modelBuilder);
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		var domainEvents = ChangeTracker.Entries<IEntity>()
			.SelectMany(entry => entry.Entity.DomainEvents);
		var result = await base.SaveChangesAsync(cancellationToken);
		await PublishDomainEventsAsync(domainEvents);
		return result;
	}

	private async Task PublishDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents)
	{
		var tasks = domainEvents
			.Select(x => _publisher.Publish(x));
		await Task.WhenAll(tasks);
	}
}