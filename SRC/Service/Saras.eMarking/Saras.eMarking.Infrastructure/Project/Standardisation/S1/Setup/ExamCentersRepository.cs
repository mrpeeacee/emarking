using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Standardisation.S1.Setup
{
    public class ExamCentersRepository : BaseRepository<ExamCentersRepository>, IExamCenterRepository
    {
        private readonly ApplicationDbContext context;
        public ExamCentersRepository(ApplicationDbContext context, ILogger<ExamCentersRepository> _logger) : base(_logger)
        {
            this.context = context;
        }
        /// <summary>
        /// ProjectCenters : This GET Api is used to get the project centers
        /// </summary>
        /// <param name="ProjectId">Project Id</param>
        /// <param name="QigId">Qig Id</param>
        /// <returns></returns>
        public async Task<IList<ExamCenterModel>> ProjectCenters(long ProjectId, long QigId)
        {
            List<ExamCenterModel> Centers = null;
            try
            {
                Centers = (from cen in context.ProjectCenters
                           where cen.ProjectId == ProjectId && !cen.IsDeleted
                           select new ExamCenterModel
                           {
                               ProjectCenterID = cen.ProjectCenterId,
                               ProjectID = cen.ProjectId,
                               CenterName = cen.CenterName,
                               CenterCode = cen.CenterCode,
                               TotalNoOfScripts = cen.TotalNoOfScripts,
                               CenterID = cen.CenterId
                           }).ToList();

                List<long?> recCenterIds = (await (from PUS in context.ProjectUserScripts
                                                  where PUS.ProjectId == ProjectId && !PUS.Isdeleted && PUS.IsRecommended == true
                                                  && PUS.Qigid == QigId
                                                  select PUS.ProjectCenterId).ToListAsync()).ToList();

                List<long> Centersmappinglist = (await (from centermapping in context.ProjectQigcenterMappings
                                                       where centermapping.ProjectId == ProjectId && centermapping.Qigid == QigId
                                                       && !centermapping.IsDeleted
                                                       select centermapping.ProjectCenterId).ToListAsync()).ToList();

                var noresponcecount = 0;

                if(context.ProjectQigs.Any(p => p.ProjectQigid == QigId && !p.IsDeleted && p.IsAllQuestionMandatory))
                {
                    var ltscript = (from p in context.ProjectUserScripts
                                       join q in context.ProjectUserQuestionResponses on p.ScriptId equals q.ScriptId
                                       where p.ProjectId == ProjectId && p.Qigid == QigId && q.IsNullResponse
                                       select new ProjectUserScript
                                       {
                                           ScriptId = p.ScriptId
                                       }).ToList();

                    noresponcecount = ltscript.DistinctBy(p => p.ScriptId).Count();
                }
                else
                {
                    var ltscript = context.ProjectUserScripts.Where(p => p.ProjectId == ProjectId
                                      && p.Qigid == QigId && p.ScriptType == 1 && !p.Isdeleted).ToList();

                    noresponcecount = ltscript.DistinctBy(p => p.ScriptId).Count();


                }

                Centers.ForEach(center =>
                {
                    center.IsRecommended = recCenterIds.Any(a => a == center.ProjectCenterID);
                    if (Centersmappinglist.Any(a => a == center.ProjectCenterID))
                    {
                        center.IsSelectedForRecommendation = true;
                    }
                    center.noresponsecount = noresponcecount;
                });
                Centers = Centers.OrderBy(a => a.CenterName).ToList();

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Standardisation setup page while getting Centers for specific: Method Name: ProjectCenters() and project: ProjectID=" + ProjectId.ToString());
                throw;
            }
            return Centers;

        }

        /// <summary>
        /// UpdateProjectCenters : This POST Api is used to Update the Project Centers
        /// </summary>
        /// <param name="objExamCenterModel">objExamCenterModel</param>
        /// <param name="ProjectUserRoleID">ProjectUser RoleID</param>
        /// <param name="ProjectId">Project Id</param>
        /// <param name="QigId">Qig Id</param>
        /// <returns></returns>
        public async Task<string> UpdateProjectCenters(List<ExamCenterModel> objExamCenterModel, long ProjectUserRoleID, long ProjectId, long QigId)
        {
            string status = "";
            List<ProjectQigcenterMapping> projectQigCenterMapping;
            ProjectQigcenterMapping ProjectQigCenterMappingcreate;
            try
            {
                string QigStatus = CheckQIGWorkflowStatus(QigId, context);
                if (QigStatus == "S1Comp")
                {
                    return "S1Completed";
                }

                if (QigStatus == string.Empty)
                {

                    projectQigCenterMapping = (await (from centermapping in context.ProjectQigcenterMappings
                                                     where centermapping.ProjectId == ProjectId && centermapping.Qigid == QigId
                                                     && !centermapping.IsDeleted
                                                     select centermapping).ToListAsync()).ToList();

                    projectQigCenterMapping.ForEach(qigcentermapping =>
                    {
                        if (!objExamCenterModel.Any(a => a.ProjectCenterID == qigcentermapping.ProjectCenterId))
                        {
                            qigcentermapping.ModifiedDate = DateTime.UtcNow;
                            qigcentermapping.ModifiedBy = ProjectUserRoleID;
                            qigcentermapping.IsDeleted = true;
                            context.ProjectQigcenterMappings.Update(qigcentermapping);
                        }
                        _ = context.SaveChanges();
                        status = "UP001";
                    });

                    projectQigCenterMapping = (await (from centermapping in context.ProjectQigcenterMappings
                                                     where centermapping.ProjectId == ProjectId && centermapping.Qigid == QigId
                                                     && !centermapping.IsDeleted
                                                     select centermapping).ToListAsync()).ToList();

                    objExamCenterModel.ForEach(centermodel =>
                    {
                        if (!projectQigCenterMapping.Any(a => a.ProjectCenterId == centermodel.ProjectCenterID))
                        {
                            ProjectQigCenterMappingcreate = new ProjectQigcenterMapping()
                            {
                                ProjectCenterId = centermodel.ProjectCenterID,
                                Qigid = QigId,
                                ProjectId = ProjectId,
                                NoOfCandidates = centermodel.TotalNoOfScripts,
                                IsDeleted = false,
                                CreatedBy = ProjectUserRoleID,
                                CreatedDate = DateTime.UtcNow,
                                RecommendedBy = ProjectUserRoleID,
                                RecommendedDate = DateTime.UtcNow
                            };
                            context.ProjectQigcenterMappings.Add(ProjectQigCenterMappingcreate);
                            _ = context.SaveChanges();
                        }
                        status = "UP001";
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Standardisation setup page while updating ProjectCenter: Method Name: UpdateProjectCenters()");
                throw;
            }
            return status;
        }

    }
}
