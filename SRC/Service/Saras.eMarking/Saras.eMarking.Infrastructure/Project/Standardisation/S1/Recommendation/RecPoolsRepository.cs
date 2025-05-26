using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.Recommendation;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Standardisation.S1.Recommendation
{
    public class RecPoolsRepository : BaseRepository<RecPoolsRepository>, IRecPoolRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public RecPoolsRepository(ApplicationDbContext context, ILogger<RecPoolsRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            AppCache = _appCache;
        }
        public async Task<RecPoolModel> GetRecPoolStastics(long ProjectId, long QigId, UserRole CurrentRole)
        {
            if (CurrentRole is null)
            {
                await Task.Yield();
                throw new ArgumentNullException(nameof(CurrentRole));
            }

            RecPoolModel result = null;
            try
            {
                if (CurrentRole.ProjectUserRoleID > 0)
                {

                    var projectUserScriptEntities = (from PUS in context.ProjectUserScripts
                                                     where !PUS.Isdeleted && PUS.Qigid == QigId &&
                                                      (PUS.WorkflowStatusId == null || PUS.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Recomended, EnumWorkflowType.Script)
                                                         || PUS.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script)
                                                         || PUS.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script))
                                                     select new
                                                     {
                                                         PUS.IsRecommended,
                                                         PUS.RecommendedBy
                                                     }).ToList();

                    if (projectUserScriptEntities != null && projectUserScriptEntities.Count > 0)
                    {
                        List<AppSetting> appsettings = await (from APK in context.AppSettingKeys
                                                              join APS in context.AppSettings on APK.AppsettingKeyId equals APS.AppSettingKeyId
                                                              where APK.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.RecomendationPoolCount) && !APS.Isdeleted && APS.EntityType == (byte)EnumAppSettingEntityType.QIG && APS.ProjectId == ProjectId
                                                              select APS).ToListAsync();


                        var appValue = appsettings.FirstOrDefault(a => a.EntityId == QigId);
                        result = new RecPoolModel()
                        {
                            TotalTargetRecomendations = appValue != null ? Convert.ToInt64(appValue.Value) : 0,
                            TotalRecomended = projectUserScriptEntities.Count(a => a.IsRecommended == true),
                            TotalRecomendedByMe = projectUserScriptEntities.Count(a => a.IsRecommended == true && a.RecommendedBy == CurrentRole.ProjectUserRoleID),
                            IsAoCm = CurrentRole.RoleCode == "AO" || CurrentRole.RoleCode == "CM"
                        };

                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in RecPoolRepository page while getting Team Structure for specific project: Method Name: GetRecPoolStastics(): ProjectID=" + ProjectId.ToString(), "Error in team structure page while getting Team Structure for specific project: Method Name: GetTeamStructure(): ProjectID=" + ProjectId.ToString());
                throw;
            }
            return result;
        }
        public async Task<IList<RecPoolScriptModel>> GetRecPoolScript(long projectid, long QIGID, UserRole CurrentRole, long ScriptId = 0)
        {
            if (CurrentRole is null)
            {
                await Task.Yield();
                throw new ArgumentNullException(nameof(CurrentRole));
            }

            List<RecPoolScriptModel> result = new();
            try
            {
                if (CurrentRole.ProjectUserRoleID > 0)
                {
                    result = await (from PQIG in context.ProjectQigs
                                    join AS in context.ProjectUserScripts on PQIG.ProjectQigid equals AS.Qigid
                                    //join puqr in context.ProjectUserQuestionResponses on AS.ScriptId equals puqr.ScriptId
                                    join PC in context.ProjectCenters on AS.ProjectCenterId equals PC.ProjectCenterId
                                    join PQCM in context.ProjectQigcenterMappings on PC.ProjectCenterId equals PQCM.ProjectCenterId
                                    join PUR in context.ProjectUserRoleinfos on AS.RecommendedBy equals PUR.ProjectUserRoleId into scriptusers
                                    from su in scriptusers.DefaultIfEmpty()
                                    join recuser in context.UserInfos on su.UserId equals recuser.UserId into recuserGroup
                                    from p in recuserGroup.DefaultIfEmpty()
                                    where PQIG.ProjectId == projectid && PQIG.ProjectQigid == QIGID && PQCM.Qigid == QIGID
                                    && (AS.ScriptType == 3 || AS.ScriptType == 2)
                                    && (AS.WorkflowStatusId == null || AS.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Recomended, EnumWorkflowType.Script)
                                                     || AS.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.TrailMarking, EnumWorkflowType.Script)
                                                     || AS.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Categorization, EnumWorkflowType.Script))
                                    && !PC.IsDeleted && !PQIG.IsDeleted && !AS.Isdeleted && !PQCM.IsDeleted
                                    && (AS.IsRecommended == false || AS.RecommendedBy == CurrentRole.ProjectUserRoleID || CurrentRole.RoleCode == "AO" || CurrentRole.RoleCode == "CM")
                                    
                                    select new RecPoolScriptModel()
                                    {
                                        ProjectID = PQIG.ProjectId,
                                        //IsNullResponse = context.ProjectUserQuestionResponses.Any(a => a.ScriptId == AS.ScriptId && a.IsNullResponse && !a.Isdeleted),
                                        ScriptID = AS.ScriptId,
                                        ScriptName = AS.ScriptName,
                                        IsRecommended = AS.IsRecommended,
                                        RecommendedBy = p.LoginId,
                                        RoleCode = CurrentRole.RoleCode,
                                        CenterID = PC.CenterId,
                                        CenterName = PC.CenterName,
                                        CenterCode = PC.CenterCode,
                                        IscenterSelected = false,
                                        IsRecommendedByMe = AS.RecommendedBy == CurrentRole.ProjectUserRoleID,
                                        WorkFlowStatusCode = AppCache.GetWorkflowStatusCode(AS.WorkflowStatusId, EnumWorkflowType.Script)
                                    }).ToListAsync();

                    if (ScriptId > 0 && result != null)
                    {
                        result = result.Where(a => a.ScriptID == ScriptId).ToList();
                    }

                    if (CurrentRole.RoleCode != "AO" && CurrentRole.RoleCode != "CM")
                        result = result.OrderBy(a => a.RecommendedBy).ToList();

                }
                if (result != null && result.Count > 0)
                {
                    var lt_processstatus = (from PWFT in context.ProjectWorkflowStatusTrackings
                                            join WFS in context.WorkflowStatuses on PWFT.WorkflowStatusId equals WFS.WorkflowId
                                            where WFS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Pause) && PWFT.EntityId == QIGID && PWFT.EntityType == (int)EnumAppSettingEntityType.QIG
                                            select
                                                PWFT).ToList();
                    var processstaus = lt_processstatus.OrderByDescending(a => a.ProjectWorkflowTrackingId).Select(a => a.ProcessStatus).FirstOrDefault();

                    result.ForEach(a => a.ProcessStatus = (WorkflowProcessStatus)processstaus);
                    //result = result.Where(a => a.IsNullResponse == false).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in RecPoolRepository page while getting Team Structure for specific project: Method Name: GetRecPoolStastics(): QIGID=" + QIGID.ToString(), "Error in team structure page while getting Team Structure for specific project: Method Name: GetRecommendationsQIGScripts(): QIGID=" + QIGID.ToString());
                throw;
            }
            return result;
        }
    }
}
