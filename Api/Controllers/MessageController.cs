using Api.Graph;
using Api.Graph.Models;
using Database.Database;
using Database.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly ILogger<HeadersController> _logger;
    private readonly IReadonlyRepository _repo;

    public MessageController(ILogger<HeadersController> logger, IReadonlyRepository repository)
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
    public Sequence GraphByDate(string header, string value, string? date)
    {
        ShortData[] messages;

        if (date == null || DateOnly.TryParse(date, out var dateOnly) == false)
        {
            messages = header == "Call-Id" ? _repo.FindByCallId(value) : _repo.FindByHeader(value);
        }
        else
        {
            messages =  header == "Call-Id" ? _repo.FindByCallIdWithDate(value, dateOnly) :_repo.FindByHeaderWithDate(value, dateOnly);
        }

        return new GraphBuilder(messages).Build();
    }
}