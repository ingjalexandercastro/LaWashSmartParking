using LaWashSmartParking.Application.DTOs;
using LaWashSmartParking.Application.Interfaces;
using LaWashSmartParking.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LaWashSmartParking.API.Controllers
{
    [ApiController]
    [Route("api/iot-devices")]
    public class IoTDeviceController : ControllerBase
    {
        private readonly IIoTDeviceService _service;

        public IoTDeviceController(IIoTDeviceService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateIoTDeviceDto dto)
        {
            try
            {
                var device = await _service.RegisterAsync(dto.SerialNumber);
                return CreatedAtAction(nameof(GetAll), new { id = device.Id }, device);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var devices = await _service.GetAllAsync();
            return Ok(devices);
        }
    }
}
