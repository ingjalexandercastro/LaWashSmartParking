using LaWashSmartParking.Application.DTOs;
using LaWashSmartParking.Application.Interfaces;
using LaWashSmartParking.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LaWashSmartParking.API.Controllers
{
   [ApiController]
    [Route("api/[controller]")]
    public class ParkingSpotController : ControllerBase
    {
        private readonly IParkingSpotService _spotService;
        private readonly IIoTDeviceService _deviceService;

        public ParkingSpotController(IIoTDeviceService deviceService, IParkingSpotService spotService)
        {
            _spotService = spotService;
            _deviceService = deviceService;
        }

        // GET: /api/parking-spot
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var spots = await _spotService.GetAllSpotsAsync();
            return Ok(spots);
        }

        // POST: /api/parking-spot
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateParkingSpotDto dto)
        {
            try
            {
                var device = await _deviceService.GetBySerialNumberAsync(dto.SerialNumber);

                if (device == null)
                    return BadRequest(new { message = "Error." });

                await _spotService.AddParkingSpotAsync(dto.Location, device.Id);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: /api/parking-spot/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _spotService.RemoveSpotAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: /api/parking-spot/{serialNumber}/occupy
        [HttpPost("{serialNumber}/occupy")]
        public async Task<IActionResult> OccupyBySerial(string serialNumber)
        {
            try
            {
                await _spotService.OccupySpotAsync(serialNumber);
                return Ok(new { message = "Spot occupied." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: /api/parking-spot/{serialNumber}/free
        [HttpPost("{serialNumber}/free")]
        public async Task<IActionResult> FreeBySerial(string serialNumber)
        {
            try
            {
                await _spotService.FreeSpotAsync(serialNumber);
                return Ok(new { message = "Spot freed." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("status-count")]
        public async Task<IActionResult> GetParkingStatusCount()
        {
            var (free, occupied) = await _spotService.GetParkingSpotStatusCountAsync();

            return Ok(new ParkingSpotStatusCountDto
            {
                Free = free,
                Occupied = occupied
            });
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if (page < 1 || size < 1)
                return BadRequest(new { message = "Page and size must be greater than zero." });

            var result = await _spotService.GetPagedAsync(page, size);
            return Ok(result);
        }
    }
}
