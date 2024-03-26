using Microsoft.AspNetCore.Mvc;
using TransportZone.Air.Application.Flights.Features;
using TransportZone.Contracts.Air.Flights;

namespace TransportZone.Air.Api.Controllers;

[Route("api/[controller]")]
public class FlightsController : BaseApiController
{
	[HttpPost]
	public async Task<ActionResult> CreateAsync(FlightCreateRequest request, CancellationToken cancellationToken)
	{
		var result = await Mediatr.Send(new FlightCreate.Command(request), cancellationToken);
		return result.Match(_ => Ok(), Problem);
	}
}
