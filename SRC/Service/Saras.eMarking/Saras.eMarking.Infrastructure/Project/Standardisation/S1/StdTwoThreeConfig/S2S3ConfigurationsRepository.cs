using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.Standardisation.StdOne.StdTwoThreeConfig;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Categorisation;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.S2S3Configuraion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Standardisation.S1.StdTwoThreeConfig
{
    public class S2S3ConfigurationsRepository : BaseRepository<S2S3ConfigurationsRepository>, IStdTwoStdThreeConfigRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAppCache AppCache;
        public S2S3ConfigurationsRepository(ApplicationDbContext context, ILogger<S2S3ConfigurationsRepository> _logger, IAppCache _appCache) : base(_logger)
        {
            this.context = context;
            AppCache = _appCache;
        }

        /// <summary>
        /// GetQualifyScriptdetails : This GET Api is used to get the qualifying script details
        /// </summary>
        /// <param name="ProjectId">Project Id</param>
        /// <param name="QIGId">QIG Id</param>
        /// <returns>objQualifydetails</returns>
        public Task<QualifyingAssessmentCreationModel> GetQualifyScriptdetails(long ProjectId, long QIGId)
        {
            List<ScriptCategorizationPoolCreationModel> CategorizationPool = new List<ScriptCategorizationPoolCreationModel>();
            QualifyingAssessmentCreationModel objQualifydetails = new QualifyingAssessmentCreationModel();
            StandardizationQualifyingAssessment standardizationQualifyingAssessmentCreation = new StandardizationQualifyingAssessment();
            List<QualifyingAssessmentScriptDetailsCreationModel> lstpool = new List<QualifyingAssessmentScriptDetailsCreationModel>();

            try
            {
                logger.LogInformation($"StdTwoStdThreeConfigRepository GetQualifyScriptdetails() Method started.  projectId = {ProjectId},QIGId={QIGId}");

                CategorizationPool = (from sce in context.ScriptCategorizationPools
                                      join pus in context.ProjectUserScripts on sce.ScriptId equals pus.ScriptId
                                      join usm in context.UserScriptMarkingDetails on sce.UserScriptMarkingRefId equals usm.Id
                                      where sce.ProjectId == ProjectId && sce.Qigid == QIGId && !sce.IsDeleted && !pus.Isdeleted && !usm.IsDeleted && sce.PoolType == (int)ScriptCategorizationPoolType.StandardizationScript
                                      select new ScriptCategorizationPoolCreationModel
                                      {
                                          ScriptCategorizationPoolID = sce.ScriptCategorizationPoolId,
                                          ScriptID = sce.ScriptId,
                                          ScriptName = pus.ScriptName,
                                          MarkedBy = usm.MarkedBy,
                                          UserScriptMarkingRefID = sce.UserScriptMarkingRefId,
                                          MaxMarks = sce.MaxMarks,
                                          FinalizedMarks = sce.FinalizedMarks
                                      }).ToList();

                if (CategorizationPool != null && CategorizationPool.Count > 0)
                {
                    objQualifydetails = new QualifyingAssessmentCreationModel
                    {
                        ApprovalType = 2,
                        ScriptPresentationType = 2,
                        TotalNoOfScripts = CategorizationPool.Count,
                        ToleranceCount = 1
                    };
                    lstpool = new List<QualifyingAssessmentScriptDetailsCreationModel>();

                    CategorizationPool.ForEach(script =>
                    {
                        QualifyingAssessmentScriptDetailsCreationModel objpool = new()
                        {
                            ScriptID = script.ScriptID,
                            ScriptName = script.ScriptName,
                            MarkedBy = script.MarkedBy,
                            IsSelected = true,
                            MaxMarks = script.MaxMarks,
                            FinalizedMarks = script.FinalizedMarks,
                            ScriptCategorizationPoolID = script.ScriptCategorizationPoolID,
                            UserScriptMarkingRefID = script.UserScriptMarkingRefID,

                        };
                        lstpool.Add(objpool);
                    });

                    objQualifydetails.Qscriptdetails = lstpool;

                    standardizationQualifyingAssessmentCreation = context.StandardizationQualifyingAssessments.FirstOrDefault(a => a.ProjectId == ProjectId && a.Qigid == QIGId && !a.IsDeleted && a.IsActive == true);
                    if (standardizationQualifyingAssessmentCreation != null)
                    {
                        var partialResult = (from QAAssessment in context.StandardizationQualifyingAssessments
                                             join QAScripts in context.QualifyingAssessmentScriptDetails on QAAssessment.QualifyingAssessmentId equals QAScripts.QualifyingAssessmentId
                                             join sce in context.ScriptCategorizationPools on QAScripts.ScriptCategorizationPoolId equals sce.ScriptCategorizationPoolId
                                             where !QAScripts.IsDeleted &&
                                             QAScripts.QualifyingAssessmentId == standardizationQualifyingAssessmentCreation.QualifyingAssessmentId &&
                                             sce.ProjectId == ProjectId && sce.Qigid == QIGId && !sce.IsDeleted && !QAAssessment.IsDeleted &&
                                             sce.PoolType == (int)ScriptCategorizationPoolType.StandardizationScript &&
                                             QAAssessment.IsActive == true
                                             select new QualifyingAssessmentScriptDetailsCreationModel
                                             {
                                                 ScriptCategorizationPoolID = QAScripts.ScriptCategorizationPoolId,
                                                 IsSelected = QAScripts.IsSelected,
                                                 OrderIndex = QAScripts.OrderIndex,
                                                 ScriptID = sce.ScriptId,
                                                 QassessmentScriptId = QAScripts.QassessmentScriptId
                                             }).ToList();

                        objQualifydetails.Qscriptdetails.ForEach(stdScript =>
                        {
                            QualifyingAssessmentScriptDetailsCreationModel assmentscript = partialResult.FirstOrDefault(a => a.ScriptCategorizationPoolID == stdScript.ScriptCategorizationPoolID);
                            if (assmentscript != null)
                            {
                                stdScript.IsSelected = assmentscript.IsSelected;
                                stdScript.OrderIndex = assmentscript.OrderIndex;
                                stdScript.QassessmentScriptId = assmentscript.QassessmentScriptId;
                            }
                            else
                            {
                                stdScript.IsSelected = false;
                            }

                        });
                        objQualifydetails.QualifyingAssessmentId = standardizationQualifyingAssessmentCreation.QualifyingAssessmentId;
                        objQualifydetails.ApprovalType = standardizationQualifyingAssessmentCreation.ApprovalType;
                        objQualifydetails.ScriptPresentationType = standardizationQualifyingAssessmentCreation.ScriptPresentationType;
                        objQualifydetails.NoOfScriptSelected = standardizationQualifyingAssessmentCreation.NoOfScriptSelected;
                        objQualifydetails.TotalNoOfScripts = standardizationQualifyingAssessmentCreation.TotalNoOfScripts;
                        objQualifydetails.ToleranceCount = standardizationQualifyingAssessmentCreation.ToleranceCount;

                    }

                    if (objQualifydetails != null && objQualifydetails.Qscriptdetails != null)
                    {
                        if (objQualifydetails.Qscriptdetails.Any(a => a.OrderIndex == null))
                        {
                            int? maxorderindex = objQualifydetails.Qscriptdetails.Max(a => a.OrderIndex);
                            maxorderindex ??= 0;
                            maxorderindex++;
                            objQualifydetails.Qscriptdetails.ForEach(elem =>
                            {
                                elem.OrderIndex ??= maxorderindex++;

                            });
                        }

                        objQualifydetails.NoOfScriptSelected = objQualifydetails.Qscriptdetails.Count(a => a.IsSelected == true);
                        objQualifydetails.Qscriptdetails = objQualifydetails.Qscriptdetails.OrderBy(a => a.OrderIndex).ToList();
                        int indNo = 1;
                        objQualifydetails.Qscriptdetails.ForEach(indcout =>
                        {
                            indcout.IndexNo = indNo++;
                        });
                        objQualifydetails.TotalNoOfScripts = objQualifydetails.Qscriptdetails.Count;

                        objQualifydetails.NoOfStandardizationScriptCategorized = 0;
                        objQualifydetails.NoOfAdditionalStandizationScriptPoolCategorized = 0;
                        objQualifydetails.NoOfBenchMarkScriptPoolCategorized = 0;

                        ScriptCategorizationCount(objQualifydetails, QIGId, ProjectId);

                    }
                }

                logger.LogInformation($"StdTwoStdThreeConfigRepository -> GetQualifyScriptdetails() Method ended.  projectId = {ProjectId},QIGId={QIGId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StdTwoStdThreeConfigRepository->GetQualifyScriptdetails() for specific QIG and parameters are project: projectId = {ProjectId},QIGId={QIGId}");
                throw;
            }
            return Task.FromResult(objQualifydetails);

        }
        private void ScriptCategorizationCount(QualifyingAssessmentCreationModel objQualifydetails, long QIGId, long projectId)
        {
            var ScCount = context.ScriptCategorizationPools.Where(scp => scp.Qigid == QIGId && scp.ProjectId == projectId && !scp.IsDeleted)
                             .GroupBy(r => r.PoolType)
                             .Select(scp => new
                             {
                                 scp.Key,
                                 value = scp.Count()
                             });
            var list = ScCount.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];

                if (item.Key == 1)
                {
                    objQualifydetails.NoOfStandardizationScriptCategorized = ScCount.Where(x => x.Key == item.Key).Select(x => x.value).FirstOrDefault();
                }
                if (item.Key == 2)
                {
                    objQualifydetails.NoOfAdditionalStandizationScriptPoolCategorized = ScCount.Where(x => x.Key == item.Key).Select(x => x.value).FirstOrDefault();
                }
                if (item.Key == 3)
                {
                    objQualifydetails.NoOfBenchMarkScriptPoolCategorized = ScCount.Where(x => x.Key == item.Key).Select(x => x.value).FirstOrDefault();
                }
            }

            var scriptCategorized = context.QigstandardizationScriptSettings.FirstOrDefault(a => a.Qigid == QIGId && !a.Isdeleted);
            if (scriptCategorized != null)
            {
                objQualifydetails.StandardizationScriptPoolCount = scriptCategorized.StandardizationScript;
                objQualifydetails.AdditionalStandizationScriptPoolCount = scriptCategorized.AdditionalStdScript;
                objQualifydetails.BenchMarkScriptPoolCount = scriptCategorized.BenchmarkScript;
            }
            else
            {
                objQualifydetails.StandardizationScriptPoolCount = 0;
                objQualifydetails.AdditionalStandizationScriptPoolCount = 0;
                objQualifydetails.BenchMarkScriptPoolCount = 0;
            }
        }
        
        /// <summary>
        /// CreateQualifyingAssessment : This POST Api is used to Insert Qualifying assessment and Qualifying Script details
        /// </summary>
        /// <param name="objQualifyingAssessmentModel">objQualifyingAssessment Model</param>
        /// <param name="ProjectId">Project Id</param>
        /// <param name="QIGId">QIG Id</param>
        /// <param name="ProjectUserRoleID">ProjectUser RoleID</param>
        /// <returns></returns>
        public async Task<string> CreateQualifyingAssessment(QualifyingAssessmentCreationModel objQualifyingAssessmentModel, long ProjectId, long QIGId, long ProjectUserRoleID)
        {
            StandardizationQualifyingAssessment standardizationQualifyingAssessment;
            long QualifyingAssessmentID = 0;
            string status = "ER001";
            try
            {
                logger.LogInformation($"StdTwoStdThreeConfigRepository  CreateQualifyingAssessment() Method started.  projectId = {ProjectId},QIGId={QIGId} and ProjectUserRoleID = {ProjectUserRoleID}");

                string createstatus = CheckQIGWorkflowStatus(QIGId, context);
                if (!string.IsNullOrEmpty(createstatus))
                {
                    return createstatus;
                }

                standardizationQualifyingAssessment = await context.StandardizationQualifyingAssessments.Where(a => a.ProjectId == ProjectId && a.Qigid == QIGId && !a.IsDeleted && a.IsActive == true).FirstOrDefaultAsync();
                if (standardizationQualifyingAssessment != null)
                {
                    status = await UpdateQualifyingAssessment(objQualifyingAssessmentModel, ProjectId, ProjectUserRoleID);
                }
                else
                {
                    if (objQualifyingAssessmentModel != null)
                    {
                        standardizationQualifyingAssessment = new StandardizationQualifyingAssessment()
                        {
                            ProjectId = ProjectId,
                            Qigid = QIGId,
                            TotalNoOfScripts = objQualifyingAssessmentModel.TotalNoOfScripts,
                            NoOfScriptSelected = objQualifyingAssessmentModel.NoOfScriptSelected,
                            ScriptPresentationType = objQualifyingAssessmentModel.ScriptPresentationType,
                            IsTagged = objQualifyingAssessmentModel.IsTagged,
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = ProjectUserRoleID,
                            ApprovalType = objQualifyingAssessmentModel.ApprovalType,
                            ToleranceCount = objQualifyingAssessmentModel.ToleranceCount,
                            IsDeleted = false,
                        };
                        context.StandardizationQualifyingAssessments.Add(standardizationQualifyingAssessment);
                        using var dbContextTransaction = context.Database.BeginTransaction();
                        try
                        {
                            context.SaveChanges();
                            QualifyingAssessmentID = standardizationQualifyingAssessment.QualifyingAssessmentId;
                            if (QualifyingAssessmentID > 0 && objQualifyingAssessmentModel.Qscriptdetails != null && objQualifyingAssessmentModel.Qscriptdetails.Count > 0)
                            {
                                int ordercount = 1;
                                for (int item = 0; item < objQualifyingAssessmentModel.Qscriptdetails.Count; item++)
                                {
                                    QualifyingAssessmentScriptDetail qualifyingAssessmentScriptDetails = new()
                                    {
                                        QualifyingAssessmentId = QualifyingAssessmentID,
                                        IsSelected = objQualifyingAssessmentModel.Qscriptdetails[item].IsSelected,
                                        IsDeleted = false,
                                        OrderIndex = ordercount++,
                                        ScriptCategorizationPoolId = objQualifyingAssessmentModel.Qscriptdetails[item].ScriptCategorizationPoolID,
                                        CreatedDate = DateTime.UtcNow,
                                        CreatedBy = ProjectUserRoleID
                                    };
                                    context.QualifyingAssessmentScriptDetails.Add(qualifyingAssessmentScriptDetails);
                                }
                                context.SaveChanges();
                                dbContextTransaction.Commit();
                                status = "S001";
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"Error in StdTwoStdThreeConfigRepository ->CreateQualifyingAssessment() projectId = {ProjectId} ,QIGId={QIGId} and ProjectUserRoleID = {ProjectUserRoleID},QualifyingAssessmentID={QualifyingAssessmentID},status = {status}");
                            dbContextTransaction.Rollback();
                        }
                    }
                }
                logger.LogInformation($"StdTwoStdThreeConfigRepository ->CreateQualifyingAssessment() Method completed sucessfully. projectId = {ProjectId} ,QIGId={QIGId} and ProjectUserRoleID = {ProjectUserRoleID},QualifyingAssessmentID={QualifyingAssessmentID},status = {status}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StdTwoStdThreeConfigRepository ->CreateQualifyingAssessment() projectId = {ProjectId} ,QIGId={QIGId} and ProjectUserRoleID = {ProjectUserRoleID},QualifyingAssessmentID={QualifyingAssessmentID},status = {status}");
                throw;
            }

            return status;
        }

        /// <summary>
        /// UpdateQualifyingAssessment : This PATCH Api is used to Update Qualifying assessment and Qualifying Script details
        /// </summary>
        /// <param name="objQualifyingAssessmentModel">objQualifyingAssessment Model</param>
        /// <param name="ProjectId">Project Id</param>
        /// <param name="ProjectUserRoleID">ProjectUser RoleID</param>
        /// <returns></returns>
        public async Task<string> UpdateQualifyingAssessment(QualifyingAssessmentCreationModel objQualifyingAssessmentModel, long ProjectId, long ProjectUserRoleID)
        {
            StandardizationQualifyingAssessment updateQualifyingAssessment;
            long QualifyingAssessmentID = 0;
            string status = "ER001";

            try
            {
                logger.LogInformation($"StdTwoStdThreeConfigRepository  UpdateQualifyingAssessment() Method started.  projectId = {ProjectId},QIGId={objQualifyingAssessmentModel.QigId} and ProjectUserRoleID = {ProjectUserRoleID}");

                string updatestatus = CheckQIGWorkflowStatus(objQualifyingAssessmentModel.QigId, context);
                if (!string.IsNullOrEmpty(updatestatus))
                {
                    return updatestatus;
                }

                updateQualifyingAssessment = await context.StandardizationQualifyingAssessments.Where(a => a.ProjectId == ProjectId && a.Qigid == objQualifyingAssessmentModel.QigId && !a.IsDeleted).FirstOrDefaultAsync();

                if (updateQualifyingAssessment != null)
                {
                    updateQualifyingAssessment.NoOfScriptSelected = objQualifyingAssessmentModel.NoOfScriptSelected;
                    updateQualifyingAssessment.ModifiedBy = ProjectUserRoleID;
                    updateQualifyingAssessment.ModifiedDate = DateTime.UtcNow;
                    updateQualifyingAssessment.IsDeleted = false;
                    updateQualifyingAssessment.ToleranceCount = objQualifyingAssessmentModel.ToleranceCount;
                    updateQualifyingAssessment.TotalNoOfScripts = objQualifyingAssessmentModel.TotalNoOfScripts;
                    updateQualifyingAssessment.NoOfScriptSelected = objQualifyingAssessmentModel.NoOfScriptSelected;
                    updateQualifyingAssessment.ScriptPresentationType = objQualifyingAssessmentModel.ScriptPresentationType;
                    updateQualifyingAssessment.ApprovalType = objQualifyingAssessmentModel.ApprovalType;
                    updateQualifyingAssessment.IsTagged = objQualifyingAssessmentModel.IsTagged;


                    QualifyingAssessmentID = updateQualifyingAssessment.QualifyingAssessmentId;
                    if (QualifyingAssessmentID > 0 && objQualifyingAssessmentModel.Qscriptdetails != null && objQualifyingAssessmentModel.Qscriptdetails.Count > 0)
                    {
                        int rowcount = 1;
                        objQualifyingAssessmentModel.Qscriptdetails.ForEach(scrpt =>
                        {
                            scrpt.OrderIndex = rowcount++;
                        });

                        List<QualifyingAssessmentScriptDetail> updateQScriptdetailsValues = (await (from QA in context.StandardizationQualifyingAssessments
                                                                                                    join QASD in context.QualifyingAssessmentScriptDetails on QA.QualifyingAssessmentId equals QASD.QualifyingAssessmentId
                                                                                                    where QASD.QualifyingAssessmentId == QualifyingAssessmentID && QA.IsActive == true && !QA.IsDeleted && !QA.IsTagged && !QASD.IsDeleted
                                                                                                    select QASD).ToListAsync()).ToList();

                        if (updateQScriptdetailsValues != null && updateQScriptdetailsValues.Count > 0)
                        {
                            context.StandardizationQualifyingAssessments.Update(updateQualifyingAssessment);

                            objQualifyingAssessmentModel.Qscriptdetails.ForEach(resp =>
                            {
                                QualifyingAssessmentScriptDetail objscriptentity = updateQScriptdetailsValues.FirstOrDefault(a => a.QualifyingAssessmentId == QualifyingAssessmentID && a.QassessmentScriptId == resp.QassessmentScriptId && a.ScriptCategorizationPoolId == resp.ScriptCategorizationPoolID);
                                if (objscriptentity != null)
                                {
                                    objscriptentity.ModifiedDate = DateTime.UtcNow;
                                    objscriptentity.ModifiedBy = ProjectUserRoleID;
                                    objscriptentity.OrderIndex = resp.OrderIndex;
                                    objscriptentity.IsSelected = resp.IsSelected;
                                    context.QualifyingAssessmentScriptDetails.Update(objscriptentity);
                                }
                                else
                                {
                                    context.QualifyingAssessmentScriptDetails.Add(new QualifyingAssessmentScriptDetail
                                    {
                                        QualifyingAssessmentId = QualifyingAssessmentID,
                                        ScriptCategorizationPoolId = resp.ScriptCategorizationPoolID,
                                        IsSelected = resp.IsSelected,
                                        OrderIndex = resp.OrderIndex,
                                        IsDeleted = false,
                                        CreatedBy = ProjectUserRoleID,
                                        CreatedDate = DateTime.UtcNow
                                    });
                                }

                            });
                            context.SaveChanges();

                            status = "U001";
                        }
                    }

                }
                logger.LogInformation($"StdTwoStdThreeConfigRepository ->UpdateQualifyingAssessment() Method completed sucessfully. projectId = {ProjectId} ,QIGId={objQualifyingAssessmentModel.QigId} and ProjectUserRoleID = {ProjectUserRoleID},status = {status}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StdTwoStdThreeConfigRepository ->UpdateQualifyingAssessment() projectId = {ProjectId} ,QIGId={objQualifyingAssessmentModel.QigId} and ProjectUserRoleID = {ProjectUserRoleID},status = {status}");
                throw;
            }
            return status;
        }

        /// <summary>
        /// CreateWorkflowStatusTracking : This POST Api is used to create remarks for s1
        /// </summary>
        /// <param name="objS1CompltedModel">objS1Complted Model</param>
        /// <param name="WorkflowID">Workflow ID</param>
        /// <param name="ProjectUserRoleID">ProjectUser RoleID</param>
        /// <param name="ProjectID">Project ID</param>
        /// <returns>status</returns>
        public async Task<string> CreateWorkflowStatusTracking(S1Complted objS1CompltedModel, long WorkflowID, long ProjectUserRoleID, long ProjectID)
        {
            string status;
            try
            {

                List<ScriptCategorizationPool> categorizationPoolEntities = context.ScriptCategorizationPools.Distinct().Where(x => x.Qigid == objS1CompltedModel.EntityID && !x.IsDeleted && x.PoolType > 0).ToList();

                QigstandardizationScriptSetting qigScriptSetting = context.QigstandardizationScriptSettings.FirstOrDefault(x => x.Qigid == objS1CompltedModel.EntityID && !x.Isdeleted);


                StandardizationQualifyingAssessment qassessment = await context.StandardizationQualifyingAssessments.Where(a => a.Qigid == objS1CompltedModel.EntityID && !a.IsDeleted).FirstOrDefaultAsync();
                CategorisationStasticsModel categorisationStastics = new()
                {
                    StandardisedScript = categorizationPoolEntities.Count(a => a.PoolType == (int)ScriptCategorizationPoolType.StandardizationScript),
                    AdlStandardisedScript = categorizationPoolEntities.Count(a => a.PoolType == (int)ScriptCategorizationPoolType.AdditionalStandardizationScript),
                    BenchMarkScript = categorizationPoolEntities.Count(a => a.PoolType == (int)ScriptCategorizationPoolType.BenchMarkScript)
                };

                var lt_s1completed = (from PWFT in context.ProjectWorkflowStatusTrackings
                                      join WFS in context.WorkflowStatuses on PWFT.WorkflowStatusId equals WFS.WorkflowId
                                      where WFS.WorkflowCode == StringEnum.GetStringValue(EnumWorkflowStatus.Standardization_1) && PWFT.EntityId == objS1CompltedModel.EntityID && PWFT.EntityType == 2 && !PWFT.IsDeleted && !WFS.IsDeleted
                                      select new { PWFT, WFS }).ToList();
                int? S1Completed = lt_s1completed.OrderByDescending(a => a.PWFT.ProjectWorkflowTrackingId).Select(a => a.PWFT.ProcessStatus).FirstOrDefault();

                if (S1Completed == (int)WorkflowProcessStatus.Completed)
                {
                    return "S1AlreadyCompleted";
                }

                if (qassessment == null)
                {
                    return "ASMNTC";
                }

                if (qigScriptSetting.StandardizationScript > categorisationStastics.StandardisedScript || qigScriptSetting.BenchmarkScript > categorisationStastics.BenchMarkScript || qigScriptSetting.AdditionalStdScript > categorisationStastics.AdlStandardisedScript)
                {
                    return "TRGTNOTREACH";
                }

                ProjectWorkflowStatusTracking wrkflow = await context.ProjectWorkflowStatusTrackings.Where(a => a.EntityId == objS1CompltedModel.EntityID && a.EntityType == (int)EnumAppSettingEntityType.QIG && a.WorkflowStatusId == AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Standardization_1, EnumWorkflowType.Qig) && a.ProcessStatus == (int)WorkflowProcessStatus.Completed && !a.IsDeleted).FirstOrDefaultAsync();
                if (wrkflow != null)
                {
                    wrkflow.IsDeleted = true;
                    context.ProjectWorkflowStatusTrackings.Update(wrkflow);
                }

                context.ProjectWorkflowStatusTrackings.Add(new ProjectWorkflowStatusTracking()
                {
                    EntityId = objS1CompltedModel.EntityID,
                    EntityType = (int)EnumAppSettingEntityType.QIG,
                    WorkflowStatusId = AppCache.GetWorkflowStatusId(EnumWorkflowStatus.Standardization_1, EnumWorkflowType.Qig),
                    ProcessStatus = (int)WorkflowProcessStatus.Completed,
                    Remarks = objS1CompltedModel.Remarks,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = ProjectUserRoleID,
                });

                new ShareRepository(logger).MoveUncategorisedScripttoLiveMarking(objS1CompltedModel.EntityID, ProjectUserRoleID, ProjectID, AppCache, context);

                context.SaveChanges();

                status = "P001";
            }

            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StdTwoStdThreeConfigRepository page while creating CreateWorkflowStatusTracking : Method Name : CreateWorkflowStatusTracking()");
                throw;
            }
            return status;
        }


        /// <summary>
        /// GetS1CompletedRemarks : This GET Api is used to get thw remarks for s1 completed.
        /// </summary>
        /// <param name="EntityID">Entity ID</param>
        /// <param name="EntityType">Entity Type</param>
        /// <param name="WorkflowStatusID">WorkflowStatus ID</param>
        /// <returns>s1completedremarks</returns>
        public async Task<List<S1Complted>> GetS1CompletedRemarks(long EntityID, byte EntityType, int WorkflowStatusID)
        {
            List<S1Complted> s1completedremarks = null;
            try
            {
                s1completedremarks = (await (from pwt in context.ProjectWorkflowStatusTrackings
                                             where pwt.EntityId == EntityID && pwt.EntityType == EntityType && pwt.WorkflowStatusId == WorkflowStatusID && !pwt.IsDeleted
                                             select new S1Complted
                                             {
                                                 EntityID = pwt.EntityId,
                                                 EntityType = pwt.EntityType,
                                                 Remarks = pwt.Remarks,
                                                 ProcessStatus = pwt.ProcessStatus,
                                                 ProjectWorkflowTrackingID = pwt.ProjectWorkflowTrackingId,
                                                 WorkflowStatusId = pwt.WorkflowStatusId
                                             }).ToListAsync()).ToList();
                if (s1completedremarks != null)
                {
                    s1completedremarks.ForEach(a =>
                    {
                        a.WorkflowStatusCode = AppCache.GetWorkflowStatusCode(a.WorkflowStatusId, EnumWorkflowType.Qig);
                    });

                    s1completedremarks = s1completedremarks.OrderBy(a => a.ProjectWorkflowTrackingID).ToList();
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in StdTwoStdThreeConfigRepository page while getting for specific Qigs remarks: Method Name: GetS1CompletedRemarks() and project: EntityID=" + EntityID.ToString() + ", EntityType=" + EntityType.ToString() + ", WorkflowStatusID=" + WorkflowStatusID.ToString() + "");
                throw;
            }
            return s1completedremarks;

        }

    }
}