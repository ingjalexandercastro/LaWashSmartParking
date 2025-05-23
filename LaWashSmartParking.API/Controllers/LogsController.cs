using LaWashSmartParking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly IParkingUsageLogRepository _logRepo;

    public LogsController(IParkingUsageLogRepository logRepo)
    {
        _logRepo = logRepo;
    }

    // GET: /api/logs
    [HttpGet]
    public async Task<IActionResult> GetAllLogs()
    {
        var logs = await _logRepo.GetAllAsync();
        return Ok(logs);
    }

    // GET: /api/logs/by-spot/{spotId}
    [HttpGet("by-spot/{spotId}")]
    public async Task<IActionResult> GetLogsBySpotId(Guid spotId)
    {
        var logs = await _logRepo.GetBySpotIdAsync(spotId);
        return Ok(logs);
    }
}
