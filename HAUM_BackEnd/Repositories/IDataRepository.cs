using HAUM_BackEnd.Entities;

namespace HAUM_BackEnd.Repositories
{
    public interface IDataRepository
    {
        Task<IEnumerable<DataDTO>> GetAllDataAsync(Guid deviceId);
        Task<IEnumerable<DataDTO>> GetDeviceAllDataAsync(Guid deviceId,SensorTypeEnum sensorType);
        Task<DataDTO?> GetDeviceLastDataAsync(Guid deviceId, SensorTypeEnum sensorType);
        Task<IEnumerable<DataDTO>> GetDeviceDataByTimeStampAsync(Guid deviceId, SensorTypeEnum sensorType, DateTime dateTime);
        Task<IEnumerable<DataDTO>> GetDeviceDataByTimeRangeAsync(Guid deviceId, SensorTypeEnum sensorType, DateTime startTime, DateTime endTime);
    }
}
