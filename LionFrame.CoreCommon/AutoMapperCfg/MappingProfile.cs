using AutoMapper;
using LionFrame.Domain;
using LionFrame.Model;

namespace LionFrame.CoreCommon.AutoMapperCfg
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ForMember(c => c.Address, c => c.MapFrom(u => u.Address.Province + u.Address.City + u.Address.Area + u.Address.Detail));
        }
    }
}
