using AutoMapper;
using LionFrame.Model.ResponseDto.SystemDto;
using LionFrame.Model.SystemBo;

namespace LionFrame.CoreCommon.AutoMapperCfg
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RoleCacheBo, RoleDto>();
            CreateMap<UserCacheBo, UserDto>().ForMember(c => c.RoleDtos, c => c.MapFrom(d => d.RoleCacheBos));
            CreateMap<MenuCacheBo, MenuDto>();
        }
    }
}
