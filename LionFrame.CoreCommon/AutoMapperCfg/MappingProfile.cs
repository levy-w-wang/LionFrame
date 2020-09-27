using AutoMapper;
using LionFrame.Basic.Extensions;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model.SystemBo;
using System.Linq;

namespace LionFrame.CoreCommon.AutoMapperCfg
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<SysRole, RoleCacheBo>();
            //CreateMap<SysMenu, MenuCacheBo>();
            //CreateMap<SysUser, UserCacheBo>()
            //    .ForMember(c => c.RoleCacheBos,
            //        c => c.MapFrom(d => d.SysUserRoleRelations.Select(u => u.SysRole).DistinctBy(r => r.RoleId)));
        }
    }
}
