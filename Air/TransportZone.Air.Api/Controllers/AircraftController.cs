using Microsoft.AspNetCore.Mvc;
using TransportZone.Air.Application.Aircrafts.Features;
using TransportZone.Contracts.Air.Aircrafts;

namespace TransportZone.Air.Api.Controllers;

[Route("api/[controller]")]
public class AircraftController : BaseApiController
{
	[HttpPost]
	public async Task<ActionResult> CreateAsync(AircraftCreateRequest request, CancellationToken cancellationToken)
	{
		var result = await Mediatr.Send(new AircraftCreate.Command(request), cancellationToken);
		return result.Match(_ => Ok(), Problem);
	}
}