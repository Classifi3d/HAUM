using AutoMapper;
using HAUM_BackEnd.Context;
using HAUM_BackEnd.Entities;
using Microsoft.EntityFrameworkCore;
using MapperConfiguration = HAUM_BackEnd.Context.MapperConfiguration;

namespace HAUM_BackEnd.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private Mapper _mapper;
        
        public DeviceRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _mapper = MapperConfiguration.InitializeAutomapper();
        }

        public async Task<IEnumerable<DeviceDTO>?> GetUserDevicesAsync(Guid userId)
        {
            var user = await _dbContext.User.FindAsync(userId);
            if (user != null) { 
                var devices = await _dbContext.Device.Where(d => d.UserId == userId).ToListAsync();
                if (devices != null) {
                    return _mapper.Map<IEnumerable<DeviceDTO>>(devices);
                }
            }
            return null;
        }

        public async Task<IEnumerable<Device>> GetAllDevicesAsync()
        {
            return await _dbContext.Device.ToListAsync();
            
        }

        public async Task<DeviceDTO?> GetUserDeviceByIdAsync(Guid userId, Guid deviceId)
        {
            var user = await _dbContext.User.FindAsync(userId);
            if (user != null)
            {
                var device = await _dbContext.Device.FindAsync(deviceId);
                if (device != null)
                {
                    return _mapper.Map<DeviceDTO>(device);
                }
            }
            return null;
        }

        public async Task<bool> AddUserDeviceAsync(Guid userId, DeviceDTO deviceDTO)
        {
            var user = await _dbContext.User.FindAsync(userId);
            if (user != null)
            {
                if (deviceDTO != null)
                {
                    var device = _mapper.Map<Device>(deviceDTO);
                    device.Id = Guid.NewGuid();
                    user.Devices!.Add(device);
                    _dbContext.User.Update(user);
                    await _dbContext.Device.AddAsync(device);
                    await _dbContext.SaveChangesAsync();    
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> UpdateUserDeviceAsync(Guid userId, DeviceDTO deviceDTO)
        {
            if (deviceDTO != null)
            {
                var user = await _dbContext.User.FindAsync(userId);
                if (user != null)
                {
                    var device = _dbContext.Device.Where(d => d.Id == deviceDTO.Id && d.UserId == userId).FirstOrDefault();
                    if(device != null)
                    {
                        device = _mapper.Map<Device>(deviceDTO);
                        device.UserId = userId;
                        device.User = user; 
                        _dbContext.Device.Update(device);
                        await _dbContext.SaveChangesAsync();
                        return true;
                    }

                }
            }
            return false;
        }

        public async Task<bool> DeleteUserDeviceAsync(Guid userId, Guid deviceId)
        {
            var user = await _dbContext.User.FindAsync(userId);
            if (user != null)
            {
                var device = await _dbContext.Device.FindAsync(deviceId);
                if (device != null)
                {
                    _dbContext.Device.Remove(device);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }



    }
}
