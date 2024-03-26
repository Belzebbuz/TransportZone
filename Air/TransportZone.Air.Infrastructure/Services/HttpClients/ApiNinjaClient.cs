using System.Net.Http.Json;
using ErrorOr;
using Microsoft.Extensions.Logging;
using TransportZone.Air.Application.Abstractions.HttpClients;
using TransportZone.Air.Application.Abstractions.HttpClients.Contracts;

namespace TransportZone.Air.Infrastructure.Services.HttpClients;

public class ApiNinjaClient(HttpClient client, ILogger<ApiNinjaClient> logger) : IApiNinjaClient
{
	public async Task<ErrorOr<IEnumerable<ApiNinjaAirportResponse>>> GetAirportsAsync(string country)
	{
		var result = new List<ApiNinjaAirportResponse>();
		var resultEmpty = false;
		var page = 1;
		try
		{
			while (!resultEmpty)
			{
				var response = await client.GetAsync($"/v1/airports?country={country}&offset={page * 30}");
				if (!response.IsSuccessStatusCode)
				{
					var responseError = await response.Content.ReadAsStringAsync();
					var error = $"Ошибка при выполнении запроса. {responseError}";
					logger.LogError(error);
					return Error.Unexpected(responseError);
				}
				var pageResult = await response.Content.ReadFromJsonAsync<ICollection<ApiNinjaAirportResponse>>();
				if (pageResult is null)
				{
					var error = $"Ошибка при выполнении запроса. Не удалось обработать значение.";
					logger.LogError(error);
					resultEmpty = true;
				}
				else
				{
					resultEmpty = !pageResult.Any();
					result.AddRange(pageResult);
					page++;
				}
			}

			return result;
		}
		catch (Exception e)
		{
			logger.LogError(e.Message);
			return Error.Unexpected(description:e.Message);
		}
	}
}