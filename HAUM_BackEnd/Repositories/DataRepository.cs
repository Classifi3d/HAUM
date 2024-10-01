using AutoMapper;
using HAUM_BackEnd.Context;
using HAUM_BackEnd.Entities;
using Microsoft.EntityFrameworkCore;
using MapperConfiguration = HAUM_BackEnd.Context.MapperConfiguration;

namespace HAUM_BackEnd.Repositories
{
    public class DataRepository : IDataRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private Mapper _mapper;

        public DataRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _mapper = MapperConfiguration.InitializeAutomapper();
        }

        public async Task<IEnumerable<DataDTO>?> GetAllDataAsync(Guid deviceId)
        {
            var device = await _dbContext.Device.FindAsync(deviceId);
            if (device != null)
            {
                var datas = await _dbContext.Data.Where(d => d.DeviceId == deviceId).AsNoTracking().ToListAsync();
                if (datas != null)
                {
                    return _mapper.Map<IEnumerable<DataDTO>>(datas);
                }
            }
            return null;
        }

        public async Task<IEnumerable<DataDTO>?> GetDeviceAllDataAsync(Guid deviceId, SensorTypeEnum sensorType)
        {
            var device = await _dbContext.Device.FindAsync(deviceId);
            if (device != null)
            {
                var datas = await _dbContext.Data.Where(d => d.DeviceId == deviceId).AsNoTracking().ToListAsync();
                if (datas != null)
                {
                    var sensorDatas = datas.Where(d => d.Type == sensorType).ToList();
                    return _mapper.Map<IEnumerable<DataDTO>>(sensorDatas);
                }
            }
            return null;
        }

        public async Task<DataDTO?> GetDeviceLastDataAsync(Guid deviceId, SensorTypeEnum sensorType)
        {
            var device = await _dbContext.Device.FindAsync(deviceId);
            if (device != null)
            {
                var datas = await _dbContext.Data.Where(d => d.DeviceId == deviceId).AsNoTracking().ToListAsync();
                if (datas != null)
                {
                    var sensorDatas = datas.Where(d => d.Type == sensorType).OrderByDescending(o => o.Time).FirstOrDefault();
                    return _mapper.Map<DataDTO>(sensorDatas);
                }
            }
            return null;
        }

        public async Task<IEnumerable<DataDTO>> GetDeviceDataByTimeStampAsync(Guid deviceId, SensorTypeEnum sensorType, DateTime date)
        {
            var device = await _dbContext.Device.FindAsync(deviceId);
            if (device != null)
            {
                var datas = await _dbContext.Data.Where(d => d.DeviceId == deviceId).AsNoTracking().ToListAsync();
                if (datas != null)
                {
                    var sensorDatas = datas.Where(d => d.Type == sensorType).ToList();
                    var sensorDataTimeRange = sensorDatas.Where(d => d.Time.Day == date.Day);
                    return _mapper.Map<IEnumerable<DataDTO>>(sensorDataTimeRange);
                }
            }
            return null;
        }

        public async Task<IEnumerable<DataDTO>> GetDeviceDataByTimeRangeAsync(Guid deviceId, SensorTypeEnum sensorType, DateTime startTime, DateTime endTime)
        {
            var device = await _dbContext.Device.FindAsync(deviceId);
            if (device != null)
            {
                var datas = await _dbContext.Data.Where(d => d.DeviceId == deviceId).AsNoTracking().ToListAsync();
                if (datas != null)
                {
                    var sensorDatas = datas.Where(d => d.Type == sensorType).ToList();
                    var sensorDataTimeRange = sensorDatas.Where( d => d.Time > startTime && d.Time < endTime);
                    return _mapper.Map<IEnumerable<DataDTO>>(sensorDataTimeRange);
                }
            }
            return null;
        }
    }
}
