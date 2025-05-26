using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Setup.QigConfiguration
{
    public class QigSummeryRepository : BaseRepository<QigSummeryRepository>, IQigSummeryRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;

        public QigSummeryRepository(ApplicationDbContext context, ILogger<QigSummeryRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            AppCache = _appCache;
        }

        /// <summary>
        /// GetQigSummary : This GET Api used to get the qig summary
        /// </summary>
        /// <param name="qigId"></param>
        /// <param name="projectUserRoleID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns> 
        public async Task<QigSummaryModel> GetQigSummary(long qigId, long projectUserRoleID, long projectID)
        {
            QigSummaryModel qigSummaryModel = new QigSummaryModel();

            var QigSetup = await context.ProjectWorkflowStatusTrackings.Where(p => p.EntityId == qigId && !p.IsDeleted && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.QigSetup, EnumWorkflowType.Qig)).FirstOrDefaultAsync();

            if (QigSetup != null)
            {
                qigSummaryModel.IsQigSetup = true;
            }


            qigSummaryModel.IsLiveMarkingStart = context.ProjectWorkflowStatusTrackings.Any(p => p.EntityId == qigId && !p.IsDeleted && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Live_Marking_Qig, EnumWorkflowType.Qig));

            return qigSummaryModel;
        }

        /// <summary>
        /// SaveQigSummery : This POST Api used to save the Qig summary for specific project
        /// </summary>
        /// <param name="qigId"></param>
        /// <param name="isProjectSetupTrue"></param>
        /// <param name="isLiveMarkingTrue"></param>
        /// <param name="projectUserRoleID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns> 
        public Task<string> SaveQigSummery(long qigId, bool isProjectSetupTrue, bool isLiveMarkingTrue, long projectUserRoleID, long projectID)
        {
            string status = "ERR01";

            using (var DbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (isProjectSetupTrue && !context.ProjectWorkflowStatusTrackings.Any(p => p.EntityId == qigId && !p.IsDeleted && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.QigSetup, EnumWorkflowType.Qig)))
                    {
                        ProjectWorkflowStatusTracking projectWorkflowStatusTracking = new()
                        {
                            EntityId = qigId,
                            EntityType = (byte)EnumAppSettingEntityType.QIG,
                            ProcessStatus = (int)WorkflowProcessStatus.Completed,
                            WorkflowStatusId = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.QigSetup, EnumWorkflowType.Qig),
                            CreatedBy = projectUserRoleID
                        };

                        context.ProjectWorkflowStatusTrackings.Add(projectWorkflowStatusTracking);

                        context.SaveChanges();
                        status = "SU001";
                    }


                    if (isLiveMarkingTrue && context.ProjectWorkflowStatusTrackings.Any(p => p.EntityId == qigId && !p.IsDeleted && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.QigSetup, EnumWorkflowType.Qig)) &&
                        !context.ProjectWorkflowStatusTrackings.Any(p => p.EntityId == qigId && !p.IsDeleted && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Live_Marking_Qig, EnumWorkflowType.Qig)))
                    {

                        StartedLiveMarking(qigId, projectUserRoleID, projectID);
                        _ = RcCheckSchedulerRepository.CreateUpdateRcSchedulerJob(context, logger, projectID, qigId, AppCache, projectUserRoleID);
                        context.SaveChanges();
                        status = "SU001";

                    }
                    DbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    DbContextTransaction.Rollback();
                    logger.LogError(ex,"Error in QigSummery Page while save qig summary : Method Name: SaveQigSummery() and ProjectID = " + projectID + "");
                    throw;
                }
            }
            return Task.FromResult(status);

        }

        /// <summary>
        /// StartedLiveMarking : This Method used to check weather live marking started or not
        /// </summary>
        /// <param name="qigId"></param>
        /// <param name="projectUserRoleID"></param>
        /// <param name="ProjectID"></param>
        private void StartedLiveMarking(long qigId, long projectUserRoleID, long ProjectID)
        {
            List<ProjectWorkflowStatusTracking> livemarkingstatus = context.ProjectWorkflowStatusTrackings.Where(p => p.EntityId == qigId && !p.IsDeleted && p.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Live_Marking_Qig, EnumWorkflowType.Qig)).ToList();

            if (livemarkingstatus != null && livemarkingstatus.Count > 0)
            {
                livemarkingstatus.ForEach(a =>
                {
                    a.IsDeleted = true;
                    context.ProjectWorkflowStatusTrackings.Update(a);
                });
            }

            ProjectWorkflowStatusTracking projectWorkflowStatusTracking = new ProjectWorkflowStatusTracking();

            projectWorkflowStatusTracking.EntityId = qigId;
            projectWorkflowStatusTracking.EntityType = (byte)EnumAppSettingEntityType.QIG;
            projectWorkflowStatusTracking.ProcessStatus = (int)WorkflowProcessStatus.Completed;
            projectWorkflowStatusTracking.WorkflowStatusId = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Live_Marking_Qig, EnumWorkflowType.Qig);
            projectWorkflowStatusTracking.CreatedBy = projectUserRoleID;

            context.ProjectWorkflowStatusTrackings.Add(projectWorkflowStatusTracking);

            new ShareRepository(logger).MoveUncategorisedScripttoLiveMarking(qigId, projectUserRoleID, ProjectID, AppCache, context);

            context.SaveChanges();
        }
    }
}
