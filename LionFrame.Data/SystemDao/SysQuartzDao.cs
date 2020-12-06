using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LionFrame.Basic.Extensions;
using LionFrame.Data.BasicData;
using LionFrame.Domain.SystemDomain;
using LionFrame.Model.RequestParam.QuartzParams;
using LionFrame.Model.ResponseDto.ResultModel;

namespace LionFrame.Data.SystemDao
{
    public class SysQuartzDao : BaseData
    {
        /// <summary>
        /// 获取任务分页数据
        /// </summary>
        /// <param name="taskListParam"></param>
        /// <returns></returns>
        public async Task<PageResponse<SysQuartz>> GetTaskPageList(TaskListParam taskListParam)
        {
            CloseTracking();
            Expression<Func<SysQuartz, bool>> quartzExpression = quartz => true;
            if (taskListParam.Description.IsNotNullOrEmpty())
            {
                quartzExpression = quartzExpression.And(c => c.Description.Contains(taskListParam.Description));
            }

            if (taskListParam.Group.IsNotNullOrEmpty())
            {
                quartzExpression = quartzExpression.And(c => c.JobGroup.Contains(taskListParam.Group));
            }

            if (taskListParam.Name.IsNotNullOrEmpty())
            {
                quartzExpression = quartzExpression.And(c => c.JobName.Contains(taskListParam.Name));
            }

            if (taskListParam.TriggerState != null)
            {
                quartzExpression = quartzExpression.And(c => c.TriggerState == taskListParam.TriggerState);
            }

            var data = CurrentDbContext.SysQuartzs.Where(quartzExpression);
            var resultEntities = await LoadPageEntitiesAsync(data, taskListParam.CurrentPage, taskListParam.PageSize, true, c => c.TriggerState);
            return resultEntities;
        }
    }
}