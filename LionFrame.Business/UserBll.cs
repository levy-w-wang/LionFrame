using AutoMapper;
using LionFrame.Basic.AutofacDependency;
using LionFrame.Data.SystemDao;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model.RequestParam;
using LionFrame.Model.ResponseDto.ResultModel;

namespace LionFrame.Business
{
    public class UserBll : IScopedDependency
    {
        public SysUserDao SysUserDao { get; set; }
        public Mapper map { get; set; }

        public SysUser Test()
        {
            //SysUserDao.CloseTracking();
            return SysUserDao.First<SysUser>(c => c.UserId == 1);
        }

        public ResponseModel Login(LoginParam loginParam)
        {
            var responseResult = SysUserDao.Login(loginParam);
            return responseResult;
        }
    }
}
