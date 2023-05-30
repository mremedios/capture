using Database.Database;
using Database.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MethodsController : ControllerBase
{
    private readonly ILogger<HeadersController> _logger;
    private readonly IMethodsRepository _repo;

    public MethodsController(ILogger<HeadersController> logger, IMethodsRepository repository)
    {
        _logger = logger;
        _repo = repository;
    }

    [HttpGet()]
    public IEnumerable<string> Get()
    {
        return _repo.FindAll().Select(x => x.ToString());
    }

    [HttpPost()]
    public async Task<int> Post(string[] rawMethods)
    {
        var methods = new List<SipMethods>();
        foreach (var rawMethod in rawMethods)
        {
            if (Enum.TryParse(rawMethod, true, out SipMethods m))
            {
                methods.Add(m);
            }
        }
        await _repo.InsertAsync(methods.ToArray());
        return methods.Count;
    }

    [HttpDelete()]
    public int Delete(string[] rawMethods)
    {
        var methods = new List<SipMethods>();
        foreach (var rawMethod in rawMethods)
        {
            if (Enum.TryParse(rawMethod, true, out SipMethods m))
            {
                methods.Add(m);
            }
        }
        _repo.Delete(methods.ToArray());
        return methods.Count;
    }
}