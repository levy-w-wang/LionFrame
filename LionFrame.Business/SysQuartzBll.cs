using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFrame.Basic.AutofacDependency;
using LionFrame.CoreCommon.AutoMapperCfg;
using LionFrame.Data.SystemDao;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model.QuartzModels;

namespace LionFrame.Business
{
    public class SysQuartzBll : IScopedDependency
    {
        public SysQuartzDao SysQuartzDao { get; set; }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> AddTask(ScheduleEntityParam entity)
        {
            var sysQuartz = entity.MapTo<SysQuartz>();
            sysQuartz.CreatedTime = DateTime.Now;
            sysQuartz.LastFireTime = null;
            await SysQuartzDao.AddAsync(sysQuartz);
            var result = await SysQuartzDao.SaveChangesAsync();
            return result > 0;
        }

        //public async Task<bool> ModifyTask(string jobGroup,string jobName)
        //{
        //    SysQuartzDao.CurrentDbContext.SysQuartzs.Where(c=>c.JobGroup == jobGroup && c.JobName == jobName);
        //    var result = await SysQuartzDao.SaveChangesAsync();
        //    return result > 0;
        //}
    }
}
