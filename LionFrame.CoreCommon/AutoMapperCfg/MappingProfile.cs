using AutoMapper;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model.RequestParam.SystemParams;
using LionFrame.Model.ResponseDto.SystemDto;
using LionFrame.Model.SystemBo;

namespace LionFrame.CoreCommon.AutoMapperCfg
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region role

            CreateMap<RoleCacheBo, RoleDto>();

            #endregion
            
            #region user

            CreateMap<UserCacheBo, UserDto>().ForMember(c => c.RoleDtos, c => c.MapFrom(d => d.RoleCacheBos));

            #endregion

            #region menu

            CreateMap<MenuCacheBo, MenuPermsDto>();
            CreateMap<IncrementMenuParam, SysMenu>();
            CreateMap<SysMenu, MenuManageDto>();

            #endregion
        }
    }
}
