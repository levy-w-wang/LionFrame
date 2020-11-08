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
    }
}
