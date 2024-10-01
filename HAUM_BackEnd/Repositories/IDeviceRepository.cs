using HAUM_BackEnd.Entities;

namespace HAUM_BackEnd.Repositories
{
    public interface IDeviceRepository
    {
        Task<IEnumerable<DeviceDTO>?> GetUserDevicesAsync(Guid userId);
        Task<IEnumerable<Device>> GetAllDevicesAsync();
        Task<DeviceDTO?> GetUserDeviceByIdAsync(Guid userId, Guid deviceId);
        Task<bool> AddUserDeviceAsync(Guid userId, DeviceDTO deviceDTO);
        Task<bool> UpdateUserDeviceAsync(Guid userId, DeviceDTO deviceDTO);
        Task<bool> DeleteUserDeviceAsync(Guid userId, Guid deviceId);
    }
}
