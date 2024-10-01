using AutoMapper;
using HAUM_BackEnd.Entities;
namespace HAUM_BackEnd.Context
{
    public class MapperConfiguration
    {
        public static Mapper InitializeAutomapper()
        {
            var config = new AutoMapper.MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<User, UserDTO>().ReverseMap();
                    cfg.CreateMap<Device, DeviceDTO>().ReverseMap();
                    cfg.CreateMap<Data, DataDTO>().ReverseMap();
                }
            );
             
            var mapper = new Mapper(config);
            return mapper;
        }
    }
}
