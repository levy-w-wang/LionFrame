using System.Collections.Generic;
using LionFrame.Business;
using LionFrame.CoreCommon.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LionFrame.CoreCommon.CustomFilter;
using LionFrame.Model.RequestParam.SystemParams;

namespace LionFrame.Controller
{
    [Route("api/[controller]")]
    public class MenuController : BaseUserController
    {
        public MenuBll MenuBll { get; set; }

        /// <summary>
        /// 获取当前用户可以访问的菜单树
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("menutree")]
        public async Task<ActionResult> GetMenuTree()
        {
            var result = await Task.FromResult(MenuBll.GetCurrentMenuTree(CurrentUser));
            return Succeed(result);
        }

        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("menu"), PopedomFilter(true, 1)]
        public async Task<ActionResult> AddMenu(IncrementMenuParam incrementMenu)
        {
            var result = await MenuBll.AddMenu(CurrentUser, incrementMenu);
            return MyJson(result);
        }
    }
}
