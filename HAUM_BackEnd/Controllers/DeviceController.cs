using HAUM_BackEnd.Entities;
using HAUM_BackEnd.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace HAUM_BackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceController : Controller
    {
        private readonly IDeviceRepository _deviceRepository;

        public DeviceController(IDeviceRepository deviceRepository) { 
            _deviceRepository = deviceRepository;
        }

        [HttpGet]
        [Route("{userId:guid}")]
        [ActionName("GetDevices")]
        public async Task<IActionResult> GetUserDevicesAsync(Guid userId)
        {
            var devices = await _deviceRepository.GetUserDevicesAsync(userId);
            if(devices != null) { 
                return Ok(devices);
            }
            else 
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("{userId:guid}/{deviceId:guid}")]
        [ActionName("GetDeviceById")]
        public async Task<IActionResult> GetUserDeviceByIdAsync(Guid userId, Guid deviceId)
        {
             var device = await _deviceRepository.GetUserDeviceByIdAsync(userId, deviceId);
            if(device != null)
            {
                return Ok(device);
            }
            else 
            { 
                return NotFound();
            }
        }

        [HttpPost]
        [Route("{userId:guid}")]
        public async Task<IActionResult> AddUserDeviceAsync(Guid userId,[FromBody] DeviceDTO deviceDTO)
        {
            var isAdded = await _deviceRepository.AddUserDeviceAsync(userId,deviceDTO);
            if(isAdded) {
                return Ok();
            }
            else
            { 
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("{userId:guid}")]
        public async Task<IActionResult> UpdateUserDeviceAsync(Guid userId,[FromBody] DeviceDTO deviceDTO)
        {
            var isUpdated = await _deviceRepository.UpdateUserDeviceAsync(userId, deviceDTO);
            if(isUpdated)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("{userId:guid}/{deviceId:guid}")]
        [ActionName("DeleteDeviceById")]
        public async Task<IActionResult> DeleteUserDeviceAsync(Guid userId, Guid deviceId)
        {
            var isDeleted = await _deviceRepository.DeleteUserDeviceAsync(userId, deviceId);
            if(isDeleted)
            {
                return Ok();
            }
            else
            { 
                return NotFound();
            }
        }
    }
}
