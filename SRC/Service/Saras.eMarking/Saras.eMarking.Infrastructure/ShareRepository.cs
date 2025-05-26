using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Saras.eMarking.Infrastructure
{
    public class ShareRepository
    {
        public readonly ILogger logger;
        public ShareRepository(ILogger _logger)
        {
            logger = _logger;
        }
        /// <summary>
        /// MoveUncategorisedScripttoLiveMarking : This Method is to move uncategorised script to Live Marking
        /// </summary>
        /// <param name="qigId">qig Id</param>
        /// <param name="ProjectUserRoleID">ProjectUser RoleID</param>
        /// <param name="projectId">project Id</param>
        public void MoveUncategorisedScripttoLiveMarking(long qigId, long ProjectUserRoleID, long projectId, IAppCache AppCache, ApplicationDbContext context)
        {
            try
            {
                int categorizationWfId = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script);

                int LiveMarkingWfId = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.LiveMarking, EnumWorkflowType.Script);

                List<ProjectUserScript> unCategorisedScripts = context.ProjectUserScripts.Where(pus => (pus.WorkflowStatusId != categorizationWfId || pus.WorkflowStatusId == null) && pus.ProjectId == projectId && pus.Qigid == qigId && !pus.Isdeleted).ToList();

                if (unCategorisedScripts != null && unCategorisedScripts.Count > 0)
                {
                    context.ProjectUserScripts.UpdateRange(unCategorisedScripts.Select(a =>
                    {
                        a.IsRecommended = false;
                        a.RecommendedBy = null;
                        a.RecommendedDate = null;
                        a.WorkflowStatusId = LiveMarkingWfId;
                        return a;
                    }).ToList());
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in MoveUncategorisedScripttoLiveMarking() : qigId=" + qigId + ", ProjectUserRoleID = " + ProjectUserRoleID + ", projectId=" + projectId + "");
                throw;
            }
        }

    }
}
