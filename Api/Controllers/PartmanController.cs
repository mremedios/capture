using System.ComponentModel;
using Database.Database;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PartmanController : ControllerBase
{
    private readonly ILogger<HeadersController> _logger;
    private readonly IPartmanRepository _repo;

    public PartmanController(ILogger<HeadersController> logger, IPartmanRepository repository)
    {
        _logger = logger;
        _repo = repository;
    }

    [HttpPatch(), Description("Retention period in days")]
    public async Task SetRetention(int retention)
    {
        await _repo.UpdateRetention(retention);
    }

}