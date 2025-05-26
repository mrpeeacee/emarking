using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Linq;

namespace Saras.eMarking.Infrastructure
{
    public abstract class BaseRepository<T> where T : class
    {
        public readonly ILogger logger;
        protected BaseRepository(ILogger<T> _logger)
        {
            logger = _logger;
        }

        public string CheckQIGWorkflowStatus(long QigId, ApplicationDbContext context)
        {
            var status = "";
            try
            {
                var Projectworkflowstatustrack = (from workflowstatus in context.WorkflowStatuses
                                                  join workflowstatustrack in context.ProjectWorkflowStatusTrackings on workflowstatus.WorkflowId equals workflowstatustrack.WorkflowStatusId
                                                  where workflowstatustrack.EntityId == QigId && !workflowstatustrack.IsDeleted && workflowstatustrack.EntityType == 2 &&
                                                  (workflowstatus.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Closure) || workflowstatus.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Pause) || workflowstatus.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Standardization_1))
                                                  select new { workflowstatustrack.ProjectWorkflowTrackingId, workflowstatus.WorkflowCode, workflowstatus.WorkflowName, workflowstatustrack.ProcessStatus, workflowstatustrack.Remarks }).ToList();

                var status1 = Projectworkflowstatustrack != null && Projectworkflowstatustrack.OrderByDescending(i => i.ProjectWorkflowTrackingId).Take(1).Any(i => i.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Closure) && i.ProcessStatus == 5);
                var status2 = Projectworkflowstatustrack != null && Projectworkflowstatustrack.OrderByDescending(i => i.ProjectWorkflowTrackingId).Take(1).Any(i => i.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Pause) && i.ProcessStatus == 4);
                var status3 = Projectworkflowstatustrack != null && Projectworkflowstatustrack.Where(i => i.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Standardization_1) && i.ProcessStatus == 3).OrderByDescending(i => i.ProjectWorkflowTrackingId).Take(1).Any();

                if (status1)
                {
                    status = "Closure";
                }
                else if (status2)
                {
                    status = "Paused";
                }
                else if (status3)
                {
                    status = "S1Comp";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in standardisation setup page while checking QIG WorkflowStatus:Method Name: CheckQIGWorkflowStatus()");
                throw;
            }
            return status;
        }
    }
}