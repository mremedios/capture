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
    private readonly ICallsRepository _repo;

    public MessageController(ILogger<HeadersController> logger, ICallsRepository repository)
    {
        _logger = logger;
        _repo = repository;
    }

    [HttpGet()]
    public IEnumerable<ShortData> MessagesByHeader(string header)
    {
        return _repo.FindByHeader(header);
    }

    // [HttpGet(), Route("graph")]
    // public Sequence GraphByInterval(string header, string value, string from, string to)
    // {
    //     var messages = _repo.FindByHeader(value);
    //     return new GraphBuilder(messages).Build();
    // }
    
    [HttpGet(), Route("graph")]
    public Sequence GraphByDate(string header, string value, string? date)
    {
        ShortData[] messages;
        if (date == null || DateOnly.TryParse(date, out var dateOnly) == false)
        {
            messages = _repo.FindByHeader(value);
        }
        else
        {
            
            messages = _repo.FindByHeaderAndDate(value, dateOnly);
        }
        
        return new GraphBuilder(messages).Build();
    }
}