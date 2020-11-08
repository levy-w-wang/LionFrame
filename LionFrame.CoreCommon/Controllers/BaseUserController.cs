using LionFrame.Model.SystemBo;
using LionFrame.CoreCommon.CustomFilter;

namespace LionFrame.CoreCommon.Controllers
{
    /// <summary>
    /// 登录验证控制器
    /// </summary>
    [AuthorizationFilter]
    public abstract class BaseUserController : BaseController
    {
        private UserCacheBo _user;

        /// <summary>
        /// 当前用户
        /// </summary>
        protected UserCacheBo CurrentUser
        {
            get => _user ??= LionUser.CurrentUser;
            set
            {
                LionUser.CurrentUser = value;
                _user = value;
            }
        }

        // 将此逻辑放置在请求管道最前面验证
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    base.OnActionExecuting(filterContext);

        //    if (filterContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        //    {
        //        var isDefined = controllerActionDescriptor.MethodInfo.GetCustomAttributes(true)
        //            .Any(a => a.GetType() == typeof(AllowAnonymousAttribute));
        //        if (isDefined)
        //        {
        //            return;
        //        }
        //    }

        //    var validResult = LionUser.TokenLogin();

        //    switch (validResult)
        //    {
        //        case SysConstants.TokenValidType.LogonInvalid:
        //            filterContext.Result = new CustomHttpStatusCodeResult(200, ResponseCode.Unauthorized, validResult.GetDescription());
        //            return;
        //        case SysConstants.TokenValidType.LoggedInOtherPlaces:
        //            filterContext.Result = new CustomHttpStatusCodeResult(200, ResponseCode.Unauthorized, validResult.GetDescription());
        //            return;
        //        case SysConstants.TokenValidType.Success:
        //            return;
        //        default:
        //            throw new CustomSystemException("未实现的TokenValidType",ResponseCode.DataTypeError);
        //    }
        //}
    }
}
