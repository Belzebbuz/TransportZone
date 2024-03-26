using Microsoft.AspNetCore.Mvc;
using TransportZone.Air.Application.Airports.Features;
using TransportZone.Contracts.Air.Airports;

namespace TransportZone.Air.Api.Controllers;

[Route("api/[controller]")]
public class AirportsController : BaseApiController
{
	[HttpPost]
	public async Task<ActionResult> CreateAsync(AirportCreateRequest request, CancellationToken cancellationToken)
	{
		var result = await Mediatr.Send(new AirportCreate.Command(request), cancellationToken);
		return result.Match(x => Ok(), Problem);
	}
	
	[HttpGet]
	public IAsyncEnumerable<GetAsyncAirportsResponse> GetAsync([FromQuery] string? city, CancellationToken cancellationToken) 
		=> Mediatr.CreateStream(new GetAsyncAirports.Query(city), cancellationToken);
	
}