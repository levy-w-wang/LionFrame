using System.Threading.Tasks;
using LionFrame.Business;
using LionFrame.CoreCommon.Controllers;
using LionFrame.Model.RequestParam.RoleParams;
using Microsoft.AspNetCore.Mvc;

namespace LionFrame.Controller
{
    [Route("api/[controller]")]
    public class RoleController : BaseUserController
    {
        public RoleBll RoleBll { get; set; }

        /// <summary>
        /// 角色一览
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Route("page"), HttpPost]
        public async Task<ActionResult> RoleList(RolePageParam param)
        {
            var result = await RoleBll.RoleListAsync(param, CurrentUser);
            return Succeed(result);
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Route("add"), HttpPost]
        public async Task<ActionResult> AddRole(IncrementRoleParam param)
        {
            var result = await RoleBll.AddRoleAsync(param, CurrentUser);
            return MyJson(result);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [Route("modify"), HttpPut]
        public async Task<ActionResult> ModifyRole(IncrementRoleParam param)
        {
            var result = await RoleBll.ModifyRoleAsync(param, CurrentUser);
            return MyJson(result);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <returns></returns>
        [HttpDelete, Route("remove/{roleId}")]
        public async Task<ActionResult> RemoveRole([FromRoute] long roleId)
        {
            var result = await RoleBll.RemoveRoleAsync(roleId, CurrentUser);
            return MyJson(result);
        }

        /// <summary>
        /// 获取当前角色能访问的权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet, Route("menu/{roleId}")]
        public async Task<ActionResult> GetRoleMenu([FromRoute] long roleId)
        {
            var result = await RoleBll.GetRoleMenuAsync(roleId, CurrentUser);
            return MyJson(result);
        }
        /// <summary>
        /// 获取角色关联用户
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet, Route("user/{roleId}")]
        public async Task<ActionResult> GetRoleUser([FromRoute] long roleId)
        {
            var result = await RoleBll.GetRoleUserAsync(roleId, CurrentUser);
            return MyJson(result);
        }

        /// <summary>
        /// 修改角色能访问的页面权限
        /// </summary>
        /// <param name="roleMenuParam"></param>
        /// <returns></returns>
        [HttpPost, Route("menu/modify")]
        public async Task<ActionResult> ModifyRoleMenu(ModifyRoleMenuParam roleMenuParam)
        {
            var result = await RoleBll.ModifyRoleMenuAsync(roleMenuParam, CurrentUser);
            return MyJson(result);
        }
    }
}
