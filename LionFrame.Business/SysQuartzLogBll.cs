using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFrame.Basic.AutofacDependency;
using LionFrame.Data.SystemDao;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model.RequestParam.QuartzParams;
using LionFrame.Model.ResponseDto.ResultModel;

namespace LionFrame.Business
{
    public class SysQuartzLogBll : IScopedDependency
    {
        public SysQuartzLogDao SysQuartzLogDao { get; set; }

        public async Task<bool> AddTaskLogAsync(SysQuartzLog sysQuartzLog)
        {
            await SysQuartzLogDao.AddAsync(sysQuartzLog);
            var result = await SysQuartzLogDao.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// 获取任务执行日志分页数据
        /// </summary>
        /// <param name="taskLogListParam"></param>
        /// <returns></returns>
        public async Task<PageResponse<SysQuartzLog>> GetTaskLogPageListAsync(TaskLogListParam taskLogListParam)
        {
            return await SysQuartzLogDao.GetTaskLogPageListAsync(taskLogListParam);
        }
    }
}
