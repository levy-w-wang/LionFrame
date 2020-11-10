using System.Collections.Generic;
using LionFrame.Business;
using LionFrame.CoreCommon.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LionFrame.CoreCommon.CustomFilter;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model.RequestParam.SystemParams;
using LionFrame.Model.ResponseDto.ResultModel;

namespace LionFrame.Controller
{
    [Route("api/[controller]")]
    public class MenuController : BaseUserController
    {
        public MenuBll MenuBll { get; set; }

        /// <summary>
        /// 获取当前用户可以访问的菜单树 递归生成菜单结构体
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("menutree")]
        public async Task<ActionResult> GetMenuTree()
        {
            var result = await MenuBll.GetCurrentMenuTreeAsync(CurrentUser);
            return Succeed(result);
        }

        /// <summary>
        /// 获取当前用户可以访问的菜单树 按钮放置于页面下，方便分配权限
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("menulist")]
        public async Task<ActionResult> GetMenuList()
        {
            var result = await MenuBll.GetCurrentMenuListAsync(CurrentUser);
            return Succeed(result);
        }

        #region 系统管理员用

        /// <summary>
        /// 新增菜单或修改
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("menu"), PopedomFilter(true, 1)]
        public async Task<ActionResult> AddMenu(IncrementMenuParam incrementMenu)
        {
            var result = incrementMenu.IsUpdate
                ? await MenuBll.UpdateMenu(CurrentUser, incrementMenu)
                : await MenuBll.AddMenu(CurrentUser, incrementMenu);
            return MyJson(result);
        }

        /// <summary>
        /// 物理删除菜单 按钮
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpDelete, Route("menu/{menuId}"), PopedomFilter(true, 1)]
        public async Task<ActionResult> DeleteMenu([FromRoute] string menuId)
        {
            var result = await MenuBll.DeleteMenuAsync(menuId);
            return MyJson(result);
        }
        /// <summary>
        /// 获取系统的菜单管理
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("manage"), PopedomFilter(true, 1)]
        public async Task<ActionResult> MenuManage()
        {
            var result = await Task.FromResult(MenuBll.GetMenuManage());
            return Succeed(result);
        }

        /// <summary>
        /// 分配菜单 给非系统管理员使用  修改菜单角色关系表
        /// </summary>
        /// <param name="assignMenuParam"></param>
        /// <returns></returns>
        [HttpPost, Route("assign"), PopedomFilter(true, 1)]
        public async Task<ActionResult> AssignMenu(AssignMenuParam assignMenuParam)
        {
            var result = assignMenuParam.Type ? await MenuBll.AssignMenuAsync(assignMenuParam.MenuIdList, CurrentUser.UserId) : await MenuBll.CancelAssignMenuAsync(assignMenuParam.MenuIdList, CurrentUser.UserId);
            return MyJson(result);
        }

        #endregion

    }
}
