using Database.Database;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HeadersController : ControllerBase
{
	private readonly ILogger<HeadersController> _logger;
	private readonly IAvailableHeaderRepository _repo;

	public HeadersController(ILogger<HeadersController> logger, IAvailableHeaderRepository repository)
	{
		_logger = logger;
		_repo = repository;
	}

	[HttpGet()]
	public string[] Get()
	{
		return _repo.FindAll();
	}

	[HttpPost()]
	public Task Post(string[] headers)
	{
		return _repo.InsertAsync(headers);
	}

	[HttpDelete()]
	public void Delete(string[] headers)
	{
		_repo.Delete(headers);
	}
}