using HAUM_BackEnd.Entities;
using HAUM_BackEnd.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HAUM_BackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : Controller
    {
        private readonly IDataRepository _deviceRepository;

        public DataController(IDataRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        [HttpGet]
        [Route("{deviceId:guid}")]
        [ActionName("GetData")]
        public async Task<IActionResult> GetAllDataAsync(Guid deviceId)
        {
            var data = await _deviceRepository.GetAllDataAsync(deviceId);
            if (data != null)
            {
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("{deviceId:guid}/type/{sensorType:int}")]
        [ActionName("GetDataByType")]
        public async Task<IActionResult> GetDeviceAllDataAsync(Guid deviceId, SensorTypeEnum sensorType)
        {
            var data = await _deviceRepository.GetDeviceAllDataAsync(deviceId, sensorType);
            if (data != null)
            {
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("{deviceId:guid}/type/{sensorType:int}/last")]
        [ActionName("GetLastDataByType")]
        public async Task<IActionResult> GetDeviceLastDataAsync(Guid deviceId, SensorTypeEnum sensorType)
        {
            var data = await _deviceRepository.GetDeviceLastDataAsync(deviceId, sensorType);
            if (data != null)
            {
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("{deviceId:guid}/type/{sensorType:int}/date/{date:datetime}")]
        [ActionName("GetDataByTypeAndDate")]
        public async Task<IActionResult> GetDeviceDataByTimeStampAsync(Guid deviceId, SensorTypeEnum sensorType,DateTime date)
        {
            var data = await _deviceRepository.GetDeviceDataByTimeStampAsync(deviceId, sensorType, date);
            if (data != null)
            {
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("{deviceId:guid}/type/{sensorType:int}/date-range/{startDate:datetime}/{endDate:datetime}")]
        [ActionName("GetDataByTypeAndDateRange")]
        public async Task<IActionResult> GetDeviceDataByTimeRangeAsync(Guid deviceId, SensorTypeEnum sensorType, DateTime startDate, DateTime endDate)
        {
            var data = await _deviceRepository.GetDeviceDataByTimeRangeAsync(deviceId, sensorType, startDate, endDate);
            if (data != null)
            {
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
