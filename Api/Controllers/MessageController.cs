using Api.Graph;
using Api.Graph.Models;
using Capture.Service.Database;
using Capture.Service.Models;
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
    public IEnumerable<ShortData> MessagesByHeader(string header)
    {
        return _repo.FindByHeader(header);
    }

    [HttpGet(), Route("graph")]
    public Sequence GraphByHeader(string header)
    {
        var messages = _repo.FindByHeader(header);
        return new GraphBuilder(messages).Build();
    }
}