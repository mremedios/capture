using Capture.Service.Database;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly ILogger<HeadersController> _logger;
    private readonly IHeaderRepository _repo;

    public MessageController(ILogger<HeadersController> logger, IHeaderRepository repository)
    {
        _logger = logger;
        _repo = repository;
    }

    [HttpGet()]
    public IEnumerable<string> Get(string header)
    {
        return _repo.FindByHeader(header);
    }
}