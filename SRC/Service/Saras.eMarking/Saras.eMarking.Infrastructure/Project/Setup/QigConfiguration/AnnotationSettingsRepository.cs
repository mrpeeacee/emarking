using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Setup.QigConfiguration;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Setup.QigConfiguration
{
    public class AnnotationSettingsRepository : BaseRepository<AnnotationSettingsRepository>, IAnnotationSettingRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache appCache;

        public AnnotationSettingsRepository(ApplicationDbContext context, ILogger<AnnotationSettingsRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            this.appCache = _appCache;
        }

        /// <summary>
        /// GetQigAnnotationSetting :This GET Api used to get the Annotation setting for particular Qig
        /// </summary>
        /// <param name="QigId"></param>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public async Task<AnnotationSettingModel> GetQigAnnotationSetting(long QigId, long ProjectId)
        {
            AnnotationSettingModel result = null;
            Boolean IsAnnotationsMandatory = true;
            try
            {

                if (context.AppSettings.Any(i => i.AppSettingKeyId == appCache.GetAppsettingKeyId(EnumAppSettingKey.AnnotationsMandatory) && i.EntityId == QigId && i.ProjectId == ProjectId && !i.Isdeleted))
                    IsAnnotationsMandatory = Convert.ToBoolean(context.AppSettings.Where(i => i.AppSettingKeyId == appCache.GetAppsettingKeyId(EnumAppSettingKey.AnnotationsMandatory) && i.EntityId == QigId && i.ProjectId == ProjectId && !i.Isdeleted).FirstOrDefault().Value);
                else
                    IsAnnotationsMandatory = Convert.ToBoolean(context.AppSettings.Where(i => i.AppSettingKeyId == appCache.GetAppsettingKeyId(EnumAppSettingKey.AnnotationsMandatory)).FirstOrDefault().DefaultValue);
                var trialmarked = await (from pus in context.ProjectUserScripts
                                         join usm in context.UserScriptMarkingDetails on pus.ScriptId equals usm.ScriptId
                                         join ws in context.WorkflowStatuses on pus.WorkflowStatusId equals ws.WorkflowId
                                         where pus.Qigid == QigId && !pus.Isdeleted && !ws.IsDeleted && usm.ScriptMarkingStatus == 2 && !usm.IsDeleted
                                         && (ws.WorkflowCode.Contains(StringEnum.GetStringValue(EnumWorkflowStatus.TrailMarking)) || ws.WorkflowCode.Contains(StringEnum.GetStringValue(EnumWorkflowStatus.Categorization)))
                                         select new { pus.ScriptId }).ToListAsync();


                result = new AnnotationSettingModel()
                {
                    IsAnnotationsMandatory = IsAnnotationsMandatory,
                    IsScriptTrialMarked = trialmarked.Count > 0,
                    IsTagged = context.QigtoAnnotationTemplateMappings.Any(x => x.Qigid == QigId && !x.IsDeleted)
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in AnnotationSettingRepository page while Getting Qig AnnotationSetting : Method Name : GetQigAnnotationSetting() : QigId=" + Convert.ToString(QigId));
                throw;
            }
            return result;
        }

        /// <summary>
        /// UpdateQigAnnotationSetting : This POST Api used to update the Annotation setting to particular Qig
        /// </summary>
        /// <param name="qigId"></param>
        /// <param name="objqigconfigmodel"></param>
        /// <param name="projectUserRoleID"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<string> UpdateQigAnnotationSetting(long qigId, AnnotationSettingModel objqigconfigmodel, long projectUserRoleID, long projectId)
        {
            string status = "P000";
            try
            {
                var appsettings = await (from APK in context.AppSettingKeys
                                         join APS in context.AppSettings on APK.AppsettingKeyId equals APS.AppSettingKeyId
                                         where (APK.AppsettingKey1 == StringEnum.GetStringValue(EnumAppSettingKey.AnnotationsMandatory)) && !APS.Isdeleted && APS.EntityType == null && APS.EntityId == null
                                         select new
                                         {
                                             APS.EntityId,
                                             APS.EntityType,
                                             EntityValue = APS.Value,
                                             APK.AppsettingKey1,
                                             APK.AppsettingKeyId,
                                             APS.DefaultValue
                                         }).FirstOrDefaultAsync();


                List<AppSettingModel> objappsettinglist = new List<AppSettingModel>
                {
                    new AppSettingModel()
                    {
                        EntityID = qigId,
                        EntityType = EnumAppSettingEntityType.QIG,
                        AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.AnnotationsMandatory),
                        SettingGroupID = appCache.GetAppsettingKeyGroupId(EnumAppSettingKey.AnnotationsMandatory),
                        Value = objqigconfigmodel.IsAnnotationsMandatory.ToString(),
                        DefaultValue = appsettings.DefaultValue,
                        ValueType = EnumAppSettingValueType.Bit,
                        ProjectID = projectId

                    },

                    new AppSettingModel()
                    {
                        EntityID = qigId,
                        EntityType = EnumAppSettingEntityType.QIG,
                        AppSettingKeyID = appCache.GetAppsettingKeyId(EnumAppSettingKey.QCAnnotationSettings),
                        Value = "true",
                        ValueType = EnumAppSettingValueType.Bit,
                        ProjectID = projectId,
                        SettingGroupID = appCache.GetAppsettingGroupId(EnumAppSettingGroup.QIGSettings)
                    }
                };

                status = AppSettingRepository.UpdateAllSettings(context, logger, objappsettinglist, projectUserRoleID);


            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in AnnotationSettingRepository page while updating Qig AnnotationSetting : Method Name : UpdateQigAnnotationSetting() : QigId=" + Convert.ToString(qigId), "Error in AnnotationSettingRepository page while updating Qig AnnotationSetting : Method Name : UpdateQigAnnotationSetting() : ProjectUserRoleID=" + projectUserRoleID.ToString());
                throw;
            }
            return status;
        }

        /// <summary>
        /// SaveAnnotationForQIG : This POST Api is used save the Annotation for particular Qig
        /// </summary>
        /// <param name="qigId"></param>
        /// <param name="qigAnnotationDetails"></param>
        /// <param name="projectId"></param>
        /// <param name="projectUserRoleID"></param>
        /// <returns></returns>
        public async Task<string> SaveAnnotationForQIG(long qigId, QigAnnotationDetails qigAnnotationDetails, long projectId, long projectUserRoleID)
        {
            string status = "ER001";

            try
            {
                var AnnotationTemplatename = await context.QigtoAnnotationTemplateMappings.Where(x => x.Qigid == qigId && !x.IsDeleted).FirstOrDefaultAsync();

                if (AnnotationTemplatename == null)
                {


                    AnnotationTemplate annotationTemplate = new AnnotationTemplate()
                    {
                        AnnotationTemplateName = "Template-" + qigId,
                        AnnotationTemplateCode = "Template-" + qigId,
                        IsDefault = false,
                        IsTagged = true,
                        Isdeleted = false,
                        CreatedDate = DateTime.Now,
                        CreatedBy = projectUserRoleID

                    };

                    context.AnnotationTemplates.Add(annotationTemplate);
                    context.SaveChanges();

                    Domain.Entities.QigtoAnnotationTemplateMapping qigmapping = new Domain.Entities.QigtoAnnotationTemplateMapping()
                    {
                        Qigid = qigId,
                        AnnotationTemplateId = context.AnnotationTemplates.Where(x => x.AnnotationTemplateCode == "Template-" + qigId && !x.Isdeleted).Select(x => x.AnnotationTemplateId).FirstOrDefault(),
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = projectUserRoleID,
                        CreatedDate = DateTime.UtcNow
                    };

                    context.QigtoAnnotationTemplateMappings.Add(qigmapping);
                    context.SaveChanges();

                }

                var annotationID = await context.QigtoAnnotationTemplateMappings.Where(x => x.Qigid == qigId && !x.IsDeleted).Select(x => x.AnnotationTemplateId).FirstOrDefaultAsync();

                List<ClsAnnotationToolIds> ltanntoolids = new List<ClsAnnotationToolIds>();

                List<AnnotationGroups> Lt_Annotationgrp = qigAnnotationDetails.AnnotationGroup;

                foreach (var item in Lt_Annotationgrp)
                {
                    foreach (var annotationTool in item.AnnotationTools)
                    {

                        ClsAnnotationToolIds clsAnnotationToolIDs = new ClsAnnotationToolIds();

                        clsAnnotationToolIDs.AnnotationToolID = context.AnnotationTools.Where(x => x.AnnotationToolCode == annotationTool.AnnotationToolCode).Select(x => x.AnnotationToolId).FirstOrDefault();

                        clsAnnotationToolIDs.isChecked = annotationTool.isChecked;

                        ltanntoolids.Add(clsAnnotationToolIDs);

                    }
                }



                if (ltanntoolids.Count > 0)
                {
                    foreach (var anntolids in ltanntoolids)
                    {
                        if (context.AnnotationTemplateDetails.Any(x => x.AnnotationToolId == anntolids.AnnotationToolID && x.AnnotationTemplateId == annotationID))
                        {
                            if (anntolids.isChecked)
                            {
                                var AnnotationTemplateDetails = context.AnnotationTemplateDetails.Where(x => x.AnnotationTemplateId == annotationID && x.AnnotationToolId == anntolids.AnnotationToolID).FirstOrDefault();

                                if (AnnotationTemplateDetails != null)
                                {
                                    AnnotationTemplateDetails.Isdeleted = false;
                                    AnnotationTemplateDetails.ModifiedBy = projectUserRoleID;
                                    AnnotationTemplateDetails.ModifiedDate = DateTime.UtcNow;

                                    context.AnnotationTemplateDetails.Update(AnnotationTemplateDetails);
                                    context.SaveChanges();
                                }
                            }
                            else
                            {
                                if (context.AnnotationTemplateDetails.Any(x => !x.Isdeleted && x.AnnotationToolId == anntolids.AnnotationToolID && x.AnnotationTemplateId == annotationID))
                                {
                                    var AnnotationTemplateDetails = context.AnnotationTemplateDetails.Where(x => x.AnnotationTemplateId == annotationID && x.AnnotationToolId == anntolids.AnnotationToolID).FirstOrDefault();

                                    if (AnnotationTemplateDetails != null)
                                    {
                                        AnnotationTemplateDetails.Isdeleted = true;
                                        AnnotationTemplateDetails.ModifiedBy = projectUserRoleID;
                                        AnnotationTemplateDetails.ModifiedDate = DateTime.UtcNow;

                                        context.AnnotationTemplateDetails.Update(AnnotationTemplateDetails);
                                        context.SaveChanges();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (anntolids.isChecked)
                            {
                                AnnotationTemplateDetail annotationTemplateDetails = new AnnotationTemplateDetail()
                                {
                                    AnnotationTemplateId = annotationID,
                                    AnnotationToolId = anntolids.AnnotationToolID,
                                    Isdeleted = false,
                                    CreatedDate = DateTime.UtcNow,
                                    CreatedBy = projectUserRoleID

                                };

                                context.AnnotationTemplateDetails.Add(annotationTemplateDetails);
                                context.SaveChanges();
                            }
                        }
                    }
                }

                status = "S001";
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in AnnotationSettingRepository page while save QIG Annotation : Method Name : SaveAnnotationForQIG() and ProjectID=" + projectId + "ProjectUserRoleID=" + projectUserRoleID);
                throw;
            }

            return status;
        }

        /// <summary>
        /// GetAnnotationForQIG : This GET Api used to get the Annotation for particular Qig
        /// </summary>
        /// <param name="qigId"></param>
        /// <param name="projectID"></param>
        /// <param name="projectUserRoleID"></param>
        /// <returns></returns>
        public async Task<QigAnnotationDetails> GetAnnotationForQIG(long qigId, long projectID, long projectUserRoleID)
        {
            try
            {
                QigAnnotationDetails qigAnnotationDetails = new QigAnnotationDetails();

                List<AnnotationGroups> Lt_AnnotationGroup = new List<AnnotationGroups>();

                var AnnotationGroupIDs = context.AnnotationTools.Where(at => !at.IsDeleted).GroupBy(x => x.AnnotationGroupId)
                    .Select(x => new { AnnotationGroupID = x.Key }).ToList();



                var AnnotationTemplatename = await context.QigtoAnnotationTemplateMappings.Where(x => x.Qigid == qigId && !x.IsDeleted).FirstOrDefaultAsync();

                if (AnnotationTemplatename != null)
                {

                    var Templatename = context.AnnotationTemplates.Where(x => x.AnnotationTemplateId == AnnotationTemplatename.AnnotationTemplateId).FirstOrDefault();
                    qigAnnotationDetails.TemplateName = Templatename.AnnotationTemplateName;
                    qigAnnotationDetails.TemplateCode = Templatename.AnnotationTemplateCode;

                    foreach (var annotationGroupID in AnnotationGroupIDs)
                    {

                        AnnotationGroups annotationGroups = new AnnotationGroups();

                        annotationGroups.GroupName = context.AnnotationGroups.Where(ag => ag.AnnotationGroupId == annotationGroupID.AnnotationGroupID && !ag.Isdeleted).Select(x => x.AnnotationGroupName).FirstOrDefault();

                        annotationGroups.GroupCode = context.AnnotationGroups.Where(ag => ag.AnnotationGroupId == annotationGroupID.AnnotationGroupID && !ag.Isdeleted).Select(x => x.AnnotationGroupCode).FirstOrDefault();

                        var LtAnnotationToolsDetails = context.AnnotationTools.Where(at => !at.IsDeleted && at.AnnotationGroupId == annotationGroupID.AnnotationGroupID).ToList();

                        List<AnnotationTools> annotationToolsList = new List<AnnotationTools>();

                        if (Templatename != null)
                        {
                            var LtAnnotationTools = context.AnnotationTemplateDetails.Where(atd => atd.AnnotationTemplateId == Templatename.AnnotationTemplateId).ToList();

                            if (LtAnnotationTools != null)
                            {
                                foreach (var item in LtAnnotationToolsDetails)
                                {
                                    AnnotationTools annotationTools = new AnnotationTools();

                                    if (LtAnnotationTools.Any(lta => lta.AnnotationToolId == item.AnnotationToolId && !lta.Isdeleted))
                                    {
                                        annotationTools.AnnotationToolName = item.AnnotationToolName;
                                        annotationTools.Path = item.Path;
                                        annotationTools.isChecked = true;
                                        annotationTools.AnnotationToolCode = item.AnnotationToolCode;
                                        annotationTools.ColorCode = item.ColorCode;
                                    }
                                    else
                                    {
                                        annotationTools.AnnotationToolName = item.AnnotationToolName;
                                        annotationTools.Path = item.Path;
                                        annotationTools.isChecked = false;
                                        annotationTools.AnnotationToolCode = item.AnnotationToolCode;
                                        annotationTools.ColorCode = item.ColorCode;
                                    }
                                    annotationToolsList.Add(annotationTools);
                                }

                            }
                            else
                            {
                                foreach (var item in LtAnnotationToolsDetails)
                                {
                                    AnnotationTools annotationTools = new AnnotationTools();
                                    annotationTools.AnnotationToolName = item.AnnotationToolName;
                                    annotationTools.Path = item.Path;
                                    annotationTools.isChecked = false;
                                    annotationTools.AnnotationToolCode = item.AnnotationToolCode;
                                    annotationTools.ColorCode = item.ColorCode;

                                    annotationToolsList.Add(annotationTools);
                                }
                            }

                        }
                        else
                        {
                            foreach (var item in LtAnnotationToolsDetails)
                            {
                                AnnotationTools annotationTools = new AnnotationTools();
                                annotationTools.AnnotationToolName = item.AnnotationToolName;
                                annotationTools.Path = item.Path;
                                annotationTools.isChecked = false;
                                annotationTools.AnnotationToolCode = item.AnnotationToolCode;
                                annotationTools.ColorCode = item.ColorCode;

                                annotationToolsList.Add(annotationTools);
                            }
                        }
                        annotationGroups.AnnotationTools = annotationToolsList;
                        Lt_AnnotationGroup.Add(annotationGroups);

                    }
                }

                else
                {
                    var DefaultTemplatename = await context.AnnotationTemplates.Where(x => x.AnnotationTemplateCode == "Default").FirstOrDefaultAsync();


                    qigAnnotationDetails.TemplateName = DefaultTemplatename.AnnotationTemplateName;
                    qigAnnotationDetails.TemplateCode = DefaultTemplatename.AnnotationTemplateCode;

                    foreach (var annotationGroupID in AnnotationGroupIDs)
                    {

                        AnnotationGroups objannotationGroups = new AnnotationGroups();

                        objannotationGroups.GroupName = context.AnnotationGroups.Where(ag => ag.AnnotationGroupId == annotationGroupID.AnnotationGroupID && !ag.Isdeleted).Select(x => x.AnnotationGroupName).FirstOrDefault();

                        objannotationGroups.GroupCode = context.AnnotationGroups.Where(ag => ag.AnnotationGroupId == annotationGroupID.AnnotationGroupID && !ag.Isdeleted).Select(x => x.AnnotationGroupCode).FirstOrDefault();


                        var LtAnnotationToolsDetails = context.AnnotationTools.Where(at => at.AnnotationGroupId == annotationGroupID.AnnotationGroupID && !at.IsDeleted).ToList();

                        List<AnnotationTools> annotationToolsList = new List<AnnotationTools>();

                        foreach (var item in LtAnnotationToolsDetails)
                        {
                            AnnotationTools objannotationTools = new AnnotationTools();

                            objannotationTools.AnnotationToolName = item.AnnotationToolName;
                            objannotationTools.Path = item.Path;
                            objannotationTools.isChecked = false;
                            objannotationTools.AnnotationToolCode = item.AnnotationToolCode;
                            objannotationTools.ColorCode = item.ColorCode;

                            annotationToolsList.Add(objannotationTools);
                        }

                        objannotationGroups.AnnotationTools = annotationToolsList;

                        Lt_AnnotationGroup.Add(objannotationGroups);
                    }
                }

                qigAnnotationDetails.AnnotationGroup = Lt_AnnotationGroup;
                return qigAnnotationDetails;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in AnnotationSettingRepository page while save QIG Annotation : Method Name : GetAnnotationForQIG() and ProjectID=" + projectID + "ProjectUserRoleID=" + projectUserRoleID);
                throw;
            }
        }
    }
}
