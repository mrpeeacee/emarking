using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Standardisation.S1.Setup
{
    public class KeyPersonnelsRepository : BaseRepository<KeyPersonnelsRepository>, IKeyPersonnelRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public KeyPersonnelsRepository(ApplicationDbContext context, ILogger<KeyPersonnelsRepository> _logger, IAppCache appCache) : base(_logger)
        {
            this.context = context;
            AppCache = appCache;
        }

        /// <summary>
        /// UpdateKeyPersonnels : This POST Api is used to update the the key peronnels
        /// </summary>
        /// <param name="objKeyPersonnelModel">objKeyPersonnelModel</param>
        /// <param name="ProjectUserRoleID">ProjectUser RoleID</param>
        /// <param name="ProjectId">ProjectId</param>
        /// <param name="QigId">Qig Id</param>
        /// <returns>status</returns>
        public async Task<string> UpdateKeyPersonnels(List<KeyPersonnelModel> objKeyPersonnelModel, long ProjectUserRoleID, long ProjectId, long QigId)
        {
            string status = "";
            List<ProjectQigteamHierarchy> result;
            try
            {
                string QigStatus = CheckQIGWorkflowStatus(QigId, context);
                if (QigStatus == "S1Comp")
                {
                    return "S1Completed";
                }

                if (CheckQIGWorkflowStatus(QigId, context) == "")
                {

                    result = (await (from pqt in context.ProjectQigteamHierarchies
                                    where pqt.ProjectId == ProjectId && pqt.Qigid == QigId && !pqt.Isdeleted && pqt.IsKp
                                    && pqt.IsActive == true
                                    select pqt).ToListAsync()).ToList();

                    result.ForEach(qigteam =>
                    {
                        if (!objKeyPersonnelModel.Any(a => a.ProjectUserRoleID == qigteam.ProjectUserRoleId))
                        {
                            if (context.ProjectUserScripts.Any(i => i.Qigid == QigId && i.RecommendedBy == qigteam.ProjectUserRoleId && !i.Isdeleted))
                            {

                                status = "Alreadyrecommendedortrailmarked";
                            }

                            else if ((from PUS in context.UserScriptMarkingDetails
                                      join pu in context.ProjectUserScripts
                                      on PUS.ScriptId equals pu.ScriptId
                                      where PUS.ProjectId == ProjectId && !PUS.IsDeleted && PUS.IsActive == true
                                      && pu.Qigid == QigId && !pu.Isdeleted && PUS.MarkedBy == qigteam.ProjectUserRoleId
                                      select PUS.MarkedBy).Any())
                            {
                                status = "Alreadyrecommendedortrailmarked";
                            }
                        }
                    });

                    if (status != "Alreadyrecommendedortrailmarked")
                    {

                        result.ForEach(qigteam =>
                        {
                            if (!objKeyPersonnelModel.Any(a => a.ProjectUserRoleID == qigteam.ProjectUserRoleId))
                            {
                                qigteam.ModifiedDate = DateTime.UtcNow;
                                qigteam.ModifiedBy = ProjectUserRoleID;
                                qigteam.IsKp = false;
                                context.ProjectQigteamHierarchies.Update(qigteam);
                            }
                            _ = context.SaveChanges();
                            status = "UP002";
                        });


                        result = (await (from pqt in context.ProjectQigteamHierarchies
                                        where pqt.ProjectId == ProjectId && pqt.Qigid == QigId && !pqt.Isdeleted && pqt.IsKp
                                        && pqt.IsActive == true
                                        select pqt).ToListAsync()).ToList();

                        objKeyPersonnelModel.ForEach(qigmodel =>
                        {
                            if (!result.Any(a => a.ProjectUserRoleId == qigmodel.ProjectUserRoleID))
                            {
                                var res = (from pqt in context.ProjectQigteamHierarchies
                                           where pqt.ProjectUserRoleId == qigmodel.ProjectUserRoleID && !pqt.IsKp &&
                                           pqt.Qigid == QigId && pqt.ProjectId == ProjectId && !pqt.Isdeleted && pqt.IsActive == true
                                           select pqt).ToList();

                                res.ForEach(projectqigteamy =>
                                {
                                    projectqigteamy.ModifiedDate = DateTime.UtcNow;
                                    projectqigteamy.ModifiedBy = ProjectUserRoleID;
                                    projectqigteamy.IsKp = true;
                                    context.ProjectQigteamHierarchies.Update(projectqigteamy);
                                });
                                _ = context.SaveChanges();
                            }
                            status = "UP002";
                        });
                    }
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Standardisation setup page while updating Key Personnels: Method Name: UpdateKeyPersonnels()");
                throw;
            }
            return status;
        }

        /// <summary>
        /// ProjectKps : This GET Api is used to get the project kps
        /// </summary>
        /// <param name="ProjectId">Project Id</param>
        /// <param name="QigId">Qig Id</param>
        /// <returns>kpmodel</returns>
        public async Task<IList<KeyPersonnelModel>> ProjectKps(long ProjectId, long QigId)
        {
            List<KeyPersonnelModel> kpmodel = null;
            try
            {
                kpmodel = (await (from uri in context.ProjectUserRoleinfos
                                 join u in context.UserInfos on uri.UserId equals u.UserId
                                 join p in context.ProjectInfos on uri.ProjectId equals p.ProjectId
                                 join r in context.Roleinfos on uri.RoleId equals r.RoleId
                                 join PQT in context.ProjectQigteamHierarchies on uri.ProjectUserRoleId equals PQT.ProjectUserRoleId
                                 where !r.Isdeleted && !uri.Isdeleted &&
                                 uri.IsActive == true && !u.IsDeleted && PQT.ProjectId == ProjectId && r.RoleCode != "MARKER" &&
                                 r.RoleCode != "EO" && PQT.IsActive == true && PQT.Qigid == QigId && !PQT.Isdeleted && PQT.IsActive == true
                                 select new KeyPersonnelModel
                                 {
                                     ProjectUserRoleID = uri.ProjectUserRoleId,
                                     RoleID = r.RoleId,
                                     RoleCode = r.RoleCode,
                                     LoginName = u.LoginId,
                                     IsKP = PQT.IsKp
                                 }).Distinct().ToListAsync()).ToList();

                var trialid = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script);
                var catid = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script);


                List<long?> recByIds = (await (from PUS in context.ProjectUserScripts
                                              where PUS.ProjectId == ProjectId && !PUS.Isdeleted && PUS.IsRecommended == true
                                              && PUS.Qigid == QigId
                                              select PUS.RecommendedBy).ToListAsync()).ToList();

                List<long?> trialByIds = (await (from PUS in context.UserScriptMarkingDetails
                                                join pu in context.ProjectUserScripts
                                                on PUS.ScriptId equals pu.ScriptId
                                                where PUS.ProjectId == ProjectId && !PUS.IsDeleted && PUS.IsActive == true
                                                && pu.Qigid == QigId && (PUS.WorkFlowStatusId == trialid || PUS.WorkFlowStatusId == catid)
                                                select PUS.MarkedBy).ToListAsync()).ToList();

                kpmodel.ForEach(kp =>
                {
                    kp.IsKpTagged = recByIds.Any(a => a == kp.ProjectUserRoleID);
                    kp.IsKpTrialmarkedorcategorised = trialByIds.Any(b => b == kp.ProjectUserRoleID);
                });
                kpmodel = kpmodel.OrderBy(a => a.LoginName).ToList();

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Standardisation setup page while getting Project Kps for specific Project: Method Name: ProjectKps() and project: ProjectID=" + ProjectId.ToString());
                throw;
            }
            return kpmodel;
        }
    }
}
