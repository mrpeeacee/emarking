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
    public class StdSettingRepository : BaseRepository<StdSettingRepository>, IStdSettingRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache appCache;  
        public StdSettingRepository(ApplicationDbContext context, ILogger<StdSettingRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            this.appCache = _appCache;    
        }

        /// <summary>
        /// GetQigStdSettingsandPracticeMandatory : This GET Api is used to get the Qig Std Settings and Practice Mandatory
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public async Task<StdSettingModel> GetQigStdSettingsandPracticeMandatory(long QigId, long ProjectId)
        {
            StdSettingModel result = null;
            Boolean IsPracticemandatory = true;
            Boolean Disablestandardisationreq = false;
            try
            {
                var QIGresults = await (from pqe in context.ProjectQigs
                                        join qis in context.QigstandardizationScriptSettings on pqe.ProjectQigid equals qis.Qigid
                                        where pqe.ProjectQigid == QigId && !pqe.IsDeleted && !qis.Isdeleted
                                        select new
                                        {
                                            pqe.Qigname,
                                            qis.Qigid,
                                            qis.SettingId,
                                            qis.StandardizationScript,
                                            qis.BenchmarkScript,
                                            qis.AdditionalStdScript,
                                            qis.QualityAssuranceScript,
                                            qis.IsS1available,
                                            qis.IsS2available,
                                            qis.IsS3available,
                                            qis.S1startDate
                                        }).OrderBy(x => x.Qigid).FirstOrDefaultAsync();

                if (context.AppSettings.Any(i => i.AppSettingKeyId == appCache.GetAppsettingKeyId(EnumAppSettingKey.PracticeMandatory) && i.EntityId == QigId && i.ProjectId == ProjectId && !i.Isdeleted))
                    IsPracticemandatory = Convert.ToBoolean(context.AppSettings.Where(i => i.AppSettingKeyId == appCache.GetAppsettingKeyId(EnumAppSettingKey.PracticeMandatory) && i.EntityId == QigId && i.ProjectId == ProjectId && !i.Isdeleted).FirstOrDefault().Value);
                else
                    IsPracticemandatory = Convert.ToBoolean(context.AppSettings.Where(i => i.AppSettingKeyId == appCache.GetAppsettingKeyId(EnumAppSettingKey.PracticeMandatory)).FirstOrDefault().DefaultValue);


                var workflowstatus = (await (from workflowsta in context.WorkflowStatuses
                                            where (workflowsta.WorkflowCode == "RCMMED" || workflowsta.WorkflowCode == "PRACTICE" || workflowsta.WorkflowCode == "QASSESSMENT")
                                            select new { workflowsta.WorkflowId, workflowsta.WorkflowCode }).ToListAsync()).ToList();

                var recid = workflowstatus.FirstOrDefault(a => a.WorkflowCode == "RCMMED").WorkflowId;


                List<long?> recByIds = (await (from PUS in context.ProjectUserScripts
                                              where PUS.ProjectId == ProjectId && !PUS.Isdeleted && PUS.IsRecommended == true
                                              && PUS.Qigid == QigId && PUS.WorkflowStatusId == recid
                                              select PUS.RecommendedBy).ToListAsync()).ToList();

                List<long?> recById = (await (from PUS in context.ProjectUserScripts
                                             where PUS.ProjectId == ProjectId && !PUS.Isdeleted && PUS.IsRecommended == true
                                             && PUS.Qigid == QigId
                                             select PUS.RecommendedBy).ToListAsync()).ToList();

                var IsS1completed = (await (from wfs in context.WorkflowStatuses
                                           join pwst in context.ProjectWorkflowStatusTrackings
                                           on wfs.WorkflowId equals pwst.WorkflowStatusId
                                           where wfs.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Standardization_1) &&
                                           !pwst.IsDeleted && pwst.EntityId == QigId && pwst.ProcessStatus == 3 && pwst.EntityType == 2
                                           select new { wfs.WorkflowId, wfs.WorkflowCode, pwst.WorkflowStatusId }).ToListAsync()).ToList();


                var qigappsettingkey = (await (from asg in context.AppsettingGroups
                                              join apk in context.AppSettingKeys on asg.SettingGroupId equals apk.SettingGroupId
                                              join ase in context.AppSettings on apk.AppsettingKeyId equals ase.AppSettingKeyId
                                              where ase.EntityId == QigId && ase.ProjectId == ProjectId && asg.SettingGroupCode == "QIGSETTINGS" && asg.SettingGroupId == ase.AppsettingGroupId && !asg.IsDeleted && !apk.IsDeleted && !ase.Isdeleted
                                              select new { asg.SettingGroupId, apk.AppsettingKeyId, apk.AppsettingKey1, apk.AppsettingKeyName, ase.Value, ase.DefaultValue }).ToListAsync()).ToList();


                if (QIGresults != null && QIGresults.IsS1available == true)
                {
                    var disablestdreq = (await (from pqcm in context.ProjectQigcenterMappings
                                               where pqcm.ProjectId == ProjectId && !pqcm.IsDeleted
                                               && pqcm.Qigid == QigId
                                               select new { pqcm }).ToListAsync()).ToList();
                    if (disablestdreq.Count > 0)
                    {
                        Disablestandardisationreq = true;
                    }

                }
                else if (QIGresults == null || QIGresults?.IsS1available == false)
                {
                    var offstdreq = (await (from pqcm in context.ProjectUserScripts
                                           join usmd in context.UserScriptMarkingDetails on pqcm.ScriptId equals usmd.ScriptId
                                           join wfs in context.WorkflowStatuses on pqcm.WorkflowStatusId equals wfs.WorkflowId
                                           where wfs.WorkflowCode == "LVMRKG" && !wfs.IsDeleted && !usmd.IsDeleted && pqcm.ProjectId == ProjectId && !pqcm.Isdeleted
                                           && pqcm.Qigid == QigId
                                           select new { pqcm, usmd, wfs }).ToListAsync()).ToList();
                    if (offstdreq.Count > 0)
                    {
                        Disablestandardisationreq = true;
                    }
                }


                if (QIGresults != null)
                {

                    result = new StdSettingModel()
                    {
                        QIGName = QIGresults.Qigname,
                        QIGID = QIGresults.Qigid,
                        SettingID = QIGresults.SettingId,
                        StandardizationScript = QIGresults.StandardizationScript,
                        BenchmarkScript = QIGresults.BenchmarkScript,
                        AdditionalStdScript = QIGresults.AdditionalStdScript,
                        QualityAssuranceScript = QIGresults.QualityAssuranceScript,
                        IsS1Available = QIGresults.IsS1available,
                        IsS2Available = QIGresults.IsS2available,
                        IsS3Available = QIGresults.IsS3available,
                        S1StartDate = QIGresults.S1startDate,
                        IsPracticemandatory = IsPracticemandatory,
                        IsRecommandedQig = recByIds.Count > 0,
                        IsPracticed = false,
                        Disablestandardisationreq = Disablestandardisationreq,
                        IsS1ClosureCompleted = IsS1completed.Count > 0,
                        RecommendMarkScheme = qigappsettingkey.Any(i => i.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.QuestionLevel) && i.Value == "true") ? StringEnum.GetStringValue(EnumAppSettingKey.QuestionLevel) : StringEnum.GetStringValue(EnumAppSettingKey.QIGLevel),
                        IsScriptRecommended = recById != null && recById.Count > 0,
                        IsLiveMarkingStart = context.ProjectWorkflowStatusTrackings.Any(p => p.EntityId == QigId && !p.IsDeleted && p.WorkflowStatusId == appCache.GetWorkflowStatusId(EnumWorkflowStatus.Live_Marking_Qig, EnumWorkflowType.Qig))

                    };
                }
                else
                {

                    result = new StdSettingModel()
                    {
                        SettingID = 0,
                        StandardizationScript = 1,
                        BenchmarkScript = 1,
                        AdditionalStdScript = 0,
                        IsS1Available = false,
                        IsS2Available = false,
                        IsS3Available = false,
                        IsPracticemandatory = IsPracticemandatory,
                        IsRecommandedQig = recByIds.Count > 0,
                        IsPracticed = false,
                        Disablestandardisationreq = Disablestandardisationreq,
                        IsS1ClosureCompleted = IsS1completed.Count > 0,
                        RecommendMarkScheme = qigappsettingkey.Any(i => i.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.QuestionLevel) && i.Value == "true") ? StringEnum.GetStringValue(EnumAppSettingKey.QuestionLevel) : StringEnum.GetStringValue(EnumAppSettingKey.QIGLevel),
                        IsScriptRecommended = recById != null && recById.Count > 0
                    };

                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in StdSettingRepository page while getting QIG Standardization Script : Method Name : GetQigStdSettingsandPracticeMandatory() and QidId=" + QigId.ToString());
                throw;
            }
            return result;
        }

        /// <summary>
        /// UpdateQigStdSettingsandPracticeMandatory : This POST Api is used save the QIG Standardisation Script Settings and Practice mandatory
        /// </summary>
        /// <param name="objQIGStandardizationScriptSettingsModel"></param>
        /// <param name="ProjectUserRoleID"></param>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public async Task<string> UpdateQigStdSettingsandPracticeMandatory(StdSettingModel objQIGStandardizationScriptSettingsModel, long ProjectUserRoleID, long ProjectID)
        {      
            string status;

            var lt_s1completed = await (from PWFT in context.ProjectWorkflowStatusTrackings
                                  join WFS in context.WorkflowStatuses on PWFT.WorkflowStatusId equals WFS.WorkflowId
                                  where WFS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Standardization_1) && PWFT.EntityId == objQIGStandardizationScriptSettingsModel.QIGID && PWFT.EntityType == 2 && !PWFT.IsDeleted && !WFS.IsDeleted
                                  select new { PWFT, WFS }).ToListAsync();
            int? S1Completed = lt_s1completed.OrderByDescending(a => a.PWFT.ProjectWorkflowTrackingId).Select(a => a.PWFT.ProcessStatus).FirstOrDefault();



            QigstandardizationScriptSetting Qigstdscriptsettings;
            try
            {
    
                if (objQIGStandardizationScriptSettingsModel.IsLiveMarkingStart != true)
                {
                    Qigstdscriptsettings = context.QigstandardizationScriptSettings.Where(item => item.SettingId == objQIGStandardizationScriptSettingsModel.SettingID).FirstOrDefault();
                    if (Qigstdscriptsettings != null && S1Completed != (int)WorkflowProcessStatus.Completed)
                    {
                        Qigstdscriptsettings.StandardizationScript = objQIGStandardizationScriptSettingsModel.StandardizationScript;
                        Qigstdscriptsettings.Qigid = objQIGStandardizationScriptSettingsModel.QIGID;
                        Qigstdscriptsettings.BenchmarkScript = objQIGStandardizationScriptSettingsModel.BenchmarkScript;
                        Qigstdscriptsettings.AdditionalStdScript = objQIGStandardizationScriptSettingsModel.AdditionalStdScript;
                        Qigstdscriptsettings.QualityAssuranceScript = objQIGStandardizationScriptSettingsModel.QualityAssuranceScript;
                        Qigstdscriptsettings.IsS1available = objQIGStandardizationScriptSettingsModel.IsS1Available;
                        Qigstdscriptsettings.IsS2available = objQIGStandardizationScriptSettingsModel.IsS2Available;
                        Qigstdscriptsettings.IsS3available = objQIGStandardizationScriptSettingsModel.IsS1Available;
                        Qigstdscriptsettings.S1startDate = objQIGStandardizationScriptSettingsModel.S1StartDate;
                        Qigstdscriptsettings.ModifiedDate = DateTime.UtcNow;
                        Qigstdscriptsettings.ModifiedBy = ProjectUserRoleID;
                        context.QigstandardizationScriptSettings.Update(Qigstdscriptsettings);
                        context.SaveChanges();
                        status = "P001";

                    }
                    else if (Qigstdscriptsettings != null && S1Completed == (int)WorkflowProcessStatus.Completed)
                    {
                        Qigstdscriptsettings.Qigid = objQIGStandardizationScriptSettingsModel.QIGID;
                        Qigstdscriptsettings.IsS1available = objQIGStandardizationScriptSettingsModel.IsS1Available;
                        Qigstdscriptsettings.IsS2available = objQIGStandardizationScriptSettingsModel.IsS2Available;
                        Qigstdscriptsettings.IsS3available = objQIGStandardizationScriptSettingsModel.IsS1Available;
                        Qigstdscriptsettings.S1startDate = objQIGStandardizationScriptSettingsModel.S1StartDate;
                        Qigstdscriptsettings.ModifiedDate = DateTime.UtcNow;
                        Qigstdscriptsettings.ModifiedBy = ProjectUserRoleID;
                        context.QigstandardizationScriptSettings.Update(Qigstdscriptsettings);
                        context.SaveChanges();
                        status = "P001";
                    }
                    else
                    {
                        Qigstdscriptsettings = new QigstandardizationScriptSetting()
                        {
                            Qigid = objQIGStandardizationScriptSettingsModel.QIGID,
                            StandardizationScript = objQIGStandardizationScriptSettingsModel.StandardizationScript,
                            BenchmarkScript = objQIGStandardizationScriptSettingsModel.BenchmarkScript,
                            AdditionalStdScript = objQIGStandardizationScriptSettingsModel.AdditionalStdScript,
                            QualityAssuranceScript = objQIGStandardizationScriptSettingsModel.QualityAssuranceScript,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = ProjectUserRoleID,
                            IsS1available = objQIGStandardizationScriptSettingsModel.IsS1Available,
                            IsS2available = objQIGStandardizationScriptSettingsModel.IsS2Available,
                            IsS3available = objQIGStandardizationScriptSettingsModel.IsS1Available,//objQIGStandardizationScriptSettingsModel.IsS3Available,
                            S1startDate = objQIGStandardizationScriptSettingsModel.S1StartDate,
                        };

                        context.QigstandardizationScriptSettings.Add(Qigstdscriptsettings);
                        context.SaveChanges();
                    } 

                    List<AppSettingModel> objappsettinglist = new List<AppSettingModel>
                    {
                        new AppSettingModel()
                        {
                            EntityID = objQIGStandardizationScriptSettingsModel.QIGID,
                            EntityType = EnumAppSettingEntityType.QIG,
                            AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.PracticeMandatory),
                            SettingGroupID = appCache.GetAppsettingKeyGroupId(EnumAppSettingKey.PracticeMandatory),
                            Value = objQIGStandardizationScriptSettingsModel.IsPracticemandatory.ToString(),
                            ValueType = EnumAppSettingValueType.Bit,
                            ProjectID = ProjectID

                        },
                        new AppSettingModel()
                        {
                            EntityID = objQIGStandardizationScriptSettingsModel.QIGID,
                            EntityType = EnumAppSettingEntityType.QIG,
                            AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.QuestionLevel),
                            Value = (objQIGStandardizationScriptSettingsModel.RecommendMarkScheme.ToUpper() == StringEnum.GetStringValue(EnumAppSettingKey.QuestionLevel)).ToString().ToLower(),
                            ValueType = EnumAppSettingValueType.Bit,
                            ProjectID = ProjectID,
                            SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                        },
                        new AppSettingModel()
                        {
                            EntityID = objQIGStandardizationScriptSettingsModel.QIGID,
                            EntityType = EnumAppSettingEntityType.QIG,
                            AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.QIGLevel),
                            Value = (objQIGStandardizationScriptSettingsModel.RecommendMarkScheme.ToUpper() == StringEnum.GetStringValue(EnumAppSettingKey.QIGLevel)).ToString().ToLower(),
                            ValueType = EnumAppSettingValueType.Bit,
                            ProjectID = ProjectID,
                            SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                        },

                        new AppSettingModel()
                        {
                            EntityID = objQIGStandardizationScriptSettingsModel.QIGID,
                            EntityType = EnumAppSettingEntityType.QIG,
                            AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.QCStdSetting),
                            Value = "true",
                            ValueType = EnumAppSettingValueType.Bit,
                            ProjectID = ProjectID,
                            SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                        }
                    };

                    status = AppSettingRepository.UpdateAllSettings(context, logger, objappsettinglist, ProjectUserRoleID);
                    if (objQIGStandardizationScriptSettingsModel.IsS1Available == false)
                    {
                        var result = (from uri in context.ProjectUserRoleinfos
                                      join u in context.UserInfos on uri.UserId equals u.UserId
                                      join p in context.ProjectInfos on uri.ProjectId equals p.ProjectId
                                      join r in context.Roleinfos on uri.RoleId equals r.RoleId
                                      join PQT in context.ProjectQigteamHierarchies on uri.ProjectUserRoleId equals PQT.ProjectUserRoleId
                                      where !r.Isdeleted && !uri.Isdeleted &&
                                      uri.IsActive == true && !u.IsDeleted && PQT.ProjectId == ProjectID && (r.RoleCode == "TL" ||
                                      r.RoleCode == "ATL") && PQT.IsActive == true && PQT.Qigid == objQIGStandardizationScriptSettingsModel.QIGID
                                      && !PQT.Isdeleted && PQT.IsKp
                                      select PQT).ToList();

                        result.ForEach(qigteam =>
                        {
                            qigteam.ModifiedDate = DateTime.UtcNow;
                            qigteam.ModifiedBy = ProjectUserRoleID;
                            qigteam.IsKp = false;
                            context.ProjectQigteamHierarchies.Update(qigteam); 
                        });
                        _ = context.SaveChanges();
                    }
                }
                else
                {
                    status = "E001";
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in StdSettingRepository page while updating QIG Standardization Script : Method Name : UpdateQigStdSettingsandPracticeMandatory()");
                throw;
            }
            return status;
        }
    }
}
