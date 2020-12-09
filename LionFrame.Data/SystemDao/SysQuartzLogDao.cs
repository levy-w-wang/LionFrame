using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LionFrame.Basic.Extensions;
using LionFrame.Data.BasicData;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model.RequestParam.QuartzParams;
using LionFrame.Model.ResponseDto.ResultModel;

namespace LionFrame.Data.SystemDao
{
    public class SysQuartzLogDao : BaseData
    {
        public async Task<PageResponse<SysQuartzLog>> GetTaskLogPageListAsync(TaskLogListParam taskLogListParam)
        {
            CloseTracking();
            Expression<Func<SysQuartzLog, bool>> quartzExpression = quartzLog => true;
            if (taskLogListParam.Description.IsNotNullOrEmpty())
            {
                quartzExpression = quartzExpression.And(c => c.Description.Contains(taskLogListParam.Description));
            }

            if (taskLogListParam.Group.IsNotNullOrEmpty())
            {
                quartzExpression = quartzExpression.And(c => c.JobGroup.Contains(taskLogListParam.Group));
            }

            if (taskLogListParam.Name.IsNotNullOrEmpty())
            {
                quartzExpression = quartzExpression.And(c => c.JobName.Contains(taskLogListParam.Name));
            }

            var data = CurrentDbContext.SysQuartzLogs.Where(quartzExpression);
            var resultEntities = await LoadPageEntitiesAsync(data, taskLogListParam.CurrentPage, taskLogListParam.PageSize, false, c => c.LogId);
            return resultEntities;
        }
    }
}