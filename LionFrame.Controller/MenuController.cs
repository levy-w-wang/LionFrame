using LionFrame.Business;
using LionFrame.CoreCommon.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LionFrame.Controller
{
    [Route("api/[controller]")]
    public class MenuController : BaseUserController
    {
        public MenuBll MenuBll { get; set; }

        /// <summary>
        /// 获取可以访问的菜单树
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("menutree")]
        public async Task<ActionResult> GetMenuTree()
        {
            var result = await Task.FromResult(MenuBll.GetCurrentMenuTree(CurrentUser));
            return Succeed(result);
        }

    }
}
